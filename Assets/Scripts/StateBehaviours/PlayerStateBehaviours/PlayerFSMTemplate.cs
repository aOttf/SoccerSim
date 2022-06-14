using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;

/// <summary>
/// This is the base class every Player FSMBase derives from
/// </summary>
[RequireComponent(typeof(PlayerAgent), typeof(RootFSM))]
public abstract class PlayerFSMBase : FSMBase
{
    protected GameManager m_manager;
    protected PlayerAgent m_player;

    protected override void Awake()
    {
        base.Awake();
        m_manager = GameManager.Instance;
        m_player = GetComponent<PlayerAgent>();
    }
}

/// <summary>
/// This is the base class every Player Sub State Machine derives from
/// </summary>
[RequireComponent(typeof(PlayerAgent), typeof(RootFSM))]
public abstract class PlayerFSM : FSM
{
    protected GameManager m_manager;
    protected PlayerAgent m_player;

    protected override void Awake()
    {
        base.Awake();
        m_manager = GameManager.Instance;
        m_player = GetComponent<PlayerAgent>();
    }
}