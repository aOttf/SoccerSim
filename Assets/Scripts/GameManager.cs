using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    #region Inspector Serializations

    [Header("Environment Parameters")]
    [Tooltip("Height of the level")]
    public float levelHeight;

    [Tooltip("Width of the level")]
    public float levelWidth;

    [Tooltip("Height of the Pitch")]
    public float pitchHeight;

    [Tooltip("Width of the pitch")]
    public float pitchWidth;

    [Tooltip("Height of the Goal")]
    public float goalHeight;

    [Tooltip("Depth(Width) of the Goal")]
    public float goalDepth;

    [Tooltip("The Offset to the goal in which the ball appears will be regarded as a successful shoot")]
    public float scoreRange;

    [Header("Soccer Parameters")]
    public float kickStrength;
    public float dragStrength;
    public float dribbleDistance;

    [Header("Player Parameters")]
    [Tooltip("How long should a player dribbles the soccer at least")]
    public float holdoffTime;

    [Header("AI Parameters")]
    [Tooltip("The minimum distance needed from the player to the ball for player in waiting state.")]
    public float playerActiveDistance;

    #endregion Inspector Serializations

    #region Caches

    [HideInInspector] public float halfLevelHeight;
    [HideInInspector] public float halfLevelWidth;
    [HideInInspector] public float halfPitchHeight;
    [HideInInspector] public float halfPitchWidth;
    [HideInInspector] public float halfGoalHeight;
    [HideInInspector] public float halfGoalDepth;

    private Rect m_levelArea;
    private Rect m_pitchArea;
    private Rect m_goalAreaRed;
    private Rect m_goalAreaBlue;

    #endregion Caches

    #region Entities

    public Ball soccer;
    public TeamAgent redTeam;
    public TeamAgent blueTeam;

    public List<PlayerAgent>[] players = new List<PlayerAgent>[2];

    public PlayerController controller;

    #endregion Entities

    public Rect TeamGoal(TeamColor color) => color == TeamColor.Red ? m_goalAreaRed : m_goalAreaBlue;

    public Rect OpponentGoal(TeamColor color) => color == TeamColor.Red ? m_goalAreaBlue : m_goalAreaRed;

    public List<PlayerAgent> Teammates(TeamColor color) => players[(int)color];

    public List<PlayerAgent> Opponents(TeamColor color) => players[((int)color + 1) % 2];

    public TeamAgent Team(TeamColor color) => color == TeamColor.Red ? redTeam : blueTeam;

    public TeamAgent OpponentTeam(TeamColor color) => color == TeamColor.Red ? blueTeam : redTeam;

    protected override void Init()
    {
        //initialize Cached Parameters
        halfLevelHeight = levelHeight / 2;
        halfLevelWidth = levelWidth / 2;
        halfPitchHeight = pitchHeight / 2;
        halfPitchWidth = pitchWidth / 2;
        halfGoalHeight = goalHeight / 2;
        halfGoalDepth = goalDepth / 2;

        //Initialize Rectangle Areas
        //Take z-axis as y-axis
        m_levelArea = new Rect(0, 0, levelWidth, levelHeight);
        m_pitchArea = new Rect(0, 0, pitchWidth, pitchHeight);
        m_goalAreaBlue = new Rect(halfLevelWidth - halfPitchWidth - goalDepth, halfLevelHeight - halfGoalHeight, goalDepth, goalHeight);
        //Inverse Rectangle
        m_goalAreaRed = new Rect(m_goalAreaBlue.x + goalDepth + pitchWidth, m_goalAreaBlue.y, -goalDepth, goalHeight);

        //Initialize Entities
        soccer = FindObjectOfType<Ball>();
        foreach (var t in FindObjectsOfType<TeamAgent>())
            if (t.TeamColor == TeamColor.Blue)
                blueTeam = t;
            else
                redTeam = t;

        players[(int)TeamColor.Blue] = blueTeam.teammates;
        players[(int)TeamColor.Red] = redTeam.teammates;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        void DrawRect(Rect rect)
        {
            Vector3 pivot = new Vector3(rect.x, 0, rect.y);
            float width = rect.width;
            float height = rect.height;
            Gizmos.DrawLine(pivot, pivot + Vector3.right * width);
            Gizmos.DrawLine(pivot + Vector3.right * width, pivot + Vector3.right * width + Vector3.forward * height);
            Gizmos.DrawLine(pivot, pivot + Vector3.forward * height);
            Gizmos.DrawLine(pivot + Vector3.forward * height, pivot + Vector3.forward * height + Vector3.right * width);
        }

        Gizmos.color = Color.green;
        //Draw Level
        DrawRect(m_levelArea);
        //Draw Pitch
        DrawRect(m_pitchArea);

        //Draw Goals and Score Offsets
        DrawRect(m_goalAreaBlue);
        DrawRect(m_goalAreaRed);
        Gizmos.color = Color.red;
        DrawRect(new Rect(m_goalAreaBlue.position, new Vector2(scoreRange, goalHeight)));
        DrawRect(new Rect(m_goalAreaRed.position, new Vector2(-scoreRange, goalHeight)));
    }

    public bool OutOfPitchWidth(Vector3 pos)
    {
        return pos.x < 0 || pos.x > pitchWidth;
    }

    public bool OutOfPitchHeight(Vector3 pos)
    {
        return pos.z < 0 || pos.z > pitchHeight;
    }

    public bool OutOfPitch(Vector3 pos)
        => OutOfPitchHeight(pos) || OutOfPitchWidth(pos);

    public Vector3 ClampPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Clamp(pos.x, 0, pitchWidth), pos.y, Mathf.Clamp(pos.z, 0, pitchHeight));
    }

    public PlayerAgent NearestTeammate(TeamColor color)
    {
        List<PlayerAgent> teammates = Teammates(color);

        PlayerAgent closest = null;
        float dist = float.MaxValue;
        float curDist = default;
        foreach (var mate in teammates)
        {
            if ((curDist = (mate.position - soccer.transform.position).magnitude) < dist)
            {
                closest = mate;
                dist = curDist;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        void DrawRectangle(Vector2 min, Vector2 max)
        {
            float width = max.x - min.x;
            float height = max.y - min.y;
            Vector3 min3 = new Vector3(min.x, 0, min.y);
            Vector3 max3 = new Vector3(max.x, 0, max.y);
            Gizmos.DrawLine(min3, min3 + width * Vector3.right);
            Gizmos.DrawLine(min3 + width * Vector3.right, max3);
            Gizmos.DrawLine(max3, max3 - width * Vector3.right);
            Gizmos.DrawLine(max3 - width * Vector3.right, min3);
        }

        Gizmos.color = Color.black;
        if (Application.isPlaying)
        {
            //Draw Pitch Area
            DrawRectangle(m_pitchArea.min, m_pitchArea.max);
        }
    }
}