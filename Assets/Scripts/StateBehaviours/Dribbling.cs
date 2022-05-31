using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SteeringSystem;
using DecisionMaking.StateMachine;

[RequireComponent(typeof(BestPosition), typeof(MatchDirection))]
public class Dribbling : PlayerStateBehaviour
{
    public static readonly Vector3[] eightwayDirections
        = {
        new Vector3(1, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 0),
        new Vector3(-1, 0, 1),
        new Vector3(0, 0, 1)
};

    [SerializeField] protected float m_evaluationDelta = .02f;
    protected float m_evaluationOffset;

    #region Caches

    protected BestPosition m_positionSelector;
    protected MatchDirection m_drive;

    protected Vector3[] m_eightwayPositions = new Vector3[8];
    protected float m_radius;

    protected Vector3 m_targetPosition;

    #endregion Caches

    public bool drawGizmos;

    protected override void Awake()
    {
        base.Awake();
        m_evaluationOffset = m_evaluationDelta / 4;
        m_positionSelector = GetComponent<BestPosition>();
        m_drive = GetComponent<MatchDirection>();

        m_radius = m_player.radius;
    }

    protected override void Enter()
    {
        //Start Coroutine
        StartCoroutine(nameof(EvaluateDirection));
        //Set Steering
        m_player.CurrentSteer = m_drive;
        m_player.StartSteering();
    }

    protected override void Execute()
    {
        //TODO: Pass Ball Logic
    }

    protected override void Exit()
    {
        print("Exit");
        //Stop Coroutine
        StopCoroutine(nameof(EvaluateDirection));
    }

    protected IEnumerator EvaluateDirection()
    {
        while (true)
        {
            //Update 8 way neighbouring positions
            for (int i = 0; i < 8; i++)
                m_eightwayPositions[i] = m_player.position + eightwayDirections[i] * m_radius;

            List<Vector3> dirs = new List<Vector3>();
            Vector3 curDir = m_player.forward;
            for (int i = 0; i < 12; i++)
            {
                curDir = Quaternion.Euler(0, 30, 0) * curDir;
                dirs.Add(curDir * m_radius + m_player.position);
            }

            m_drive.desiredDirection = m_positionSelector.BestCandidate(dirs.ToArray()) - m_player.position;

            ////Evaluate Utitlity of positions
            //int idx = m_positionSelector.BestCandidateIndex(m_eightwayPositions);
            //print(idx);
            //m_drive.desiredDirection = eightwayDirections[idx];

            yield return new WaitForSeconds(m_evaluationDelta + Random.Range(-m_evaluationOffset, m_evaluationOffset));
        }
    }

    protected void OnDrawGizmosSelected()
    {
        if (drawGizmos && Application.isPlaying)
        {
            foreach (var pos in m_eightwayPositions)
            {
                Gizmos.DrawSphere(pos, m_positionSelector.Utility(pos));
            }
        }
    }
}