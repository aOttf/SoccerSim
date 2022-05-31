using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public PlayerAgent owner;
    [Range(0, 1f)] public float followFactor;
    private GameManager m_manager;

    #region Caches

    private Transform m_ts;
    private float m_kickStrength;
    private float m_drag;
    private float m_dribbleDistance;

    private Vector3 m_velocity; //Current Velocity of the ball

    #endregion Caches

    private void Awake()
    {
        //Find Game Manager
        m_manager = GameManager.Instance;

        //Initialize Parameters
        m_ts = GetComponent<Transform>();

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
        UpdateVelocity();
        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        if (!owner)
            m_velocity *= m_drag;
    }

    private void UpdatePosition()
    {
        if (owner)
        {
            //Lerp from current position to the owner position with offset
            transform.position = Vector3.Slerp(transform.position, owner.OffsetPosition, followFactor);
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
            owner = player;

            //Reset player's hold off time
            player.OnResetHoldOffTimer();

            return true;
        }

        return false;
    }

    /// <summary>
    /// If Player can snatch the soccer
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool OnSnatch(PlayerAgent player)
    {
        return (!owner || owner.TeamColor != player.TeamColor) && Vector3.SqrMagnitude(transform.position - player.transform.position) < m_dribbleDistance * m_dribbleDistance;
    }

    public void OnKick(Vector3 dir)
    {
        m_velocity += m_kickStrength * dir;
    }
}