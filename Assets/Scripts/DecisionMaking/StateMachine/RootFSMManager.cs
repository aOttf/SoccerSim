using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity;
using UnityEngine;

namespace DecisionMaking.StateMachine
{
    public class RootFSMManager : MonoSingleton<RootFSMManager>
    {
        [Header("Execution Params")]
        public UpdateMode mode = UpdateMode.Update;

        [Tooltip("how often the state machine executes in seconds")]
        [SerializeField] protected float m_executionTimeStep = .02f;

        [Header("Root FSMs")]
        [Tooltip("Root FSMs executed before default time, in order.")]
        [SerializeField] protected List<RootFSM> m_beforeDefaultTimeExecution;

        [Tooltip("Root FSMs executed after default time, in order.")]
        [SerializeField] protected List<RootFSM> m_afterDefaultTimeExecution;

        protected List<RootFSM> m_inDefaultTimeExecution = new List<RootFSM>();

        protected override void Init()
        {
            base.Init();

            m_beforeDefaultTimeExecution.ForEach(rootFSM => rootFSM.isAdded = true);
            m_afterDefaultTimeExecution.ForEach(rootFSM => rootFSM.isAdded = true);

            //Find all unadded Root FSMs into default
            m_inDefaultTimeExecution.AddRange(FindObjectsOfType<RootFSM>().ToList().FindAll(rootFSM => !rootFSM.isAdded));
            m_inDefaultTimeExecution.ForEach(rootFSM => rootFSM.isAdded = true);
        }

        protected virtual void Start()
        {
            //Enter Before Default time Root Fsms in order
            EnterRootFSMs(m_beforeDefaultTimeExecution);
            //Enter Default time Root Fsms
            EnterRootFSMs(m_inDefaultTimeExecution);
            //Enter After Default time Root Fsms in order
            EnterRootFSMs(m_afterDefaultTimeExecution);

            StartCoroutine(nameof(RootFSMExecution));
        }

        /// <summary>
        /// Main Update Coroutine of Root FSMs
        /// </summary>
        /// <returns></returns>
        protected IEnumerator RootFSMExecution()
        {
            while (true)
            {
                UpdateRootFSMs(m_beforeDefaultTimeExecution);
                UpdateRootFSMs(m_inDefaultTimeExecution);
                UpdateRootFSMs(m_afterDefaultTimeExecution);

                switch (mode)
                {
                    case UpdateMode.Update:
                        yield return null;
                        break;

                    case UpdateMode.FixedUpdate:
                        yield return new WaitForFixedUpdate();
                        break;

                    case UpdateMode.TimeStep:
                        yield return new WaitForSeconds(m_executionTimeStep);
                        break;
                }
            }
        }

        /// <summary>
        ///  Turn On and Enter all turned off Root FSMs in order. Root FSMs already turned on won't re-enter.
        /// </summary>
        public void TurnOnAllRootFSMs()
        {
            TurnOnRootFSMs(m_beforeDefaultTimeExecution);
            TurnOnRootFSMs(m_inDefaultTimeExecution);
            TurnOnRootFSMs(m_afterDefaultTimeExecution);
        }

        /// <summary>
        /// Turn Off and Exit all turned on Root FSMs in order. Root FSMs already turned off won't re-exit.
        /// </summary>
        public void TurnOffAllRootFSMs()
        {
            TurnOffRootFSMs(m_beforeDefaultTimeExecution);
            TurnOffRootFSMs(m_inDefaultTimeExecution);
            TurnOffRootFSMs(m_afterDefaultTimeExecution);
        }

        /// <summary>
        /// Enter rootFSMs in order, if turned on
        /// </summary>
        /// <param name="fsms"></param>
        protected void EnterRootFSMs(List<RootFSM> fsms)
        {
            foreach (var fsm in fsms)
                if (fsm.isTurnedOn)
                    fsm.OnEnter();
        }

        /// <summary>
        /// Update rootFSMs in order, if turned on
        /// </summary>
        /// <param name="fsms"></param>
        protected void UpdateRootFSMs(List<RootFSM> fsms)
        {
            foreach (var fsm in fsms)
                if (fsm.isTurnedOn)
                    fsm.OnUpdate();
        }

        /// <summary>
        /// Update rootFSMs in order, if turned on
        /// </summary>
        /// <param name="fsms"></param>
        protected void ExitRootFSMs(List<RootFSM> fsms)
        {
            foreach (var fsm in fsms)
                if (fsm.isTurnedOn)
                    fsm.OnExit();
        }

        /// <summary>
        /// Turn on Root FSMs. Root FSMs already turned on won't re-enter.
        /// </summary>
        /// <param name="fsms"></param>
        protected void TurnOnRootFSMs(List<RootFSM> fsms)
        => fsms.ForEach(fsm => fsm.TurnOn());

        /// <summary>
        /// Turn off Root FSMs. Root FSMs already turned off won't re-exit
        /// </summary>
        /// <param name="fsms"></param>
        protected void TurnOffRootFSMs(List<RootFSM> fsms)
        => fsms.ForEach(fsm => fsm.TurnOff());
    }
}