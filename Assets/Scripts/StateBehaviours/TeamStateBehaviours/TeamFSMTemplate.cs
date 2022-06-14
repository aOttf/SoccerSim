using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;

/// <summary>
/// The base class every Team FSMBase class derives from
/// </summary>
[RequireComponent(typeof(TeamAgent))]
public abstract class TeamFSMBase : FSMBase
{
    #region Caches

    protected GameManager m_manager;
    protected TeamAgent m_team;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();
        m_manager = GameManager.Instance;
        m_team = GetComponent<TeamAgent>();
    }
}

/// <summary>
/// The Base Class every Team Sub State Machine derives from
/// </summary>
[RequireComponent(typeof(TeamAgent))]
public abstract class TeamFSM : FSM
{
    protected GameManager m_manager;
    protected TeamAgent m_team;

    protected override void Awake()
    {
        base.Awake();
        m_manager = GameManager.Instance;
        m_team = GetComponent<TeamAgent>();
    }
}