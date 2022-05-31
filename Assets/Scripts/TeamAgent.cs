using System.Collections;
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
    [SerializeField] private BlackboardManager m_blackboard;
    private Ball m_soccer;

    public List<PlayerAgent> teammates;

    #endregion Caches

    [SerializeField] private TeamColor m_color;

    [Tooltip("The Team is controlled by Player(YOU)")]
    [SerializeField] private bool m_isControlled;
    public TeamColor TeamColor => m_color;

    private void Awake()
    {
        m_manager = GameManager.Instance;
        m_soccer = m_manager.soccer;
        //m_blackboard = GetComponent<BlackboardManager>();
        print(m_blackboard);
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateFSMParams();
    }

    private void UpdateFSMParams()
    {
        //m_blackboard.SetBool("hasBall", true);
    }
}