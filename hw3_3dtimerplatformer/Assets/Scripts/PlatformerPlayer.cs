using System;
using System.Collections.Generic;
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

    [Space(20)]
    [SerializeField] public Dictionary<string, InputObject> keypresses;
    ContactPoint[] contacts;

    [Space(15)]
    public Transform cameraRotator;
    public thirdperson_PlayerInput Input;
    private GroundCheck groundCheck;
    public SpriteRenderer shadowObject;
    public LayerMask floorMask;
    float camRotX = 0;
    float selfRotY = 0;
    float _xAngVelRef = 0;
    float _yAngVelRef = 0;
    float shadowDefSize = 2;
    Vector2 moveInput;

    [Space(15)]
    private float jumpTimestamp = 0;
    private float lastVelocityY = 0;
    private float lastFloorTime = 0;
    private bool lastFloor = false;
    bool jumping = false; // Only used for floor buffering

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
    public int airJumps = 1;
    int currentAirJumps = 0;

    [Space(15)]
    public float gravityForce = -9.81f;
    public float gravityScale = 1f;
    public float jumpGravityScale = 1.2f;
    public float fallGravityScale = 0.8f;
    public float airJumpGravityScale = 1.2f;

    [Space(15)]
    public float shadowDistance = 3f;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        Input = new thirdperson_PlayerInput();
        Input.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        gravityScale = jumpGravityScale;
        camRotX = cameraRotator.localRotation.eulerAngles.x;
        selfRotY = transform.localRotation.eulerAngles.y;
        shadowDefSize = shadowObject.size.x;

        keypresses = new Dictionary<string, InputObject>();
        keypresses.Add("Jump", new InputObject());

        groundCheck = GetComponent<GroundCheck>();
        lastFloor = groundCheck.GetOnFloor();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    protected override void ObjectUpdate()
    {
        MovementLogic();
        LookingLogic();
        JumpingLogic();

        Vector3 gravity = gravityForce * gravityScale * -Vector3.up;

        if (!groundCheck.GetOnFloor())
            selfVelocity += gravity * Time.deltaTime;

        lastVelocityY = selfVelocity.y;
        lastFloor = groundCheck.GetOnFloor();

        foreach (KeyValuePair<string, InputObject> kp in keypresses)
            kp.Value.EndOfFrame();
    }

    private void LateUpdate()
    {
        ShadowCast();
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
        moveInput = Input.Player.Move.ReadValue<Vector2>();

        if (Mathf.Abs(moveInput.magnitude) < 0.1f)
            moveInput = Vector2.zero;

        Vector3 moveVector = Vector3.zero;
        moveVector += moveInput.y * transform.forward;
        moveVector += moveInput.x * transform.right;

        moveVector *= moveSpeed;

        selfVelocity = new Vector3(moveVector.x, selfVelocity.y, moveVector.z);
    }

    public void JumpingLogic()
    {
        if (lastFloor != groundCheck.GetOnFloor())
        {
            if (!groundCheck.GetOnFloor())
                lastFloorTime = Time.time;
            else
            {
                jumping = false;
                currentAirJumps = 0;
                selfVelocity.Scale(new Vector3(1, 0, 1));
            }
        }

        if (selfVelocity.y < 0 && (gravityScale == jumpGravityScale || gravityScale == airJumpGravityScale))
            gravityScale = fallGravityScale;

        if (minimumJumpTime != -1 && !keypresses["Jump"].GetPressed() && Time.time - jumpTimestamp >= minimumJumpTime && rb.linearVelocity.y > 0.05f)
            selfVelocity = Vector3.Scale(selfVelocity, new Vector3(1, 0, 1));
        else if (keypresses["Jump"].GetStarted(jumpBufferTime))
        {
            if ((groundCheck.GetOnFloor() || (Time.time - lastFloorTime <= floorBufferTime)) && !jumping)
            {
                jumping = true;
                jumpTimestamp = Time.time;
                selfVelocity = new Vector3(selfVelocity.x, jumpForce, selfVelocity.z);
                gravityScale = jumpGravityScale;
            }
            else if (airJumps - currentAirJumps > 0 && keypresses["Jump"].GetStarted())
            {
                selfVelocity = new Vector3(selfVelocity.x, jumpForce, selfVelocity.z);
                gravityScale = airJumpGravityScale;
                currentAirJumps++;
            }
        }
    }

    public void LookingLogic()
    {
        Vector2 input = Input.Player.Look.ReadValue<Vector2>();

        if (Mathf.Abs(input.magnitude) < 0.05f)
            input = Vector2.zero;

        float modifier = 0.1f * (1/60f);

        camRotX -= input.y * modifier * camSensitivity;
        camRotX = Mathf.Clamp(camRotX, minCamRot, maxCamRot);

        selfRotY += input.x * modifier * camSensitivity;

        transform.localRotation = Quaternion.Euler(new Vector3(0, selfRotY, 0));
        cameraRotator.localRotation = Quaternion.Euler(new Vector3(camRotX, 0, 0));
    }

    public void JumpStart(InputAction.CallbackContext ctx)
    {
        keypresses["Jump"].PressKey();
    }

    public void JumpEnd(InputAction.CallbackContext ctx)
    {
        keypresses["Jump"].ReleaseKey();
    }

    private void OnEnable()
    {
        Input.Player.Jump.started += JumpStart;
        Input.Player.Jump.canceled += JumpEnd;
    }

    private void OnDisable()
    {
        Input.Player.Jump.started -= JumpStart;
        Input.Player.Jump.canceled -= JumpEnd;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 2));
    }
}
