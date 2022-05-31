using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;

[RequireComponent(typeof(PlayerAgent))]
public abstract class PlayerStateBehaviour : FSMStateBehaviour
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