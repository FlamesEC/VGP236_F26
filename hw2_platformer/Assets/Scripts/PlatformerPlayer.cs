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

        public bool GetPressed(float bufferTime = 0)
        {
            if (performing)
                return true;

            if (Time.time - lastReleaseTimestamp <= bufferTime)
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
            lastPressTimestamp = Time.time;
            lastReleaseTimestamp = Time.time;
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

    [Space(15)]
    [Range(0f, 1f)]
    public float normalWeight = 0.5f;
    public float moveSpeed;
    public float jumpForce;
    public float jumpBufferTime = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keypresses = new Dictionary<string, InputObject>();
        keypresses.Add("Jump", new InputObject());
    }

    // Update is called once per frame
    void Update()
    {
        if (keypresses["Jump"].GetPressed(jumpBufferTime))
        {
            print("JUMP");
            if (GetOnFloor())
                rb.linearVelocityY = jumpForce;
        }
        else if (keypresses["Jump"].released && rb.linearVelocityY > 0.05f)
            rb.linearVelocityY = 0;

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
        
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            keypresses["Jump"].PressKey();
        else if (ctx.canceled)
            keypresses["Jump"].ReleaseKey();
    }

    public void AddInput(LocalPlayerObject p)
    {
        inputObject = p;
        inputObject.movePlayer = Move;
        inputObject.jumpPlayer = Jump;
    }

    public void RemoveInput()
    {
        inputObject = null;
    }
}
