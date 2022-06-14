using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

public class Returning : PlayerFSMBase
{
    protected Arrive m_arrive;

    protected override void Awake()
    {
        base.Awake();

        //Cache the Arrive Behaviour
        m_arrive = m_player.GetComponent<Arrive>();
    }

    public override void OnEnter()
    {
        m_player.CurrentSteer = m_arrive;
    }
}