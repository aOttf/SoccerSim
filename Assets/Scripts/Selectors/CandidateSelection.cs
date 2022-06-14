using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Represents the best candidate and associated utility value
/// </summary>
/// <typeparam name="Candidate"></typeparam>
public struct BestCandidateData<Candidate>
{
    public Candidate can;
    public float utility;

    public BestCandidateData(Candidate pCan, float pUtil)
    {
        can = pCan;
        utility = pUtil;
    }
}

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
    public BestCandidateData<Candidate> BestCandidate(params Candidate[] cans)
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

        return new BestCandidateData<Candidate>(best, min);
    }

    /// <summary>
    /// returns candidate with least utility
    /// </summary>
    /// <param name="cans"></param>
    /// <returns></returns>
    public BestCandidateData<Candidate> BestCandidate(List<Candidate> cans)
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

        return new BestCandidateData<Candidate>(best, min);
    }

    /// <summary>
    /// find candidate with least utility, and stores it in the provided buffer
    /// </summary>
    /// <param name="cans"></param>
    /// <returns></returns>
    public void BestCandidateNonAlloc(out BestCandidateData<Candidate> data, params Candidate[] cans)
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

        data.can = best;
        data.utility = min;
    }

    /// <summary>
    /// find candidate with least utility, and stores it in the provided buffer
    /// </summary>
    /// <param name="cans"></param>
    /// <returns></returns>
    public void BestCandidateNonAlloc(out BestCandidateData<Candidate> data, List<Candidate> cans)
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

        data.can = best;
        data.utility = min;
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