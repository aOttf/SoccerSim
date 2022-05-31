using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Params : ScriptableObject
{
    [Header("Environment")]
    public float height;
    public float width;
    public float pitchHeight;
    public float pitchWidth;

    //[Header("Players")]
    [Header("Soccer")]
    public float kickStrength;
    public float drag;

    [Header("Goal")]
    public float goalHeight;
    public float goalDepth;
    public float shootOffset; // The Offset to the goal in which the ball appears will be regarded as a successful shoot
    //[Header("Difficulty")]
}