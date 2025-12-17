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
        stats.tempStats.curState = State.Idle;
    }
    private void Update()
    {
        SelectState();
        UpdateState();
        UpdateRotation();
        stats.tempStats.willJump = Time.time - stats.tempStats.lastJumpTime <= settings.jumpBufferTime && stats.tempStats.curState != State.Jump;
    }
    private void FixedUpdate()
    {
        FixedUpdateState();
        FixedUpdateRotate();
        UpdateHorizontalVelocity();
        UpdateVerticalVelocity();
        stats.tempStats.isGrounded = IsGrounded();
    }
    private void SelectState()
    {   
        if ((stats.tempStats.isGrounded && stats.tempStats.willJump) || stats.tempStats.moveVelocity.y > 0 || stats.tempStats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (stats.tempStats.moveVelocity.y <= 0 && !stats.tempStats.isGrounded)
        {
            SetState(State.Fall);
        }
        else if (inputs.sprintInput && inputs.moveInput != Vector2.zero && stats.tempStats.isGrounded)
        {
            SetState(State.Run);
        }
        else if (stats.tempStats.isGrounded && inputs.moveInput != Vector2.zero)
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
        if (stats.tempStats.curState == newState) return;
        ExitState();
        stats.tempStats.curState = newState;
        EnterState();
    }
    private void EnterState()
    {
        switch (stats.tempStats.curState)
        {
            case State.Idle:
            {

            }
            break;
            case State.Walk:
            {
                stats.tempStats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                stats.tempStats.curTargetSpeed = settings.runSpeed;
            }
            break;
            case State.Jump:
            {
                stats.tempStats.moveVelocity.y = settings.jumpSpeed;
                stats.tempStats.coyoteTimeElapsed = float.MaxValue;
            }
            break;
            case State.Fall:
            {
                stats.tempStats.coyoteTimeElapsed += Time.deltaTime;
                stats.tempStats.coyoteJump = stats.tempStats.coyoteTimeElapsed < settings.coyoteTime && inputs.jumpInput;

                if (inputs.jumpInput)
                {
                    stats.tempStats.lastJumpTime = Time.time;
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
        switch (stats.tempStats.curState)
        {
            case State.Idle:
            {
                if (inputs.jumpInput) stats.tempStats.lastJumpTime = Time.time;
            }
            break;
            case State.Walk:
            {
                if (inputs.jumpInput) stats.tempStats.lastJumpTime = Time.time;
                stats.tempStats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                if (inputs.jumpInput) stats.tempStats.lastJumpTime = Time.time;
                stats.tempStats.curTargetSpeed = settings.runSpeed;
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
        switch (stats.tempStats.curState)
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
        switch (stats.tempStats.curState)
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
        rigidBody.MoveRotation(stats.tempStats.moveDirection);
    }

    private void UpdateRotation()
    {
        float yaw = inputs.lookInput.x * userSettings.sensitivity;
        stats.tempStats.curYaw += yaw;
        stats.tempStats.moveDirection = Quaternion.Euler(0.0f, stats.tempStats.curYaw, 0.0f);
        float pitch = inputs.lookInput.y * userSettings.sensitivity;
        stats.tempStats.curPitch -= pitch;
        stats.tempStats.curPitch = Mathf.Clamp(stats.tempStats.curPitch, -settings.clampedPitch, settings.clampedPitch);
        cam.transform.localRotation = Quaternion.Euler(stats.tempStats.curPitch, 0, 0);
    }
    private void UpdateHorizontalVelocity()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 inputDir = forward * inputs.moveInput.y + right * inputs.moveInput.x;
        Vector3 targetHorVelocity = inputDir * stats.tempStats.curTargetSpeed;

        float accel = stats.tempStats.isGrounded ? settings.groundAcceleration : settings.airAcceleration * Time.fixedDeltaTime;

        stats.tempStats.moveVelocity.x = Mathf.Lerp(stats.tempStats.moveVelocity.x, targetHorVelocity.x, accel);
        stats.tempStats.moveVelocity.z = Mathf.Lerp(stats.tempStats.moveVelocity.z, targetHorVelocity.z, accel);
        rigidBody.linearVelocity = stats.tempStats.moveVelocity;
    }

    private void UpdateVerticalVelocity()
    {
        stats.tempStats.moveVelocity.y -= settings.gravity * Time.fixedDeltaTime;

        //Clamping Fall speed
        stats.tempStats.moveVelocity.y = Mathf.Max(stats.tempStats.moveVelocity.y, settings.maxFallSpeed);
    }
    private bool IsGrounded()
    {
        Vector3 bottom = capsuleCollider.bounds.center + Vector3.down * (capsuleCollider.bounds.extents.y - settings.groundCheckRadius + settings.groundCheckDistance);
        
        return Physics.CheckCapsule(capsuleCollider.bounds.center, bottom, settings.groundCheckRadius, layerData.ground);
    }

    private void OnApplicationQuit()
    {
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = stats.tempStats.isGrounded ? Color.green : Color.red;

        if (capsuleCollider != null)
        {
            Vector3 bottom = capsuleCollider.bounds.center + Vector3.down * (capsuleCollider.bounds.extents.y - settings.groundCheckRadius + settings.groundCheckDistance);
            Gizmos.DrawWireSphere(bottom, settings.groundCheckRadius);
        }
    }
}
