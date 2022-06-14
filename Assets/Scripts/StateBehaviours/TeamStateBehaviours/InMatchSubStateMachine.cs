using System;
using System.Collections.Generic;
using System.Linq;

using Unity;
using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

public class InMatchSubStateMachine : TeamFSM
{
    protected override void OnFSMEnter()
    {
        base.OnFSMEnter();

        //Callback
        m_team.OnMatchEnter();
    }

    protected override void OnFSMExit()
    {
        //Callback
        m_team.OnMatchExit();

        base.OnFSMExit();
    }
}