using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerPlayer : MonoBehaviour
{
    [Serializable]
    public class InputObject
    {
        public float lastPressTimestamp = 0;
        public float lastReleaseTimestamp = 0;
        public bool performing = false;
        public bool started = false;
        public bool released = false;

        public void PressKey()
        {
            started = true;
            performing = true;
            lastPressTimestamp = Time.time;
        }

        public void ReleaseKey()
        {
            performing = false;
            released = true;
            lastReleaseTimestamp = Time.time;
        }

        public bool GetStarted(float bufferTime = 0)
        {
            if (started)
                return true;

            if (bufferTime > 0 && Time.time - lastPressTimestamp <= bufferTime)
                return true;

            return false;
        }

        public bool GetPressed(float bufferTime = 0)
        {
            if (performing)
                return true;

            if (bufferTime > 0 && Time.time - lastReleaseTimestamp <= bufferTime)
                return true;

            return false;
        }

        public void EndOfFrame()
        {
            started = false;
            released = false;
        }

        public InputObject()
        {
            lastPressTimestamp = 0;
            lastReleaseTimestamp = 0;
            performing = false;
            started = false;
            released = false;
        }
    }

    [SerializeField] public Dictionary<string, InputObject> keypresses;

    public LocalPlayerObject inputObject;
    public PlatformerManager manager;

    [Space(15)]
    public Rigidbody2D rb;
    public Collider2D col;
    public AnimatedObject animator;

    [Space(15)]
    public float xInput = 0;
    public float xDampedInputFactor = 0;
    private float xDampedPass = 0;
    private float jumpTimestamp = 0;

    [Space(15)]
    [Range(0f, 1f)]
    public float xDampTime = 0.7f;
    public float normalWeight = 0.5f;
    public float moveSpeed;
    public float jumpForce;
    public float jumpBufferTime = 0.2f;
    public float minimumJumpTime = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keypresses = new Dictionary<string, InputObject>();
        keypresses.Add("Jump", new InputObject());
    }

    // Update is called once per frame
    void Update()
    {
        xDampedInputFactor = Mathf.SmoothDamp(xDampedInputFactor, xInput, ref xDampedPass, xDampTime);

        rb.linearVelocityX = xDampedInputFactor * moveSpeed;

        if (xDampedInputFactor < 0)
            animator.sr.flipX = true;
        else if (xDampedInputFactor > 0)
            animator.sr.flipX = false;

        //print(keypresses["Jump"].GetPressed());

        if (!keypresses["Jump"].GetPressed() && Time.time - jumpTimestamp >= minimumJumpTime&& rb.linearVelocityY > 0.05f)
            rb.linearVelocityY = 0;
        else if (keypresses["Jump"].GetStarted(jumpBufferTime))
        {
            //print("JUMP");
            if (GetOnFloor())
            {
                jumpTimestamp = Time.time;
                rb.linearVelocityY = jumpForce;
                rb.linearVelocityX *= 1.05f;
            }
        }

        foreach (KeyValuePair<string, InputObject> kp in keypresses)
            kp.Value.EndOfFrame();
    }

    bool GetOnFloor()
    {
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int count = col.GetContacts(contacts);

        bool onFloor = false;

        for (int i = 0; i < count; i++)
        {
            Vector2 normal = contacts[i].normal;

            if (normal.y < normalWeight)
                continue;

            onFloor = true;
            break;
        }

        return onFloor;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        xInput = ctx.ReadValue<Vector2>().x;
        if (xInput > 0) xInput = Mathf.Ceil(xInput);
        if (xInput < 0) xInput = Mathf.Floor(xInput);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Jump phase: {ctx.phase}");

        if (ctx.started)
        {
            Debug.Log("Jump started");
            keypresses["Jump"].PressKey();
        }
        else if (ctx.canceled)
        {
            Debug.Log("Jump canceled");
            keypresses["Jump"].ReleaseKey();
        }
    }

    public void AddInput(LocalPlayerObject p)
    {
        inputObject = p;
        inputObject.movePlayer += Move;
        inputObject.jumpPlayer += Jump;
    }

    public void RemoveInput()
    {
        inputObject = null;
    }

    private void OnDestroy()
    {
        if (inputObject == null)
            return;

        inputObject.movePlayer -= Move;
        inputObject.jumpPlayer -= Jump;
    }
}
