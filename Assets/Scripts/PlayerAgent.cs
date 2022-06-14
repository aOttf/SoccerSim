using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

//[RequireComponent(typeof(FSM), typeof(BlackboardManager))]
public class PlayerAgent : SteerAgent
{
    [Header("Player Agent")]
    [Space(10)]
    [Header("On Control Params")]
    public Material onControlMat;
    public Material offControlMat;

    [Tooltip("The start and waiting position of the player.")]
    public Vector3 homePosition;

    [Tooltip("max speed when not carrying a ball")]
    public float maxNormalSpeed;

    [Tooltip("max speed when carrying a ball")]
    public float maxCarryingSpeed;

    [HideInInspector]
    public int playerID;

    #region Caches

    private PlayerController m_controller;
    private TeamAgent m_team;
    private GameManager m_manager;
    private BlackboardManager m_bb;
    private RootFSM m_fsm;
    private MeshRenderer m_renderer;
    private Transform m_ballHandler;

    private bool m_isControlled = false;    //The player is controlled by human

    private float m_invulnerableCountdown;  //Remaining invulnerable time
    private float m_dontSnatchCountdown;    //Remaining dontSnatch time

    #endregion Caches

    [Tooltip("Total Invulnerable Time after the player snatches the ball")]
    [SerializeField] private float m_invulnerableTime;

    [Tooltip("After the player kicks the ball, how long it doesn't re-snatch it.")]
    [SerializeField] private float m_dontSnatchTime;
    [SerializeField] private TeamColor m_color;

    /// <summary>
    /// the position of the ball when player is dribbling in world space
    /// </summary>
    public Vector3 OffsetPosition => m_ballHandler.position;

    public float InvulnerableTime => m_invulnerableTime;

    public float InvulnerableCountdown => m_invulnerableCountdown;
    public float DontSnatchCountdown => m_dontSnatchCountdown;

    /// <summary>
    /// If the player can be snatched by opponents
    /// </summary>
    public bool IsInvulnerable => InvulnerableCountdown > 0f;

    /// <summary>
    /// Soccer
    /// </summary>
    public Ball soccer => m_manager.soccer;

    public List<PlayerAgent> teammates => m_team.teammates;

    /// <summary>
    /// Color of the team
    /// </summary>
    public TeamColor TeamColor => m_color;

    protected override void Awake()
    {
        base.Awake();

        //Init max linear speed since soccer is not carried at beginning
        maxLinearSpeed = maxNormalSpeed;

        m_manager = GameManager.Instance;
        m_controller = m_manager.controller;
        m_team = TeamColor == TeamColor.Blue ? m_manager.blueTeam : m_manager.redTeam;
        m_bb = GetComponent<BlackboardManager>();
        m_fsm = GetComponent<RootFSM>();

        m_ballHandler = transform.GetChild(0);

        //Cache Parameters
        m_invulnerableTime = m_manager.holdoffTime;
        m_invulnerableCountdown = m_invulnerableTime;

        m_renderer = GetComponent<MeshRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        StartSteering();
        UpdateFSMParams();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.position = m_manager.ClampPosition(position);

        if (!IsDribbler)
        {
            OnChasingBall();
        }

        UpdateFSMParams();
    }

    /// <summary>
    /// Update Blackboard params associated with the FSM
    /// </summary>
    private void UpdateFSMParams()
    {
        m_bb.SetBool("tooFar", TooFar);
        m_bb.SetBool("isHome", IsHome);
        m_bb.SetBool("teamHasBall", TeamHasBall);
        m_bb.SetBool("hasDribbler", HasDribbler);
        m_bb.SetBool("isDribbler", IsDribbler);
        m_bb.SetBool("isControlled", IsControlled);
    }

    #region Callbacks

    /// <summary>
    /// Callback invoked when player kicks the soccer
    /// </summary>
    public void OnKick()
    {
        if (IsDribbler)
        {
            //Update ball's owner
            Ball soccer = this.soccer;
            soccer.owner = null;

            //OnLoseBall();

            soccer.OnKick(forward);
        }
    }

    /// <summary>
    /// Callback invoked when human enters controlling the player
    /// </summary>
    /// <param name="clearSteeringData"></param>
    public override void OnControlEnter(bool clearSteeringData)
    {
        base.OnControlEnter(clearSteeringData);
        ControlEnter();
    }

    /// <summary>
    /// Callback invoked when human leaves controlling the player
    /// </summary>
    public void OnControlExit()
    {
        ControlExit();
    }

    protected void ControlEnter()
    {
        //Change Color Indicator
        m_renderer.material = onControlMat;
        m_isControlled = true;
    }

    protected void ControlExit()
    {
        //Change Color Indicator
        m_renderer.material = offControlMat;
        m_isControlled = false;
    }

    /// <summary>
    /// Callback invoked when player successfully snatches a ball
    /// </summary>
    public void OnSnatchBall()
    {
        //Reset Invulnerable time
        OnResetHoldOffTimer();

        //If the player is on human-controlled team, switch controlling player to this
        if (IsHuman)
            m_controller.OnSwitchPlayer(this);
    }

    /// <summary>
    /// Callback invocked when player is the owner of the ball
    /// </summary>
    public void OnCarryingBall()
    {
        OnUpdateInvulnerableTimer(Time.deltaTime);

        if (!Mathf.Approximately(maxLinearSpeed, maxCarryingSpeed))
        {
            maxLinearSpeed = Mathf.MoveTowards(maxLinearSpeed, maxCarryingSpeed, Time.deltaTime);
        }
    }

    /// <summary>
    /// Callback invokes when player is not the owner of the ball
    /// </summary>
    public void OnChasingBall()
    {
        if (!Mathf.Approximately(maxLinearSpeed, maxNormalSpeed))
        {
            maxLinearSpeed = Mathf.MoveTowards(maxLinearSpeed, maxNormalSpeed, Time.deltaTime);
        }
    }

    private void OnUpdateInvulnerableTimer(float deltaTime)
    {
        if (m_invulnerableCountdown > 0f)
            m_invulnerableCountdown -= deltaTime;
    }

    private void OnResetHoldOffTimer()
    {
        m_invulnerableCountdown = m_invulnerableTime;
    }

    /// <summary>
    /// Callback invoked when entering the match
    /// </summary>
    public void OnMatchEnter()
    {
        //Turn On Root FSM
        m_fsm.TurnOn();
    }

    /// <summary>
    /// Callback invoked when exiting the match
    /// </summary>
    public void OnMatchExit()
    {
        //Turn Off Root FSM
        m_fsm.TurnOff();
    }

    #endregion Callbacks

    #region FSM Trigger Events

    /// <summary>
    /// The player is too far away from the soccer
    /// </summary>
    public bool TooFar => Mathf.Abs(position.x - m_manager.soccer.transform.position.x) > m_manager.playerActiveDistance;

    /// <summary>
    /// The soccer is dribbled by one of the players of our team
    /// </summary>
    public bool TeamHasBall => soccer.HasOwner && soccer.owner.TeamColor == TeamColor;

    /// <summary>
    /// The soccer is dribbled by a player
    /// </summary>
    public bool HasDribbler => soccer.HasOwner;

    /// <summary>
    /// Is the player in a human-controlled team
    /// </summary>
    public bool IsHuman => m_team.IsHuman;

    /// <summary>
    /// The player is controlling the soccer
    /// </summary>
    public bool IsDribbler => m_manager.soccer.owner == this;

    /// <summary>
    /// The player is controlled by human
    /// </summary>
    public bool IsControlled { get => m_isControlled; set => m_isControlled = value; }

    /// <summary>
    /// The Player has arrived home position
    /// </summary>
    /// <returns></returns>
    public bool IsHome => position == homePosition && m_velocity == Vector3.zero;

    #endregion FSM Trigger Events

    /// <summary>
    /// Test if the player is able to kick the ball to the target
    /// 1.no opponents inside the forward arc
    /// 2.the target is inside the forward arc
    /// </summary>
    /// <param name="target"></param>
    /// <param name="testDistance"></param>
    /// <param name="testAngle"></param>
    /// <returns></returns>
    public bool CanKick(Vector3 target, float testDistance, float testAngle)
        => ArcTest(target, testDistance, testAngle) &&
        !m_manager.Opponents(TeamColor).Any(oppo => ArcTest(oppo.position, testDistance, testAngle));

    public bool ArcTest(Vector3 target, float dist, float angle)
    {
        Vector3 toTarget = target - position;

        //Test angle requirement
        if (Vector3.Angle(toTarget, forward) > angle)
        {
            return false;
        }

        //Test distance requirement
        if (Vector3.SqrMagnitude(toTarget) > dist * dist)
        {
            return false;
        }

        //pass
        return true;
    }
}