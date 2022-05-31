using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerAgent m_controllingPlayer;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //Get Desired Velocity
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 desired = Vector3.right * h + Vector3.forward * v;
        m_controllingPlayer.desiredVelocity = desired.normalized * m_controllingPlayer.maxLinearSpeed;

        //Shoot&Pass
        if (Input.GetKeyDown(KeyCode.Space) && m_controllingPlayer.HasBall)
            m_controllingPlayer.Kick();
    }
}