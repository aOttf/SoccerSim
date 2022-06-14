using System.Collections;
using System.Linq;

using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;

/// <summary>
/// Color Identity of two teams
/// </summary>
public enum TeamColor
{
    Red, Blue
}

//[RequireComponent(typeof(FSM))]
public class TeamAgent : MonoBehaviour
{
    #region Caches

    private GameManager m_manager;
    private BlackboardManager m_bb;
    private RootFSM m_fsm;
    private Ball m_soccer;

    private int m_nextTeammateID = 0;
    private int m_score = 0;

    #endregion Caches

    public List<PlayerAgent> teammates;

    [SerializeField] private TeamColor m_color;

    [Tooltip("The Team is controlled by Human(YOU)")]
    [SerializeField] private bool m_isHuman;

    public TeamColor TeamColor => m_color;
    public bool IsHuman => m_isHuman;

    //Current Teammate dribbling the soccer, otherwise it returns null
    public PlayerAgent DribblingPlayer => m_soccer.owner && m_soccer.owner.TeamColor == m_color ? m_soccer.owner : null;

    private void Awake()
    {
        //Cache Components
        m_manager = GameManager.Instance;
        m_bb = GetComponent<BlackboardManager>();
        m_fsm = GetComponent<RootFSM>();

        m_soccer = m_manager.soccer;

        //Assign IDs to teammates
        foreach (var p in teammates)
            p.playerID = m_nextTeammateID++;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_fsm.TurnOn();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateFSMParams();
    }

    private void UpdateFSMParams()
    {
        //Match is kicked off if someone owns the ball
        if (m_soccer.owner != null)
        {
            m_bb.SetTrigger("hasKickedOff");
        }
    }

    /// <summary>
    /// Nearest Teammate to the soccer.
    /// If the team is controlling the soccer, returns the dribbling player
    /// </summary>
    /// <returns></returns>
    public PlayerAgent NearestTeammateToBall()
    {
        if (DribblingPlayer)
            return DribblingPlayer;

        PlayerAgent nearest = null;
        float dist = float.MaxValue;
        foreach (var p in teammates)
        {
            float curDist;
            if ((curDist = Vector3.Distance(p.position, m_soccer.position)) < dist)
            {
                dist = curDist;
                nearest = p;
            }
        }

        return nearest;
    }

    #region Callbacks

    /// <summary>
    /// Callback invoked when entering the match
    /// </summary>
    public void OnMatchEnter()
    {
        teammates.ForEach(p => p.OnMatchEnter());
    }

    /// <summary>
    /// Callback invoked when exiting the match
    /// </summary>
    public void OnMatchExit()
    {
        teammates.ForEach(p => p.OnMatchExit());
    }

    /// <summary>
    /// Callback invoked when the team makes a successful shoot
    /// </summary>
    public void OnScore()
    {
        m_score++;
    }

    #endregion Callbacks
}