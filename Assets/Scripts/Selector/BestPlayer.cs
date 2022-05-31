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

    public float maxDistance;
    public float frontCosine = .8f;
    public float interposeCosine = .8f;  //The angle of the testing cone is twice of it

    public int distanceWeight;
    public int interposeWeight;

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
        Vector3 toCan = can.transform.position - m_sourcePlayer.transform.position;
        Vector3 norCan = toCan.normalized;
        float sqrDistCan = toCan.sqrMagnitude;

        //If the candidate is not in front of the player or too far away from player
        if (Vector3.Dot(norCan, m_sourcePlayer.forward) < frontCosine || sqrDistCan > maxDistance)
            return float.MaxValue;

        //Get opponents
        List<PlayerAgent> opponents = m_manager.Opponents(m_sourcePlayer.TeamColor);

        foreach (var oppo in opponents)
        {
            Vector3 toOppo = can.position - m_sourcePlayer.position;
            Vector3 norOppo = toOppo.normalized;
            float sqrDistOppo = (oppo.position - m_sourcePlayer.position).sqrMagnitude;
            //If an opponent is inside the cone,of which the middle-line is source to candidate
            if (sqrDistOppo < sqrDistCan && Vector3.Dot(norOppo, norCan) > interposeCosine)
            {
                return float.MaxValue;
            }
        }

        return distanceWeight * Mathf.Sqrt(sqrDistCan);
    }

    protected void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (drawCone)
            {
                Handles.DrawWireArc(m_sourcePlayer.position, m_sourcePlayer.up, m_sourcePlayer.forward, Mathf.Acos(frontCosine), maxDistance);
            }
        }
    }
}