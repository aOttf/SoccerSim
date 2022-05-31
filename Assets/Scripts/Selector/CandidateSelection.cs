using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public abstract class CandidateSelector<Candidate> : MonoBehaviour
{
    protected GameManager m_manager;

    protected virtual void Awake()
    {
        m_manager = GameManager.Instance;
    }

    /// <summary>
    /// returns candidate with least utility
    /// </summary>
    /// <param name="cans"></param>
    /// <returns></returns>
    public Candidate BestCandidate(params Candidate[] cans)
    {
        float min = float.MaxValue;
        Candidate best = default;
        foreach (var can in cans)
        {
            float curUtil = Utility(can);
            if (curUtil < min)
            {
                min = curUtil;
                best = can;
            }
        }

        return best;
    }

    public int BestCandidateIndex(params Candidate[] cans)
    {
        float min = float.MaxValue;
        int bestIdx = 1;
        for (int i = 0; i < cans.Length; i++)
        {
            Candidate can = cans[i];
            float curUtil = Utility(can);
            print("Index: " + i + "Util: " + curUtil);
            if (curUtil < min)
            {
                min = curUtil;
                bestIdx = i;
            }
        }

        return bestIdx;
    }

    /// <summary>
    /// Utility Function
    /// </summary>
    /// <param name="can"></param>
    /// <returns></returns>
    public abstract float Utility(Candidate can);
}