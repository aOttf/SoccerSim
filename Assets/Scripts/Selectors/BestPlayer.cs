using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Linq;

[RequireComponent(typeof(PlayerAgent))]
public class BestPlayer : CandidateSelector<PlayerAgent>
{
    private PlayerAgent m_sourcePlayer;

    #region Utility Function Parameters

    public float interposeDistance = 2f;

    //public float frontCosine = .8f;
    public float interposeAngle = .8f;  //The angle of the testing cone is twice of it

    public int distanceWeight;
    //public int interposeWeight;

    #endregion Utility Function Parameters

    public bool drawGizmos;
    public bool drawCone;

    protected override void Awake()
    {
        base.Awake();
        m_sourcePlayer = GetComponent<PlayerAgent>();
    }

    /// <summary>
    /// 1. The Candidate is in front of the source
    /// 2. The distance to pass
    /// 3. Any Opponents along the pass way
    /// </summary>
    /// <param name="can"></param>
    /// <returns></returns>
    public override float Utility(PlayerAgent can)
    {
        //if the candidate is the source player itself, returns least utility(max value)
        if (can == m_sourcePlayer)
            return float.MaxValue;

        if (m_sourcePlayer.CanKick(can.position, interposeDistance, interposeAngle))
        {
            //Find the closest approchable candidate
            return distanceWeight * Vector3.Distance(can.position, m_sourcePlayer.position);
        }
        else
            return float.MaxValue;
    }

    protected void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (drawCone)
            {
                Handles.DrawWireArc(m_sourcePlayer.position, m_sourcePlayer.up, m_sourcePlayer.forward, interposeAngle, interposeDistance);
            }
        }
    }
}