using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationObject", menuName = "Scriptable Objects/Animation Object")]
public class AnimationObject : ScriptableObject
{
    public AnimationClipObject[] clips;
}

