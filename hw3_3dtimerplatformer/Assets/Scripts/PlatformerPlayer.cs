using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal;
using UnityEngine.Windows;

public class PlatformerPlayer : InheritableVelocityObject
{
    [Serializable]
    public class InputObject
    {
        public float lastPressTimestamp = -10;
        public float lastReleaseTimestamp = -10;
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
            lastPressTimestamp = -10;
            lastReleaseTimestamp = -10;
            performing = false;
            started = false;
            released = false;
        }
    }

    [Space(20)]
    [SerializeField] public Dictionary<string, InputObject> keypresses;
    ContactPoint[] contacts;

    [Space(15)]
    public Transform cameraRotator;
    public thirdperson_PlayerInput PlayInput;
    public SpriteRenderer shadowObject;
    public Animator animator;
    public LayerMask floorMask;
    public Vector3 checkpointPos;
    float camRotX = 0;
    //float selfRotY = 0;
    float shadowDefSize = 2;
    Vector2 moveInput;
    public bool canMove = true;
    public bool blockRotationOnDive = true;

    [Space(15)]
    private float jumpTimestamp = 0;
    private float lastVelocityY = 0;
    private float lastFloorTime = 0;
    private bool lastFloor = false;
    bool jumping = false; // Only used for floor buffering
    private float reachFloorTime = 0;
    Quaternion modelRot;

    [Space(15)]
    public float moveSpeed;

    [Space(15)]
    public float camSensitivity = 70;
    public float minCamRot = -30;
    public float maxCamRot = 30;

    [Space(15)]
    public float jumpForce;
    public float jumpBufferTime = 0.2f;
    public float minimumJumpTime = 0.1f;
    public float floorBufferTime = 0.1f;
    public float minimumAirJumpTime = 0.2f;
    public int usableAirJumps = 1;
    int currentAirJumps = 0;

    [Space(15)]
    public int usableDiveCount = 1;
    int currentDives = 0;
    public float diveForce = 20;
    float currentDiveForce = 20;
    public float diveJumpForce = 10;
    public float diveChainTime = 1;
    public bool onDiveTime = false;
    Vector3 diveVec = Vector3.zero;
    Vector3 diveDir = Vector3.zero;

    [Space(15)]
    public float gravityForce = -9.81f;
    public float gravityScale = 1f;
    public float jumpGravityScale = 1.2f;
    public float fallGravityScale = 0.8f;
    public float diveGravityScale = 1.2f;
    public float airJumpGravityScale = 1.2f;

    [Space(15)]
    public float shadowDistance = 3f;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        PlayInput = new thirdperson_PlayerInput();
        PlayInput.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gravityScale = jumpGravityScale;
        camRotX = cameraRotator.localRotation.eulerAngles.x;
        SetSelfRotation(new Vector3(0, transform.localRotation.eulerAngles.y));
        shadowDefSize = shadowObject.size.x;

        keypresses = new Dictionary<string, InputObject>();
        keypresses.Add("Jump", new InputObject());
        keypresses.Add("Dive", new InputObject());

        lastFloor = groundCheck.GetOnFloor();

        checkpointPos = transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    protected override void ObjectUpdate()
    {
        MovementLogic();
        LookingLogic();
        JumpingLogic();
        DiveLogic();

        Vector3 gravity = gravityForce * gravityScale * -Vector3.up;

        if (!groundCheck.GetOnFloor())
            selfVelocity += gravity * Time.deltaTime;

        AnimationLogic();

        lastVelocityY = selfVelocity.y;
        lastFloor = groundCheck.GetOnFloor();

        selfVelocity += diveVec;

        foreach (KeyValuePair<string, InputObject> kp in keypresses)
            kp.Value.EndOfFrame();
    }

    private void LateUpdate()
    {
        ShadowCast();

        if (transform.position.y < -15)
            KillPlayer();
    }

    public void ShadowCast()
    {
        RaycastHit ray;

        shadowObject.gameObject.SetActive(Physics.Raycast(transform.position + new Vector3(0, 1, 0), -transform.up, out ray, shadowDistance + 1, floorMask));
        if (shadowObject.gameObject.activeSelf)
        {
            shadowObject.size = Vector2.one * (shadowDefSize * ((shadowDistance - ray.distance) / shadowDistance));
            shadowObject.transform.position = ray.point + new Vector3(0, 0.01f, 0);
        }
    }

    public void MovementLogic()
    {
        if (!canMove)
        {
            moveInput = Vector3.zero;
            selfVelocity = Vector3.up * selfVelocity.y;
            return;
        }

        moveInput = PlayInput.Player.Move.ReadValue<Vector2>();

        if (Mathf.Abs(moveInput.x) < 0.3f && Mathf.Abs(moveInput.y) < 0.3f)
            moveInput = Vector2.zero;

        Vector3 moveVector = Vector3.zero;

        float forwardsFactor = moveInput.y;
        if (currentDiveForce > 0)
            forwardsFactor = currentDiveForce / diveForce;

        Vector3 fwd = transform.forward;
        if (GetOnDive() && blockRotationOnDive)
            fwd = diveDir;

        moveVector += forwardsFactor * fwd;

        if (!GetOnDive() || !blockRotationOnDive)
            moveVector += moveInput.x * transform.right;

        moveVector *= moveSpeed;

        selfVelocity = new Vector3(moveVector.x, selfVelocity.y, moveVector.z);
    }

    public bool GetOnDive()
    {
        return currentDiveForce > 0 && onDiveTime && !groundCheck.GetOnFloor();
    }

    public void JumpingLogic()
    {
        if (lastFloor != groundCheck.GetOnFloor())
        {
            if (!groundCheck.GetOnFloor())
            {
                lastFloorTime = Time.time;
                if (currentDiveForce < diveForce * 0.5f)
                    onDiveTime = false;
            }
            else
            {
                jumping = false;
                currentAirJumps = 0;
                currentDives = 0;
                selfVelocity.Scale(new Vector3(1, 0, 1));
                currentDiveForce = 0;
                reachFloorTime = Time.time;
            }
        }

        if (!canMove)
            return;

        if (selfVelocity.y < 0 && (gravityScale == jumpGravityScale || gravityScale == airJumpGravityScale))
            gravityScale = fallGravityScale;

        if (minimumJumpTime != -1 && !keypresses["Jump"].GetPressed() && Time.time - jumpTimestamp >= minimumJumpTime && rb.linearVelocity.y > 0.05f)
            selfVelocity = Vector3.Scale(selfVelocity, new Vector3(1, 0, 1));
        else if (keypresses["Jump"].GetStarted(jumpBufferTime))
        {
            if ((groundCheck.GetOnFloor() || (Time.time - lastFloorTime <= floorBufferTime)) && !jumping)
            {
                onDiveTime = false;
                jumping = true;
                jumpTimestamp = Time.time;
                selfVelocity = new Vector3(selfVelocity.x, jumpForce, selfVelocity.z);
                gravityScale = jumpGravityScale;

                ClearMomentum();
            }
            else if (usableAirJumps - currentAirJumps > 0 && keypresses["Jump"].GetStarted(jumpBufferTime) && Time.time - jumpTimestamp >= minimumAirJumpTime)
            {
                onDiveTime = false;
                selfVelocity = new Vector3(selfVelocity.x, jumpForce, selfVelocity.z);
                gravityScale = airJumpGravityScale;
                jumpTimestamp = Time.time;
                currentAirJumps++;

                ClearMomentum();
            }
        }
    }

    public void LookingLogic()
    {
        if (!canMove)
            return;

        Vector2 input = PlayInput.Player.Look.ReadValue<Vector2>();

        if (Mathf.Abs(input.magnitude) < 0.05f)
            input = Vector2.zero;

        float modifier = 0.1f * (1 / 60f);

        camRotX -= input.y * modifier * camSensitivity;
        camRotX = Mathf.Clamp(camRotX, minCamRot, maxCamRot);

        AddSelfRotation(new Vector3(0, input.x * modifier * camSensitivity, 0));

        cameraRotator.localRotation = Quaternion.Euler(new Vector3(camRotX, 0, 0));
    }

    public void DiveLogic()
    {
        if (keypresses["Dive"].GetStarted() && canMove)
        {
            if ((groundCheck.GetOnFloor() && (Time.time - reachFloorTime >= diveChainTime || !onDiveTime)) || (!groundCheck.GetOnFloor() && usableDiveCount - currentDives > 0))
            {
                onDiveTime = true;
                currentDiveForce = diveForce;
                selfVelocity.Scale(new Vector3(1, 0, 1));
                selfVelocity += Vector3.up * diveJumpForce;
                currentDives++;
                gravityScale = diveGravityScale;
                diveDir = transform.forward;
                ClearMomentum();
            }
        }

        float yInput = PlayInput.Player.Move.ReadValue<Vector2>().y;

        if (currentDiveForce > 0)
        {
            if (!onDiveTime)
            {
                if (yInput > 0.5f)
                    currentDiveForce = Mathf.Lerp(currentDiveForce, yInput * currentDiveForce, 1.0f - Mathf.Pow(0.2f, Time.deltaTime));
                else
                    currentDiveForce = 0;
            }
        }

        if (GetOnDive() && blockRotationOnDive)
            diveVec = diveDir * currentDiveForce;
        else
            diveVec = transform.forward * currentDiveForce;
    }

    public void AnimationLogic()
    {
        if (lastFloor != groundCheck.GetOnFloor())
        {
            string stateName = groundCheck.GetOnFloor() ? "ground" : "air";
            animator.Play(stateName + "_code", 0);
        }

        Vector3 parentRot = GetRotation() - selfRotation;
        if (Mathf.Abs(Vector3.Scale(selfVelocity, new Vector3(1, 0, 1)).magnitude) >= 0.05f && (!GetOnDive() || !blockRotationOnDive))
        {
            modelRot = transform.rotation;
            parentRot = Vector3.zero;
        }
        
        animator.gameObject.transform.rotation = Quaternion.Euler(parentRot + modelRot.eulerAngles);

        animator.SetFloat("XSpeed", Mathf.Abs(Vector3.Scale(selfVelocity, new Vector3(1, 0, 1)).magnitude));
        animator.SetFloat("YSpeed", selfVelocity.y);
        animator.SetFloat("YSpeedDelta", selfVelocity.y - lastVelocityY);
        animator.SetFloat("Input", canMove ? Mathf.Abs(moveInput.magnitude) : 0);
        animator.SetFloat("DirectionX", transform.InverseTransformDirection(selfVelocity).x / moveSpeed);
        animator.SetFloat("DirectionY", transform.InverseTransformDirection(selfVelocity).z / moveSpeed);
        animator.SetBool("OnGround", groundCheck.GetOnFloor());
        animator.SetBool("OnDive", onDiveTime && !groundCheck.GetOnFloor());
    }

    public void JumpStart(InputAction.CallbackContext ctx)
    {
        keypresses["Jump"].PressKey();
    }

    public void JumpEnd(InputAction.CallbackContext ctx)
    {
        keypresses["Jump"].ReleaseKey();
    }

    public void DiveStart(InputAction.CallbackContext ctx)
    {
        keypresses["Dive"].PressKey();
    }

    public void DiveEnd(InputAction.CallbackContext ctx)
    {
        keypresses["Dive"].ReleaseKey();
    }

    private void OnEnable()
    {
        PlayInput.Player.Jump.started += JumpStart;
        PlayInput.Player.Jump.canceled += JumpEnd;

        PlayInput.Player.Dive.started += DiveStart;
        PlayInput.Player.Dive.canceled += DiveEnd;
    }

    private void OnDisable()
    {
        PlayInput.Player.Jump.started -= JumpStart;
        PlayInput.Player.Jump.canceled -= JumpEnd;

        PlayInput.Player.Dive.started -= DiveStart;
        PlayInput.Player.Dive.canceled -= DiveEnd;
    }

    private void OnDestroy()
    {
        PlayInput.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 2));
    }

    /*
    private void OnGUI()
    {
        Rect r = new Rect(Vector2.zero, new Vector2(200, 60));
        GUI.TextField(r, Input.Player.Move.ReadValue<Vector2>().ToString());
    }
    */

    public void KillPlayer()
    {
        transform.position = checkpointPos;
        onDiveTime = false;
        currentDiveForce = 0;
        jumping = false;
        selfVelocity = Vector3.zero;

        //diveChainTime = -10;
        jumpTimestamp = -10;
        reachFloorTime = -10;
        lastFloorTime = -10;

        currentAirJumps = 0;
        currentDives = 0;

        if (objectQueue != null)
            objectQueue.Clear();
        if (velocityAdditions != null)
            velocityAdditions.Clear();
        if (rotationAdditions != null)
            rotationAdditions.Clear();

        animator.Play("air_code", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            checkpointPos = other.transform.position;
            Destroy(other.gameObject);
        }

        if (other.tag == "Kill")
            KillPlayer();
    }
}
