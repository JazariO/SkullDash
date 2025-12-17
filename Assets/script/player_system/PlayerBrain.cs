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

    private void Awake()
    {
        
    }

    private void Start()
    {
        stats.curState = State.Idle;
    }
    private void Update()
    {
        SelectState();
        UpdateState();
        UpdateRotation();
        stats.willJump = Time.time - stats.lastJumpTime <= settings.jumpBufferTime && stats.curState != State.Jump;
    }
    private void FixedUpdate()
    {
        FixedUpdateState();
        FixedUpdateRotate();
        UpdateHorizontalVelocity();
        UpdateVerticalVelocity();
        stats.isGrounded = IsGrounded();
    }
    private void SelectState()
    {   
        if ((stats.isGrounded && stats.willJump) || stats.moveVelocity.y > 0 || stats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (stats.moveVelocity.y <= 0 && !stats.isGrounded)
        {
            SetState(State.Fall);
        }
        else if (inputs.sprintInput && inputs.moveInput != Vector2.zero && stats.isGrounded)
        {
            SetState(State.Run);
        }
        else if (stats.isGrounded && inputs.moveInput != Vector2.zero)
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
        if (stats.curState == newState) return;
        ExitState();
        stats.curState = newState;
        EnterState();
    }
    private void EnterState()
    {
        switch (stats.curState)
        {
            case State.Idle:
            {

            }
            break;
            case State.Walk:
            {
                stats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                stats.curTargetSpeed = settings.runSpeed;
            }
            break;
            case State.Jump:
            {
                stats.moveVelocity.y = settings.jumpSpeed;
                stats.coyoteTimeElapsed = float.MaxValue;
            }
            break;
            case State.Fall:
            {
                stats.coyoteTimeElapsed += Time.deltaTime;
                stats.coyoteJump = stats.coyoteTimeElapsed < settings.coyoteTime && inputs.jumpInput;

                if (inputs.jumpInput)
                {
                    stats.lastJumpTime = Time.time;
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
        switch (stats.curState)
        {
            case State.Idle:
            {
                if (inputs.jumpInput) stats.lastJumpTime = Time.time;
            }
            break;
            case State.Walk:
            {
                if (inputs.jumpInput) stats.lastJumpTime = Time.time;
                stats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                if (inputs.jumpInput) stats.lastJumpTime = Time.time;
                stats.curTargetSpeed = settings.runSpeed;
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
        switch (stats.curState)
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
        switch (stats.curState)
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
        rigidBody.MoveRotation(stats.moveDirection);
    }

    private void UpdateRotation()
    {
        float yaw = inputs.lookInput.x * userSettings.sensitivity;
        stats.curYaw += yaw;
        stats.moveDirection = Quaternion.Euler(0.0f, stats.curYaw, 0.0f);
        float pitch = inputs.lookInput.y * userSettings.sensitivity;
        stats.curPitch -= pitch;
        stats.curPitch = Mathf.Clamp(stats.curPitch, -settings.clampedPitch, settings.clampedPitch);
        cam.transform.localRotation = Quaternion.Euler(stats.curPitch, 0, 0);
    }
    private void UpdateHorizontalVelocity()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 inputDir = forward * inputs.moveInput.y + right * inputs.moveInput.x;
        Vector3 targetHorVelocity = inputDir * stats.curTargetSpeed;

        float accel = stats.isGrounded ? settings.groundAcceleration : settings.airAcceleration * Time.fixedDeltaTime;

        stats.moveVelocity.x = Mathf.Lerp(stats.moveVelocity.x, targetHorVelocity.x, accel);
        stats.moveVelocity.z = Mathf.Lerp(stats.moveVelocity.z, targetHorVelocity.z, accel);
        rigidBody.linearVelocity = stats.moveVelocity;
    }

    private void UpdateVerticalVelocity()
    {
        stats.moveVelocity.y -= settings.gravity * Time.fixedDeltaTime;

        //Clamping Fall speed
        stats.moveVelocity.y = Mathf.Max(stats.moveVelocity.y, settings.maxFallSpeed);
    }
    private bool IsGrounded()
    {
        Vector3 bottom = capsuleCollider.bounds.center + Vector3.down * (capsuleCollider.bounds.extents.y - settings.groundCheckRadius + settings.groundCheckDistance);
        
        return Physics.CheckCapsule(capsuleCollider.bounds.center, bottom, settings.groundCheckRadius, layerData.ground);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = stats.isGrounded ? Color.green : Color.red;

        if (capsuleCollider != null)
        {
            Vector3 bottom = capsuleCollider.bounds.center + Vector3.down * (capsuleCollider.bounds.extents.y - settings.groundCheckRadius + settings.groundCheckDistance);
            Gizmos.DrawWireSphere(bottom, settings.groundCheckRadius);
        }
    }
}
