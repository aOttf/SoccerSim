using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using static Blackboard;

/// <summary>
/// Blackboard Managger.
/// Blackboard stores the global game state, where entities can read and write back
/// </summary>
public class BlackboardManager : MonoBehaviour
{
    [SerializeField] private Blackboard m_blackboardInfo;

    #region Dictionaries Caches

    private Dictionary<string, int> m_intParameters;
    private Dictionary<string, float> m_floatParameters;
    private Dictionary<string, bool> m_boolParameters;
    private Dictionary<string, Trigger> m_triggerParameters;

    #endregion Dictionaries Caches

    private void Awake()
    {
        Init();
    }

    #region Parameters Get&Set

    private void Init()
    {
        //   DontDestroyOnLoad(gameObject);

        m_intParameters = new Dictionary<string, int>();
        m_floatParameters = new Dictionary<string, float>();
        m_boolParameters = new Dictionary<string, bool>();
        m_triggerParameters = new Dictionary<string, Trigger>();
        m_blackboardInfo.LoadIntegers(m_intParameters);
        m_blackboardInfo.LoadFloats(m_floatParameters);
        m_blackboardInfo.LoadBools(m_boolParameters);
        m_blackboardInfo.LoadTriggers(m_triggerParameters);
    }

    public float GetFloat(string pName) => m_floatParameters[pName];

    public int GetInteger(string pName) => m_intParameters[pName];

    public bool GetBool(string pName) => m_boolParameters[pName];

    public void SetFloat(string pName, float value) => m_floatParameters[pName] = value;

    public void SetInteger(string pName, int value) => m_intParameters[pName] = value;

    public void SetBool(string pName, bool value) => m_boolParameters[pName] = value;

    public void SetTrigger(string pName) => (m_triggerParameters[pName]).Set();

    public void ResetTrigger(string pName) => (m_triggerParameters[pName]).Reset();

    public bool GetTrigger(string pName) => (m_triggerParameters[pName]).isTriggered;

    #endregion Parameters Get&Set
}