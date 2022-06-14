using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity;
using UnityEngine;
using UnityEditor;

namespace DecisionMaking.StateMachine
{
    public class RootFSM : FSM
    {
        [Tooltip("Is the Root FSM turned on")]
        public bool isTurnedOn = false;

        [Header("Gizmos")]
        public bool showState;

        //A flag to indicate that RootFSM has been manually added to RootFSMManager
        [HideInInspector] public bool isAdded;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Turn on the Root FSM and enter it. Root FSM already turned on won't re-enter.
        /// </summary>
        public void TurnOn()
        {
            //Dont enter twice
            if (!isTurnedOn)
            {
                isTurnedOn = true;
                OnEnter();
            }
        }

        /// <summary>
        /// Turn off the Root FSM and exit it. Root FSM already turned off won't re-exit.
        /// </summary>
        public void TurnOff()
        {
            //Dont exit twice
            if (!isTurnedOn)
            {
                isTurnedOn = false;
                OnExit();
            }
        }

        private void OnDrawGizmos()
        {
            if (showState && Application.isPlaying)
            {
                Handles.Label(transform.position, ToString());
            }
        }
    }
}