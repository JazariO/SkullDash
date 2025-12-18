using UnityEngine;
using Proselyte.DebugDrawer;
using TMPro;
public class PlayerBrain : MonoBehaviour
{
    public enum State : byte
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
        stats.tempStats.lastJumpTime = -settings.jumpBufferTime;
        stats.tempStats.curYaw = transform.eulerAngles.y;
        stats.tempStats.moveDirection = Quaternion.identity;

        float groundCheckOffset = capsuleCollider.radius * settings.groundCheckRadius;
        stats.cacheStats.groundCheckOffsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(groundCheckOffset, 0, 0),
            new Vector3(-groundCheckOffset, 0, 0),
            new Vector3(0, 0, groundCheckOffset),
            new Vector3(0, 0, -groundCheckOffset)
        };
        stats.tempStats.hitPoints = new Vector3[stats.cacheStats.groundCheckOffsets.Length];
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
        stats.tempStats.willJump = Time.time - stats.tempStats.lastJumpTime < settings.jumpBufferTime && stats.tempStats.curState != State.Jump;
    }
    private void FixedUpdate()
    {
        FixedUpdateState();
        FixedUpdateRotate();
        UpdateVelocity();
        GetGroundData();
    }

    private void OnDisable()
    {
        stats.tempStats = default;
    }
    private void SelectState()
    {   
        if ((stats.tempStats.isGrounded && stats.tempStats.willJump && stats.tempStats.slope > -1) || (stats.tempStats.moveVelocity.y > 0 && inputs.jumpHoldInput && stats.tempStats.curState != State.Fall) || stats.tempStats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (!stats.tempStats.isGrounded)
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
        else if (stats.tempStats.isGrounded)
        {
            SetState(State.Idle);
        }
        else
        {
            Debug.LogWarning($"Did not find state on game object {name}");
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
                stats.tempStats.moveVelocity.y += settings.jumpForce;
                stats.tempStats.coyoteTimeElapsed = settings.coyoteTime;
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
    private void UpdateState()
    {
        switch (stats.tempStats.curState)
        {
            case State.Idle:
            {
                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
            }
            break;
            case State.Walk:
            {
                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
                stats.tempStats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
                stats.tempStats.curTargetSpeed = settings.runSpeed;
            }
            break;
            case State.Jump:
            {
            }
            break;
            case State.Fall:
            {
                stats.tempStats.coyoteTimeElapsed += Time.deltaTime;
                stats.tempStats.coyoteJump = stats.tempStats.coyoteTimeElapsed < settings.coyoteTime && inputs.jumpPressedInput;

                if (inputs.jumpPressedInput)
                {
                    stats.tempStats.lastJumpTime = Time.time;
                }
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
                stats.tempStats.coyoteTimeElapsed = 0.0f;
                stats.tempStats.coyoteJump = false;
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
    private void UpdateVelocity()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, stats.tempStats.groundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, stats.tempStats.groundNormal).normalized;
        Vector3 moveDir = forward * inputs.moveInput.y + right * inputs.moveInput.x;

        float slopeThreshold = Vector3.Angle(stats.tempStats.groundNormal, Vector3.up) / settings.slopeAngleThreshold; // slope factor that finds the percentage of the ground angle compared to the angle threshold
        Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, stats.tempStats.groundNormal).normalized;
        float slopeDot = Vector3.Dot(moveDir, slopeDirection); // -1 to 1 value to determine the angle of the ground relative to the movement direction
        stats.tempStats.slope = stats.tempStats.curState == State.Jump || stats.tempStats.curState == State.Fall ? slopeDot * slopeThreshold : 0; // slope becomes a multiplier with 1 being the flat surface. less than one the slope is uphill, more than 1 the slope is downhill
        stats.tempStats.targetVelocity = moveDir * stats.tempStats.curTargetSpeed * (1 + stats.tempStats.slope);

        float accel = (stats.tempStats.isGrounded ? settings.groundAcceleration : settings.airAcceleration) * Time.fixedDeltaTime;

        Vector3 curMoveVelocity = Vector3.Lerp(stats.tempStats.moveVelocity, stats.tempStats.targetVelocity, accel);

        if (slopeThreshold > 1 || stats.tempStats.curState == State.Jump || stats.tempStats.curState == State.Fall)
        {
            if (!inputs.jumpHoldInput && stats.tempStats.moveVelocity.y > 0 || stats.tempStats.moveVelocity.y < 0)
            {
                stats.tempStats.curGravityMultiplier = settings.fallGravMultiplier;
            }
            else if (stats.tempStats.moveVelocity.y > 0 && stats.tempStats.moveVelocity.y < settings.antiGravApexThreshold)
            {
                stats.tempStats.curGravityMultiplier = settings.antiGravMultiplier;
            }
            else if (!stats.tempStats.isGrounded)
            {
                stats.tempStats.curGravityMultiplier = 1;
            }

            stats.tempStats.moveVelocity.y -= settings.gravity * stats.tempStats.curGravityMultiplier * Time.fixedDeltaTime;
            stats.tempStats.moveVelocity.y = Mathf.Max(stats.tempStats.moveVelocity.y, settings.maxFallSpeed);

            stats.tempStats.correctionForce = 0;
        }
        else if (stats.tempStats.slope == 0)
        {
            stats.tempStats.moveVelocity.y = 0;
            float distanceFromGround = (capsuleCollider.bounds.center.y - stats.tempStats.groundPlaneCentroid.y) * 1.1f;

            if (distanceFromGround < settings.groundCheckDistance)
            {
                stats.tempStats.correctionForce = settings.correctionSpeed * (1 - (distanceFromGround / settings.groundCheckDistance));
            }
        }
        else
        {
            stats.tempStats.moveVelocity.y = curMoveVelocity.y;
        }

        stats.tempStats.moveVelocity.x = curMoveVelocity.x;
        stats.tempStats.moveVelocity.z = curMoveVelocity.z;


        stats.tempStats.moveVelocity.y += stats.tempStats.correctionForce;
        stats.tempStats.speed = stats.tempStats.moveVelocity.magnitude;
        rigidBody.linearVelocity = stats.tempStats.moveVelocity;
        DebugDraw.WireArrow(capsuleCollider.bounds.center, capsuleCollider.bounds.center + stats.tempStats.moveVelocity, Vector3.up, color: Color.red, fromFixedUpdate: true);
    }
    private void GetGroundData()
    {
        Vector3 pointSum = Vector3.zero;
        int hitCount = 0;
        for (int i = 0; i < stats.cacheStats.groundCheckOffsets.Length; i++)
        {
            if (Physics.Raycast(capsuleCollider.bounds.center + stats.cacheStats.groundCheckOffsets[i], Vector3.down, out RaycastHit hit, settings.groundCheckDistance, layerData.ground))
            {
                pointSum += hit.point;
                stats.tempStats.hitPoints[hitCount] = hit.point;
                hitCount++;
            }
        }

        stats.tempStats.isGrounded = hitCount > 0;
        if (stats.tempStats.isGrounded)
        {
            stats.tempStats.groundPoint = pointSum / hitCount;
            stats.tempStats.groundPlaneCentroid = Vector3.zero;

            for (int i = 0; i < hitCount; i++)
            {
                stats.tempStats.groundPlaneCentroid += stats.tempStats.hitPoints[i];
            }
            stats.tempStats.groundPlaneCentroid /= hitCount;

            Vector3 planeNormal = Vector3.zero;

            if (hitCount > 1)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    Vector3 bitangent = stats.tempStats.hitPoints[i] - stats.tempStats.groundPlaneCentroid;
                    int nextIndex = (i + 1) % hitCount;
                    Vector3 tangent = stats.tempStats.hitPoints[nextIndex] - stats.tempStats.groundPlaneCentroid;
                    planeNormal += Vector3.Cross(bitangent, tangent);
                }
                stats.tempStats.groundNormal = planeNormal.normalized;
            }
            else
            {
                stats.tempStats.groundNormal = stats.tempStats.hitPoints[0].normalized;
            }
        }
        else
        {
            stats.tempStats.groundPoint = Vector3.zero;
        }


        if (!float.IsNaN(stats.tempStats.groundPlaneCentroid.x))
        {
            DebugDraw.WireQuad(stats.tempStats.groundPlaneCentroid, Quaternion.FromToRotation(Vector3.forward, stats.tempStats.groundNormal), Vector3.one, color: Color.red, fromFixedUpdate: true);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;
        Gizmos.color = Color.yellow;

        for (int i = 0; i < stats.cacheStats.groundCheckOffsets.Length; i++)
        {
            Vector3 start = capsuleCollider.bounds.center + stats.cacheStats.groundCheckOffsets[i];

            bool hit = Physics.Raycast(start, Vector3.down, settings.groundCheckDistance, layerData.ground);

            Gizmos.color = hit ? Color.red : Color.blue;
            Gizmos.DrawLine(start, start + Vector3.down * settings.groundCheckDistance);
        }
        Gizmos.color = Color.yellow;
    }
}
