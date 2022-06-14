using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

[RequireComponent(typeof(PlayerAgent))]
public class BestPosition : CandidateSelector<Vector3>
{
    private PlayerAgent m_player;
    private Vector3 m_blueGoal;
    private Vector3 m_redGoal;

    #region Utility Function Parameters

    public int awayFromHomeWeight;
    public int avoidOpponentWeight;
    public int closeToGoalWeight;

    #endregion Utility Function Parameters

    protected override void Awake()
    {
        base.Awake();
        m_player = GetComponent<PlayerAgent>();

        m_blueGoal = new Vector3(0, 0, m_manager.halfPitchHeight);
        m_redGoal = new Vector3(m_manager.pitchWidth, 0, m_manager.halfPitchHeight);
    }

    /// <summary>
    ///1. The distance to the opponent team's goal
    ///2. The distance to the team's goal
    ///3. The distance to opponents
    /// </summary>
    /// <param name="can"></param>
    /// <returns></returns>
    public override float Utility(Vector3 can)
    {
        if (m_manager.OutOfPitch(can))
            return float.MaxValue;

        List<PlayerAgent> opponents = m_manager.Opponents(m_player.TeamColor);

        Vector3 goalCenter = m_player.TeamColor == TeamColor.Blue ? m_blueGoal : m_redGoal;
        Vector3 oppoGoalCenter = m_player.TeamColor == TeamColor.Blue ? m_redGoal : m_blueGoal;

        float goalsDiff = (goalCenter - oppoGoalCenter).sqrMagnitude;

        //away from home as far as possible
        float home = 1 / ((goalCenter - can).sqrMagnitude + .001f);
        //away from opponents  as far as possible
        float avoidance = opponents.Sum(oppo => 1 / ((oppo.position - can).sqrMagnitude + .001f));
        //print(avoidance);
        //as close as possible to the opponents goal
        float target = (can - oppoGoalCenter).sqrMagnitude / goalsDiff;

        return awayFromHomeWeight * home + avoidOpponentWeight * avoidance + closeToGoalWeight * target;
        /*
        float inverseDist = 1 / (can - goalCenter).magnitude;
        float avoidance = 1 / m_manager.Opponents(m_team.TeamColor).Sum(oponent => (oponent.position - can).magnitude);
        float centerPitch = (can.x - oppoGoalCenter.x) * (can.x - oppoGoalCenter.x);
        float opponentGoal = Mathf.Abs(can.z - oppoGoalCenter.z);
        return (int)(inverseDistanceWeight * inverseDist)
            + avoidOpponentWeight * (int)avoidance
            + opponentGoalWeight * (int)opponentGoal
            + pitchCenterWeight * (int)centerPitch;
    */
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            //Draw Red Goal
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_redGoal, 10f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_blueGoal, 10f);
        }
    }
}