using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DecisionMaking.StateMachine;
using SteeringSystem;

public class Watching : PlayerFSMBase
{
    protected Vector3 m_watchingPosition;

    protected bool hasArrived;

    #region Caches

    protected Ball m_soccer;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
    }

    public override void OnEnter()
    {
        //Set player's current steer to null
        m_player.CurrentSteer = null;
    }

    public override void OnUpdate()
    {
        //PLACEHOLDER Behavior Logic

        //if (!hasArrived)
        //{
        //    hasArrived = (m_player.position - m_watchingPosition).sqrMagnitude < float.Epsilon;
        //    if (hasArrived)
        //    {
        //        //Start Looking at the soccer
        //        m_player.SetLookAt(true, m_soccer.transform);
        //    }
        //}
    }

    public override void OnExit()
    {
        // m_player.SetLookAt(false);
    }
}