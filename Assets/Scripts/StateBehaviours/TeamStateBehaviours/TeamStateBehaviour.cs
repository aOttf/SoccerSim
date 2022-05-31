using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;

[RequireComponent(typeof(TeamAgent))]
public abstract class TeamStateBehaviour : FSMStateBehaviour
{
    #region Caches

    private GameManager m_manager;
    private TeamAgent m_team;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();
        m_manager = GameManager.Instance;
        m_team = GetComponent<TeamAgent>();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}