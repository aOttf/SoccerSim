using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

//[RequireComponent(typeof(FSM), typeof(BlackboardManager))]
public class PlayerAgent : SteerAgent
{
    [Tooltip("The start and waiting position of the player.")]
    public Vector3 homePosition;

    #region Caches

    private GameManager m_manager;
    private BlackboardManager m_blackboard;

    private Transform m_ballHandler;
    private float m_holdoffTime;

    #endregion Caches

    [SerializeField] private float m_holdoffTimer;
    [SerializeField] private TeamColor m_color;

    public Vector3 OffsetPosition => m_ballHandler.position;
    public float HoldoffTime => m_holdoffTime;

    public float HoldoffTimer => m_holdoffTimer;
    public TeamColor TeamColor => m_color;
    public Ball soccer => m_manager.soccer;

    protected override void Awake()
    {
        base.Awake();
        m_manager = GameManager.Instance;
        // m_blackboard = GetComponent<BlackboardManager>();

        m_ballHandler = transform.GetChild(0);
        //Cache Parameters
        m_holdoffTime = m_manager.holdoffTime;
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        transform.position = m_manager.ClampPosition(position);
        UpdateTimer(Time.deltaTime);
        //UpdateFSMParams();
    }

    private void UpdateTimer(float deltaTime)
    {
        if (m_holdoffTimer > 0f)
            m_holdoffTimer -= deltaTime;
    }

    private void UpdateFSMParams()
    {
        m_blackboard.SetBool("closeToBall", Mathf.Abs(position.x - m_manager.soccer.transform.position.x) < m_manager.playerActiveDistance);
        m_blackboard.SetBool("isHome", IsHome);
        m_blackboard.SetBool("hasBall", HasBall);
    }

    public void OnResetHoldOffTimer()
    {
        m_holdoffTimer = m_holdoffTime;
    }

    #region Action Sets

    public void Kick()
    {
        if (HasBall)
        {
            Ball soccer = this.soccer;
            soccer.owner = null;
            soccer.OnKick(forward);
        }
    }

    /// <summary>
    /// The Player has arrived home position
    /// </summary>
    /// <returns></returns>
    public bool IsHome => position == homePosition && m_velocity == Vector3.zero;

    public bool HasBall => m_manager.soccer.owner == this;

    #endregion Action Sets
}