using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AnimatedObject : MonoBehaviour
{
    public SpriteRenderer sr;
    public AnimationObject animatedClips;
    public AnimationClipObject currentClip;
    public int frameIndex;
    private float timeSave;
    private float timePerFrame;
    private bool hasFinished = false;
    public float speedMultiplier = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (currentClip == null)
            changeAnimation(0);
        else
        {
            int index = animatedClips.clips.ToList().IndexOf(currentClip);
            currentClip = null;
            changeAnimation(index);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animatedClips == null)
            return;

        if (Time.time > ((timePerFrame / speedMultiplier) + timeSave) && !hasFinished && timePerFrame > 0)
            animationHandler();
    }

    void animationHandler()
    {
        frameIndex++;
        if (frameIndex > currentClip.frames.Length - 1)
        {
            if (currentClip.connectedAnimation == -1)
                frameIndex = 0;
            else
            {
                changeAnimation(currentClip.connectedAnimation);
                return;
            }
        }
        if (sr != null)
            sr.sprite = currentClip.frames[frameIndex];

        timeSave = Time.time;
    }

    public void changeAnimation(int clipIndex, bool restart = true)
    {
        if (animatedClips == null)
            return;

        if (currentClip == animatedClips.clips[clipIndex] && !restart)
            return;

        //print(clipIndex);
        if (clipIndex >= animatedClips.clips.Length)
            return;

        currentClip = animatedClips.clips[clipIndex];
        if (currentClip.resetFrameIndex)
        {
            frameIndex = 0;
            timeSave = Time.time;
        }
        if (currentClip.framesPerSecond > 0)
            timePerFrame = 1f / currentClip.framesPerSecond;
        else
            timePerFrame = 0f;

        hasFinished = false;

        frameIndex--;
        animationHandler();
    }

    public void OnEnable()
    {
        resetAnimation();
    }

    public void resetAnimation()
    {
        frameIndex = 0;
        timeSave = Time.time;
    }
}
