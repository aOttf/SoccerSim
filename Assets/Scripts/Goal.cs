using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private GameManager m_manager;

    [Tooltip("The color of team it belongs to")]
    [SerializeField] private TeamColor m_teamColor;

    #region Caches

    private TeamAgent m_opponentTeam;

    #endregion Caches

    private void Awake()
    {
        //Cache Parameters
        m_manager = GameManager.Instance;
        m_opponentTeam = m_manager.OpponentTeam(m_teamColor);
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #region Callbacks

    private void OnCollisionEnter(Collision collision)
    {
        //Hit by the soccer
        if (collision.gameObject == m_manager.soccer.gameObject)
            m_opponentTeam.OnScore();
    }

    #endregion Callbacks
}