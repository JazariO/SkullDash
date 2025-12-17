using System;
using UnityEditor.Overlays;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    public enum State
    { 
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        Crouch,
    }

    [Header("Scriptable Objects")]
    [SerializeField] GameEventDataSO gameEventDataSO;
    [SerializeField] PlayerSettingsSO settings;
    [SerializeField] PlayerStatsSO stats;
    [SerializeField] InputDataSO inputs;
    [SerializeField] LayerSettingsSO layerData;
    [SerializeField] UserSettingsSO userSettings;

    [Header("Components")]
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Camera cam;

    State curState;

    [Serializable] public struct TempStats
    {
        public bool coyoteJump;
        public float coyoteTimeElapsed;
        public float curTargetSpeed;
        public Vector3 moveVelocity;
        public Quaternion moveDirection;
        public bool isGrounded;
        public bool willJump;
        public float lastJumpTime;
        public float curPitch;
        public float curYaw;

    }
    TempStats tempStats;

    private void Awake()
    {
        
    }

    private void Start()
    {
        curState = State.Idle;
    }
    private void Update()
    {
        SelectState();
        UpdateState();
        UpdateRotation();
        tempStats.willJump = Time.time - tempStats.lastJumpTime <= settings.jumpBufferTime && curState != State.Jump;
    }
    private void FixedUpdate()
    {
        FixedUpdateState();
        FixedUpdateRotate();
        UpdateHorizontalVelocity();
        UpdateVerticalVelocity();
        tempStats.isGrounded = IsGrounded();
    }
    private void SelectState()
    {   
        if ((tempStats.isGrounded && tempStats.willJump) || tempStats.moveVelocity.y > 0 || tempStats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (tempStats.moveVelocity.y <= 0 && !tempStats.isGrounded)
        {
            SetState(State.Fall);
        }
        else if (inputs.sprintInput && inputs.moveInput != Vector2.zero && tempStats.isGrounded)
        {
            SetState(State.Run);
        }
        else if (tempStats.isGrounded && inputs.moveInput != Vector2.zero)
        {
            SetState(State.Walk);
        }
        else
        {
            SetState(State.Idle);
        }
    }
    private void SetState(State newState)
    {
        if (curState == newState) return;
        ExitState();
        curState = newState;
        EnterState();
    }
    private void EnterState()
    {
        switch (curState)
        {
            case State.Idle:
            {

            }
            break;
            case State.Walk:
            {
                tempStats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                tempStats.curTargetSpeed = settings.runSpeed;
            }
            break;
            case State.Jump:
            {
                tempStats.moveVelocity.y = settings.jumpSpeed;
                tempStats.coyoteTimeElapsed = float.MaxValue;
            }
            break;
            case State.Fall:
            {
                tempStats.coyoteTimeElapsed += Time.deltaTime;
                tempStats.coyoteJump = tempStats.coyoteTimeElapsed < settings.coyoteTime && inputs.jumpInput;

                if (inputs.jumpInput)
                {
                    tempStats.lastJumpTime = Time.time;
                    inputs.jumpInput = false;
                }
            }
            break;
            case State.Crouch:
            {

            }
            break;
        }
    }
    private void UpdateState()
    {
        switch (curState)
        {
            case State.Idle:
            {
                if (inputs.jumpInput) tempStats.lastJumpTime = Time.time;
            }
            break;
            case State.Walk:
            {
                if (inputs.jumpInput) tempStats.lastJumpTime = Time.time;
                tempStats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                if (inputs.jumpInput) tempStats.lastJumpTime = Time.time;
                tempStats.curTargetSpeed = settings.runSpeed;
            }
            break;
            case State.Jump:
            {

            }
            break;
            case State.Fall:
            {

            }
            break;
            case State.Crouch:
            {

            }
            break;
        }
    }
    private void FixedUpdateState()
    {
        switch (curState)
        {
            case State.Idle:
            {

            }
            break;
            case State.Walk:
            {

            }
            break;
            case State.Run:
            {

            }
            break;
            case State.Jump:
            {
                if (rigidBody.linearVelocity.y < settings.antiGravApexThreshold)
                {
                    rigidBody.AddForce(Physics.gravity * (settings.gravity * settings.antiGravApexThreshold), ForceMode.Acceleration);
                }
            }
            break;
            case State.Fall:
            {

            }
            break;
            case State.Crouch:
            {

            }
            break;
        }
    }
    private void ExitState()
    {
        switch (curState)
        {
            case State.Idle:
            {

            }
            break;
            case State.Walk:
            {

            }
            break;
            case State.Run:
            {

            }
            break;
            case State.Jump:
            {

            }
            break;
            case State.Fall:
            {

            }
            break;
            case State.Crouch:
            {

            }
            break;
        }
    }
    private void FixedUpdateRotate()
    {
        rigidBody.MoveRotation(tempStats.moveDirection);
    }

    private void UpdateRotation()
    {
        float yaw = inputs.lookInput.x * userSettings.sensitivity;
        tempStats.curYaw += yaw;
        tempStats.moveDirection = Quaternion.Euler(0.0f, tempStats.curYaw, 0.0f);
        float pitch = inputs.lookInput.y * userSettings.sensitivity;
        tempStats.curPitch -= pitch;
        tempStats.curPitch = Mathf.Clamp(tempStats.curPitch, -settings.clampedPitch, settings.clampedPitch);
        cam.transform.localRotation = Quaternion.Euler(tempStats.curPitch, 0, 0);
    }
    private void UpdateHorizontalVelocity()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 inputDir = forward * inputs.moveInput.y + right * inputs.moveInput.x;
        Vector3 targetHorVelocity = inputDir * tempStats.curTargetSpeed;

        tempStats.moveVelocity.x = Mathf.Lerp(tempStats.moveVelocity.x, targetHorVelocity.x, settings.groundAcceleration * Time.fixedDeltaTime);
        tempStats.moveVelocity.z = Mathf.Lerp(tempStats.moveVelocity.z, targetHorVelocity.z, settings.groundAcceleration * Time.fixedDeltaTime);
        rigidBody.linearVelocity = tempStats.moveVelocity;
    }

    private void UpdateVerticalVelocity()
    {
        tempStats.moveVelocity.y -= settings.gravity * Time.fixedDeltaTime;

        //Clamping Fall speed
        tempStats.moveVelocity.y = Mathf.Max(tempStats.moveVelocity.y, settings.maxFallSpeed);
    }
    private bool IsGrounded()
    {
        Vector3 bottom = capsuleCollider.bounds.center + Vector3.down * (capsuleCollider.bounds.extents.y - settings.groundCheckRadius + settings.groundCheckDistance);
        
        return Physics.CheckCapsule(capsuleCollider.bounds.center, bottom, settings.groundCheckRadius, layerData.ground);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = tempStats.isGrounded ? Color.green : Color.red;

        if (capsuleCollider != null)
        {
            Vector3 bottom = capsuleCollider.bounds.center + Vector3.down * (capsuleCollider.bounds.extents.y - settings.groundCheckRadius + settings.groundCheckDistance);


            Gizmos.DrawWireSphere(capsuleCollider.bounds.center, settings.groundCheckRadius);
            Gizmos.DrawWireSphere(bottom, settings.groundCheckRadius);

            Gizmos.DrawLine(capsuleCollider.bounds.center + Vector3.left * settings.groundCheckRadius, bottom + Vector3.left * settings.groundCheckRadius);
            Gizmos.DrawLine(capsuleCollider.bounds.center + Vector3.right * settings.groundCheckRadius, bottom + Vector3.right * settings.groundCheckRadius);
            Gizmos.DrawLine(capsuleCollider.bounds.center + Vector3.forward * settings.groundCheckRadius, bottom + Vector3.forward * settings.groundCheckRadius);
            Gizmos.DrawLine(capsuleCollider.bounds.center + Vector3.back * settings.groundCheckRadius, bottom + Vector3.back * settings.groundCheckRadius);
        }
    }
}
