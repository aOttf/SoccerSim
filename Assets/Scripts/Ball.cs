using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;

public class Ball : SteerEntity
{
    public PlayerAgent owner;
    [Range(0, 1f)] public float followFactor;
    private GameManager m_manager;

    #region Caches

    private Transform m_ts;
    private PlayerController m_controller;

    private float m_kickStrength;
    private float m_drag;
    private float m_dribbleDistance;

    private Vector3 m_velocity; //Current Velocity of the ball

    public override Vector3 linearVelocity { get => m_velocity; set => throw new System.NotSupportedException(); }

    public override Vector3 position => m_ts.position;

    public bool HasOwner => owner != null;

    #endregion Caches

    private void Awake()
    {
        //Find Game Manager
        m_manager = GameManager.Instance;

        //Initialize Parameters
        m_ts = GetComponent<Transform>();
        m_controller = m_manager.controller;

        m_kickStrength = m_manager.kickStrength;
        m_drag = m_manager.dragStrength;
        m_dribbleDistance = m_manager.dribbleDistance;
        //m_holdoffTime = m_manager.holdoffTime;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateOwner();
        if (HasOwner)
            owner.OnCarryingBall();

        UpdateVelocity();

        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        if (HasOwner)
            m_velocity = owner.linearVelocity;
        else
            m_velocity *= m_drag;
    }

    private void UpdatePosition()
    {
        if (HasOwner)
        {
            //Lerp from current position to the owner position with offset
            //transform.position = Vector3.Slerp(transform.position, owner.OffsetPosition, followFactor);
            transform.position = owner.OffsetPosition;
        }
        else
        {
            //Not dribbled, ball physics
            Vector3 newPosition = m_ts.position + m_velocity * Time.deltaTime;
            if (m_manager.OutOfPitchWidth(newPosition))
            {
                m_velocity = Vector3.Reflect(m_velocity, Vector3.left);
            }
            else if (m_manager.OutOfPitchHeight(newPosition))
            {
                m_velocity = Vector3.Reflect(m_velocity, Vector3.forward);
            }
            else
                m_ts.position = newPosition;
        }
    }

    private void UpdateOwner()
    {
        //If the ball doesn't have an owner, check if any player can snatch the ball
        if (!owner)
            foreach (var team in m_manager.players)
            {
                foreach (var player in team)
                    if (TrySnatch(player))
                        return;
            }
        else
        {
            //If the ball has an owner, only checks the players in the opposite team
            List<PlayerAgent> otherTeam = m_manager.players[((int)owner.TeamColor + 1) % 2];
            foreach (var opponent in otherTeam)
                if (TrySnatch(opponent))
                    return;
        }
    }

    /// <summary>
    /// Returns the predicted time for the ball to travel the distance
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public int Predict(float distance)
    {
        int time = default;
        float spd = m_kickStrength;
        while (distance > float.Epsilon && spd > .25f)
        {
            distance -= spd;
            spd *= m_drag;
            time++;
        }

        return time;
    }

    private bool TrySnatch(PlayerAgent player)
    {
        if (OnSnatch(player))
        {
            //Update ball's owner
            owner = player;
            owner.OnSnatchBall();

            ////Update player's max speed
            //player.maxLinearSpeed = player.maxCarryingSpeed;

            ////Reset owner's invicible time
            //owner.OnResetHoldOffTimer();

            ////If the player is on human-controlled team, switch controlling player to this
            //if (player.IsHuman)
            //    m_controller.OnSwitchPlayer(player);

            return true;
        }

        return false;
    }

    /// <summary>
    /// If Player can snatch the soccer
    /// Condition1 : Soccer doesn't have an owner or the invicible time of the owner has passed
    /// Condition2 : close enough to the soccer
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool OnSnatch(PlayerAgent player)
    {
        if (owner)
        {
            print(Vector3.SqrMagnitude(owner.position - player.position));
            return !owner.IsInvulnerable && Vector3.SqrMagnitude(owner.position - player.position) < 1f;
        }

        return Vector3.SqrMagnitude(position - player.position) < 1f;
    }

    public void OnKick(Vector3 dir)
    {
        m_velocity += m_kickStrength * dir;
    }
}