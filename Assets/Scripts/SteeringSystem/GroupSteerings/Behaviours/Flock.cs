using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringSystem
{
    //public class Flock : GroupSteeringBehaviour
    //{
    //    public float seperationWeight;
    //    public float cohesionWeight;
    //    public float alignmentWeight;

    //    //Caches
    //    protected Vector3 m_acceleration;
    //    protected int m_neighbourCount;

    //    protected List<Vector3> m_neighbourStrengths;
    //    protected Vector3 m_centerofStrength;   //Seperation

    //    protected Vector3 m_centerOfMess;   //Cohesion
    //    protected Vector3 m_centerOfHead;   //Alignment

    //    protected override void Awake()
    //    {
    //        //Init
    //        base.Awake();
    //    }

    //    protected override SteeringOutput GetSteering()
    //    {
    //        //Find all neightbours
    //        // FindNeighbours();

    //        //Seperations

    //        //Inverse Squart Root
    //        m_neighbourStrengths = m_neighbours.ConvertAll(neighbour => (transform.position - neighbour.position) / (transform.position - neighbour.position).sqrMagnitude);
    //        m_centerofStrength = new Vector3(m_neighbourStrengths.Average(strength => strength.x), m_neighbourStrengths.Average(strength => strength.y), m_neighbourStrengths.Average(strength => strength.z));
    //        m_acceleration = m_centerofStrength * seperationWeight;

    //        //Cohesion
    //        m_centerOfMess = new Vector3(m_neighbours.Average(neighbour => neighbour.position.x), m_neighbours.Average(neighbour => neighbour.position.y), m_neighbours.Average(neighbour => neighbour.position.z));
    //        m_acceleration += cohesionWeight * (transform.position - m_centerOfMess).normalized * m_maxLinearAcceleration;

    //        //Alignment
    //        m_centerOfHead = new Vector3(m_neighbours.Average(neighbour => neighbour.forward.x), m_neighbours.Average(neighbour => neighbour.forward.y), m_neighbours.Average(neighbour => neighbour.forward.z));
    //        m_acceleration += alignmentWeight * (m_centerOfHead - transform.forward).normalized * m_maxLinearAcceleration;

    //        return SteeringOutput.LinearSteering(m_acceleration.normalized * m_maxLinearAcceleration);
    //    }

    //    protected override void OnDrawGizmosSelected()
    //    {
    //        base.OnDrawGizmosSelected();
    //    }
    //}
}