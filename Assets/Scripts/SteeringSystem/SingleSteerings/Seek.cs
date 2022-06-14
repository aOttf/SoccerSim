using UnityEngine;

using static SteeringSystem.SteeringUtilities;

namespace SteeringSystem
{
    public enum MatchMode
    {
        MatchPosition, MatchTransform
    }

    public class Seek : SteeringBehaviour
    {
        [Space(50)]
        [Tooltip("The Target transform seeking to; set this will hide target position")]
        public Transform target;
        public Vector3 targetPos;

        public MatchMode seekMode;

        protected override Vector3 GetSteering() =>
            m_entity.maxLinearSpeed * ((seekMode == MatchMode.MatchTransform ? target.position : targetPos) - m_entity.position).normalized;
    }
}