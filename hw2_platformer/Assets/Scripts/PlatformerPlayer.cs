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
    public int playerIndex = -1;

    [Space(15)]
    public Rigidbody2D rb;
    public Collider2D col;
    public AnimatedObject animator;
    public GameObject deathObject;

    [Space(15)]
    public float xInput = 0;
    public float xDampedInputFactor = 0;
    private float xDampedPass = 0;
    private float jumpTimestamp = 0;
    private float lastVelocityY = 0;
    private float lastFloorTime = 0;
    private bool lastFloor = false;
    bool jumping = false; // Only used for floor buffering
    bool dead = false; // Used for single death effect

    [Space(15)]
    [Range(0f, 1f)]
    public float xDampTime = 0.7f;
    public float normalWeight = 0.5f;
    public float moveSpeed;
    public float jumpForce;
    public float jumpBufferTime = 0.2f;
    public float minimumJumpTime = 0.1f;
    public float floorBufferTime = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        keypresses = new Dictionary<string, InputObject>();
        keypresses.Add("Jump", new InputObject());

        lastFloor = GetOnFloor();
    }

    // Update is called once per frame
    void Update()
    {
        MovementLogic();
        JumpingLogic();
        AnimationLogic();

        lastVelocityY = rb.linearVelocityY;
        lastFloor = GetOnFloor();

        foreach (KeyValuePair<string, InputObject> kp in keypresses)
            kp.Value.EndOfFrame();

        if (transform.position.y < -5)
            manager.EndLevel(true);
    }

    public void MovementLogic()
    {
        xDampedInputFactor = Mathf.SmoothDamp(xDampedInputFactor, xInput, ref xDampedPass, xDampTime);

        if (Mathf.Abs(xInput - xDampedInputFactor) < 0.01f)
            xDampedInputFactor = xInput;

        if (!GetOnFloor())
            xDampedInputFactor = xInput;

        rb.linearVelocityX = xDampedInputFactor * moveSpeed;

        if (xDampedInputFactor < 0)
            animator.sr.flipX = true;
        else if (xDampedInputFactor > 0)
            animator.sr.flipX = false;

    }

    public void JumpingLogic()
    {
        if (lastFloor != GetOnFloor())
        {
            if (!GetOnFloor())
                lastFloorTime = Time.time;
            else
                jumping = false;
        }

        if (!keypresses["Jump"].GetPressed() && Time.time - jumpTimestamp >= minimumJumpTime && rb.linearVelocityY > 0.05f)
            rb.linearVelocityY = 0;
        else if (keypresses["Jump"].GetStarted(jumpBufferTime))
        {
            //print("JUMP");
            if ((GetOnFloor() || (Time.time - lastFloorTime <= floorBufferTime)) && !jumping)
            {
                jumping = true;
                jumpTimestamp = Time.time;
                rb.linearVelocityY = jumpForce;
                rb.linearVelocityX *= 1.05f;
            }
        }
    }

    public void AnimationLogic()
    {
        if (GetOnFloor())
        {
            if (Mathf.Abs(xDampedInputFactor) > 0.01f)
                animator.changeAnimation(1, false);
            else
                animator.changeAnimation(0, false);
        }
        else
        {
            if (rb.linearVelocityY > 0)
                animator.changeAnimation(2, false);
            else if (rb.linearVelocityY < 0 && lastVelocityY >= 0)
                animator.changeAnimation(3, false);
            else if (rb.linearVelocityY < 0 && animator.currentClip != animator.animatedClips.clips[3] && animator.currentClip != animator.animatedClips.clips[4])
                animator.changeAnimation(4, false);
        }
    }

    public bool GetOnFloor()
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
        float y = ctx.ReadValue<Vector2>().y;

        if (ctx.control.device is Gamepad && Mathf.Abs(y) > 0.65f)
            xInput = 0;

        if (xInput > 0) xInput = Mathf.Ceil(xInput);
        if (xInput < 0) xInput = Mathf.Floor(xInput);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            //Debug.Log("Jump started");
            keypresses["Jump"].PressKey();
        }
        else if (ctx.canceled)
        {
            //Debug.Log("Jump canceled");
            keypresses["Jump"].ReleaseKey();
        }
    }

    public void Switch(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        manager.PlayerSwitchInput(playerIndex);
    }

    public void AddInput(LocalPlayerObject p)
    {
        inputObject = p;
        inputObject.movePlayer += Move;
        inputObject.jumpPlayer += Jump;
        inputObject.switchPlayer += Switch;
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
        inputObject.switchPlayer -= Switch;
    }

    private void OnEnable()
    {
        animator.sr.enabled = true;
        dead = false;
    }

    private void OnDisable()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        animator.changeAnimation(0, false);
    }

    bool DirectionDeath(float killerAngle)
    {
        if (killerAngle == 0 && rb.linearVelocityY > 0.1f)
            return false;
        if (killerAngle == 180 && rb.linearVelocityY < -0.1f)
            return false;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dead && collision.tag == "Kill")
        {
            if (DirectionDeath(collision.gameObject.transform.rotation.eulerAngles.z))
                KillPlayer();
        }
    }

    public void KillPlayer()
    {
        manager.EndLevel(true);
        animator.sr.enabled = false;
        Instantiate(deathObject, transform.position, Quaternion.identity);
        dead = true;
    }
}
