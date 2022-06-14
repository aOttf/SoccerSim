using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking.StateMachine
{
    public enum UpdateMode
    {
        Update, FixedUpdate, TimeStep
    }

    [Serializable]
    public class Transition : IComponentReveiver<BlackboardManager>, IComponentReveiver<FSMBase>
    {
        #region Inspector GUI

        public List<IntBlackboardCondition> intConditions;
        public List<FloatBlackboardCondition> floatConditions;
        public List<BoolBlackboardCondition> boolConditions;
        public List<TriggerBlackboardCondition> triggerConditions;

        #endregion Inspector GUI

        internal BlackboardManager m_blackboardManager;

        internal FSMBase m_fromState;
        [SerializeField] internal FSMBase m_toState;

        [Tooltip("The transition is evaluated valid only after the from state behaviour is completed.")]
        [SerializeField] internal bool m_afterCompletion;

        internal void LoadBlackboardConditions()
        {
            m_andCondition = new AndCondition();

            intConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));
            floatConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));
            boolConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));
            triggerConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));

            m_andCondition.conditions.AddRange(intConditions);
            m_andCondition.conditions.AddRange(floatConditions);
            m_andCondition.conditions.AddRange(boolConditions);
            m_andCondition.conditions.AddRange(triggerConditions);
        }

        internal AndCondition m_andCondition;

        public bool IsValid()
        {
            //Debug.Log("From " + FromState + "To " + ToState);
            return (!m_afterCompletion || m_fromState.IsComplete) && m_andCondition.IsValid();
        }

        public FSMBase FromState => m_fromState;
        public FSMBase ToState => m_toState;

        public BlackboardManager AddComponent(BlackboardManager component)
        {
            m_blackboardManager = component;
            return m_blackboardManager;
        }

        public FSMBase AddComponent(FSMBase component)
        {
            return m_fromState = component;
        }
    }

    /// <summary>
    /// FSMBase can be attached to a Finite State Machine. It's the base class every StateMachineBehaviour attached to a State Machine derived from.
    /// </summary>
    [RequireComponent(typeof(RootFSM))]
    public abstract class FSMBase : StateBehaviour
    {
        [SerializeField] protected List<Transition> m_transitions;

        public FSMBase TriggeredState => m_transitions.FirstOrDefault(transition => transition.IsValid())?.ToState;

        protected override void Awake()
        {
            base.Awake();

            foreach (var trans in m_transitions)
            {
                trans.AddComponent(m_blackboardManager);
                trans.AddComponent(this);
                trans.LoadBlackboardConditions();
            }
        }
    }

    [RequireComponent(typeof(RootFSM))]
    public abstract class FSM : FSMBase
    {
        #region Inspector GUI

        [Tooltip("The Current State the state machine is at")]
        [SerializeField] protected FSMBase m_initialState;

        [SerializeField] protected bool m_resetCurrentState;

        #endregion Inspector GUI

        #region Caches

        protected FSMBase m_trigState;
        protected FSMBase m_curState;

        #endregion Caches

        #region Properties

        public FSMBase InitialState { get => m_initialState; set => m_initialState = value; }
        public FSMBase CurrentState { get => m_curState; set => m_curState = value; }

        #endregion Properties

        public override string ToString()
        {
            return CurrentState.ToString();
        }

        protected override void Awake()
        {
            base.Awake();

            //SetUp Current State
            m_curState = m_initialState;
        }

        // Start is called before the first frame update
        protected void Start()
        {
        }

        public override sealed void OnEnter()
        {
            OnFSMEnter();

            if (m_resetCurrentState)
                CurrentState = InitialState;
            CurrentState.OnEnter();
        }

        public override sealed void OnUpdate()
        {
            //   print("Current State: " + CurrentState);
            OnFSMUpdate();

            if (m_trigState = CurrentState.TriggeredState)
            {
                CurrentState.OnExit();
                CurrentState = m_trigState;
                CurrentState.OnEnter();
            }
            else
                CurrentState.OnUpdate();
        }

        public override sealed void OnExit()
        {
            CurrentState.OnExit();

            OnFSMExit();
        }

        protected virtual void OnFSMEnter()
        { }

        protected virtual void OnFSMUpdate()
        { }

        protected virtual void OnFSMExit()
        { }
    }
}