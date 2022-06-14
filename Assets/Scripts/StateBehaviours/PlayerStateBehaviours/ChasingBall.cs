using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;

[RequireComponent(typeof(Persue))]
public class ChasingBall : PlayerFSMBase
{
    #region Caches

    protected Persue m_drive;
    protected Ball m_soccer;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();
        m_drive = GetComponent<Persue>();
        m_soccer = m_manager.soccer;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        //Set Steering
        m_player.CurrentSteer = m_drive;
        m_drive.target = m_soccer;
        //Set Target
        //UpdateChasingTarget();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        //Update Target
        UpdateChasingTarget();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void UpdateChasingTarget()
   => m_drive.target = m_soccer.HasOwner ? m_soccer.owner : m_soccer;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}