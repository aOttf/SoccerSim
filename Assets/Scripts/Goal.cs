using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private GameManager m_manager;

    [Tooltip("Which team it belongs to")]
    [SerializeField] private TeamAgent m_ownedTeam;

    #region Caches

    private float m_shootOffset;
    private float m_goalHeight;

    #endregion Caches

    private void Awake()
    {
        m_manager = GameManager.Instance;

        //Cache Parameters
        m_shootOffset = m_manager.scoreRange;
        m_goalHeight = m_manager.goalHeight;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public bool OnScore(Ball soccer) => Mathf.Abs(transform.position.x - soccer.transform.position.x) < m_shootOffset;
}