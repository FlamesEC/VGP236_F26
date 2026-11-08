using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationClipObject", menuName = "Scriptable Objects/Animation Clip Object")]
public class AnimationClipObject : ScriptableObject
{
    public Sprite[] frames;
    public int connectedAnimation = -1;
    public bool resetFrameIndex = false;
    public int framesPerSecond;
}

