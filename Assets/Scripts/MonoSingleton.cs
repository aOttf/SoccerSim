using System;
using System.Collections.Generic;
using System.Linq;

using Unity;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T m_instance;

    /// <summary>
    /// Singleton Instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (!m_instance)
                m_instance = FindObjectOfType<T>();

            if (!m_instance)
                Debug.LogError("Need At Least One Game Manager Instance Present.");

            m_instance.Init();
            return m_instance;
        }
    }

    /// <summary>
    /// Initialize Singleton Instance
    /// </summary>
    protected virtual void Init()
    { }

    protected virtual void Awake()
    {
        //If the instance hasn't been intialized, initialize it
        if (!m_instance)
        {
            Init();
            m_instance = (T)this;
        }
    }
}