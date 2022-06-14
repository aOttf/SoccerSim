using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking
{
    /// <summary>
    /// StateBehaviour is the base class from which every State Script attached to a Decision Maker derived.
    /// </summary>
    [RequireComponent(typeof(BlackboardManager))]
    public abstract class StateBehaviour : MonoBehaviour
    {
        protected BlackboardManager m_blackboardManager;
        protected bool m_isActive;

        protected virtual void Awake()
        {
            m_blackboardManager = GetComponent<BlackboardManager>();
        }

        /// <summary>
        /// Is the State Execution completed
        /// </summary>
        /// <returns></returns>
        public virtual bool IsComplete => false;

        /// <summary>
        /// On Enter the Sa
        /// </summary>
        public virtual void OnEnter()
        {
            m_isActive = true;
        }

        /// <summary>
        /// On Update the State
        /// </summary>
        public virtual void OnUpdate()
        { }

        /// <summary>
        /// On Exit the State
        /// </summary>
        public virtual void OnExit()
        { m_isActive = false; }

        public T GetStateBehaviour<T>() where T : StateBehaviour => GetComponent<T>();

        public T[] GetStateBehaviours<T>() where T : StateBehaviour => GetComponents<T>();
    }
}