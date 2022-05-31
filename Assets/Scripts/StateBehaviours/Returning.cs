using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

public class Returning : PlayerStateBehaviour
{
    protected Arrive m_arrive;

    protected override void Awake()
    {
        base.Awake();

        //Cache the Arrive Behaviour
        m_arrive = m_player.GetComponent<Arrive>();
    }

    protected override void Enter()
    {
        m_player.CurrentSteer = m_arrive;
    }
}