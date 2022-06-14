using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Caches

    private GameManager m_manager;
    private TeamAgent m_controllingTeam;
    private PlayerAgent m_controllingPlayer;

    #endregion Caches

    [SerializeField] private TeamColor m_humanControlledTeamColor;

    public TeamAgent ControllingTeam => m_controllingTeam;
    public PlayerAgent ControllingPlayer => m_controllingPlayer;

    private void Awake()
    {
        m_manager = GameManager.Instance;
        m_controllingTeam = m_humanControlledTeamColor == TeamColor.Blue ? m_manager.blueTeam : m_manager.redTeam;
    }

    // Start is called before the first frame update
    private void Start()
    {
        OnSwitchPlayer(m_controllingTeam.NearestTeammateToBall());
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
        bool spacePressed;
        //If the controlling player is the dribbler, press space kicks the ball
        if ((spacePressed = Input.GetKeyDown(KeyCode.Space)) && m_controllingPlayer.IsDribbler)
            m_controllingPlayer.OnKick();

        //Otherwise, press space switches to the nearest player to the ball
        else if (spacePressed)
        {
            PlayerAgent nextPlayer = m_controllingTeam.NearestTeammateToBall();
            OnSwitchPlayer(nextPlayer);
        }
    }

    /// <summary>
    /// Callback when controller switches the current controlled player
    /// </summary>
    /// <param name="nextPlayer"></param>
    public void OnSwitchPlayer(PlayerAgent nextPlayer)
    {
        if (m_controllingPlayer)
            m_controllingPlayer.OnControlExit();

        m_controllingPlayer = nextPlayer;
        m_controllingPlayer.OnControlEnter(false);
    }
}