using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;

/// <summary>
/// Behavior State of player being controlled by human
/// </summary>
public class OnControl : PlayerFSMBase
{
    public bool clearSteeringCaches;

    #region Caches

    private PlayerController m_controller;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();

        //Cache Controller
        m_controller = m_manager.controller;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        m_player.StopSteering(false);
    }

    public override void OnExit()
    {
        base.OnExit();
        m_player.StartSteering();
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