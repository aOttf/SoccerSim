using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

[RequireComponent(typeof(Seek), typeof(Persue))]
public class Defensing : PlayerFSMBase
{
    [Header("Interposing Params")]
    [SerializeField] protected float m_interposeDistance;

    [Header("Gizmos")]
    public bool drawTargetPosition;

    #region Caches

    protected Seek m_sdrive;
    protected Persue m_pdrive;
    protected Ball m_soccer;

    protected PlayerAgent m_target;
    protected Vector3 m_interposeDirection;
    protected Vector3 m_interposeOffset;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();

        m_sdrive = GetComponent<Seek>();
        m_sdrive.seekMode = MatchMode.MatchPosition;
        m_pdrive = GetComponent<Persue>();

        m_interposeDirection = (m_player.TeamColor == TeamColor.Blue ? -1 : 1) * Vector3.right;
        m_soccer = m_manager.soccer;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine(nameof(GenerateInterposeOffset));
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        m_target = m_soccer.owner;

        if (m_target)
        {
            m_player.CurrentSteer = m_sdrive;
            m_sdrive.targetPos = m_manager.ClampPosition(m_target.position + m_interposeOffset);
        }
        else
        {
            m_pdrive.target = m_soccer;
            m_player.CurrentSteer = m_pdrive;
        }
    }

    public override void OnExit()
    {
        StopCoroutine(nameof(GenerateInterposeOffset));
        base.OnExit();
    }

    protected IEnumerator GenerateInterposeOffset()
    {
        while (true)
        {
            m_interposeOffset = Quaternion.Euler(0, Random.Range(-60f, 60f), 0) * m_interposeDirection * m_interposeDistance;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (drawTargetPosition && m_isActive)
            {
                Gizmos.DrawLine(m_player.position, m_target.position + m_interposeDirection * m_interposeDistance);
            }
        }
    }
}