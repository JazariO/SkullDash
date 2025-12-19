using Proselyte.DebugDrawer;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
public class PlayerBrain : MonoBehaviour
{
    public enum State : byte
    { 
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        Slide,
    }

    [Header("Scriptable Objects")]
    [SerializeField] GameEventDataSO gameEventData;
    [SerializeField] PlayerSettingsSO settings;
    [SerializeField] PlayerStatsSO stats;
    [SerializeField] InputDataSO inputs;
    [SerializeField] LayerSettingsSO layerData;
    [SerializeField] UserSettingsSO userSettings;

    [Header("Components")]
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Camera cam;


    private void OnValidate()
    {
        stats.tempStats.bottomCapsuleCenter = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y + settings.groundCheckOrigin, capsuleCollider.bounds.center.z);
    }
    private void Awake()
    {
        stats.tempStats.lastJumpTime = -settings.jumpBufferTime;
        stats.tempStats.lastSlideTime = -settings.slideBufferTime;
        stats.tempStats.curYaw = transform.eulerAngles.y;
        stats.tempStats.moveRotationQuaternion = Quaternion.identity;
        stats.tempStats.curStepTheshold = settings.stepThreshold;

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
    private void Update()
    {
        ChooseState();
        UpdateState();
        UpdateRotation();
        stats.tempStats.willJump = Time.time - stats.tempStats.lastJumpTime < settings.jumpBufferTime && stats.tempStats.curState != State.Jump;
        stats.tempStats.willSlide = Time.time - stats.tempStats.lastSlideTime < settings.slideBufferTime && stats.tempStats.curState != State.Slide;
    }
    private void FixedUpdate()
    {
        rigidBody.linearVelocity = stats.tempStats.moveVelocity;
        FixedUpdateState();
        FixedUpdateRotate();
        GetGroundData();

    }
    private void OnDisable()
    {
        stats.tempStats = default;
    }
    private void ChooseState()
    {   
        if ((stats.tempStats.isGrounded && stats.tempStats.willJump && stats.tempStats.slope < 1) || (stats.tempStats.moveVelocity.y > 0 && inputs.jumpHoldInput && stats.tempStats.curState != State.Fall && stats.tempStats.correctionForce == 0) || stats.tempStats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (!stats.tempStats.isGrounded || stats.tempStats.slope > 1)
        {
            SetState(State.Fall);
        }
        else if (stats.tempStats.willSlide || inputs.crouchHoldInput)
        {
            SetState(State.Slide);
        }
        else if (inputs.sprintInput && inputs.moveInput != Vector2.zero)
        {
            SetState(State.Run);
        }
        else if (inputs.moveInput != Vector2.zero)
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
                stats.tempStats.moveVelocity.y = settings.jumpForce;
                stats.tempStats.coyoteTimeElapsed = settings.coyoteTime;
            }
            break;
            case State.Fall:
            {
            }
            break;
            case State.Slide:
            {
                float trueHeight = Mathf.Max(settings.slideHeight, capsuleCollider.radius * 2);
                capsuleCollider.height = trueHeight;
                float yOffset = Mathf.Min(trueHeight * 0.5f - 1, capsuleCollider.radius * 2);
                capsuleCollider.center = new Vector3(capsuleCollider.center.x, yOffset, capsuleCollider.center.z);
                cam.transform.localPosition = new Vector3(0, yOffset, 0);

                stats.tempStats.moveVelocity.x = settings.slideSpeed * stats.tempStats.moveDirection.x;
                stats.tempStats.moveVelocity.z = settings.slideSpeed * stats.tempStats.moveDirection.z;
                stats.tempStats.curTargetSpeed = settings.crouchSpeed;
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
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
            }
            break;
            case State.Walk:
            {
                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
                stats.tempStats.curTargetSpeed = settings.walkSpeed;
            }
            break;
            case State.Run:
            {
                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
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

                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
            }
            break;
            case State.Slide:
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
                GroundedVerticalVelocity();
                HorizontalVelocity(settings.groundAcceleration);
            }
            break;
            case State.Walk:
            {
                GroundedVerticalVelocity();
                HorizontalVelocity(settings.groundAcceleration);
            }
            break;
            case State.Run:
            {
                GroundedVerticalVelocity();
                HorizontalVelocity(settings.groundAcceleration);
            }
            break;
            case State.Jump:
            {
                AirborneVerticalVelocity();
                HorizontalVelocity(settings.airAcceleration);
            }
            break;
            case State.Fall:
            {
                AirborneVerticalVelocity(gravityMultiplier: settings.fallGravMultiplier);
                HorizontalVelocity(settings.airAcceleration);
            }
            break;
            case State.Slide:
            {
                GroundedVerticalVelocity();
                HorizontalVelocity(settings.slideAccelation);
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
                stats.tempStats.moveVelocity.y = 0;
            }
            break;
            case State.Slide:
            {
                capsuleCollider.height = settings.standingHeight;
                capsuleCollider.center = Vector3.zero;
                cam.transform.localPosition = Vector3.zero;
            }
            break;
        }
    }
    private void FixedUpdateRotate()
    {
        rigidBody.MoveRotation(stats.tempStats.moveRotationQuaternion);
    }
    private void UpdateRotation()
    {
        float yaw = inputs.lookInput.x * userSettings.sensitivity;
        stats.tempStats.curYaw += yaw;
        stats.tempStats.moveRotationQuaternion = Quaternion.Euler(0.0f, stats.tempStats.curYaw, 0.0f);
        float pitch = inputs.lookInput.y * userSettings.sensitivity;
        stats.tempStats.curPitch -= pitch;
        stats.tempStats.curPitch = Mathf.Clamp(stats.tempStats.curPitch, -settings.clampedPitch, settings.clampedPitch);
        cam.transform.localRotation = Quaternion.Euler(stats.tempStats.curPitch, 0, 0);
    }
    private void HorizontalVelocity(float accellation)
    {
        bool inAirState = stats.tempStats.curState == State.Jump || stats.tempStats.curState == State.Fall;

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, stats.tempStats.groundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, stats.tempStats.groundNormal).normalized;
        stats.tempStats.moveDirection = forward * inputs.moveInput.y + right * inputs.moveInput.x;

        Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, stats.tempStats.groundNormal).normalized;
        stats.tempStats.slope = Vector3.Angle(stats.tempStats.groundNormal, Vector3.up) / settings.slopeAngleThreshold; // slope factor that finds the percentage of the ground angle compared to the angle threshold
        float slopeDot = Vector3.Dot(stats.tempStats.moveDirection, slopeDir); // -1 to 1 value to determine the angle of the ground relative to the movement direction
        float slope = inAirState ? 1 : 1 + slopeDot * stats.tempStats.slope; // slope becomes a multiplier with 1 being the flat surface. less than one the slope is uphill, more than 1 the slope is downhill
        stats.tempStats.targetVelocity = stats.tempStats.moveDirection * stats.tempStats.curTargetSpeed * slope;

        float accel = accellation * Time.fixedDeltaTime;

        stats.tempStats.moveVelocity.x = Mathf.Lerp(stats.tempStats.moveVelocity.x, stats.tempStats.targetVelocity.x, accel);
        stats.tempStats.moveVelocity.z = Mathf.Lerp(stats.tempStats.moveVelocity.z, stats.tempStats.targetVelocity.z, accel);
        stats.tempStats.speed = stats.tempStats.moveVelocity.magnitude;

       // DebugDraw.WireArrow(capsuleCollider.bounds.center, capsuleCollider.bounds.center + stats.tempStats.moveVelocity, Vector3.up, color: Color.red, fromFixedUpdate: true);
    }
    private void AirborneVerticalVelocity(float gravityMultiplier = 1)
    {
        float gravity = settings.gravity * Time.fixedDeltaTime * gravityMultiplier;
        stats.tempStats.moveVelocity.y -= gravity;
        stats.tempStats.moveVelocity.y = Mathf.Max(stats.tempStats.moveVelocity.y, settings.maxFallSpeed);
    }

    private void GroundedVerticalVelocity()
    {
        if (stats.tempStats.slope == 0)
        {
            float distanceFromGround = stats.tempStats.bottomCapsuleCenter.y - stats.tempStats.groundPlaneCentroid.y;
            if (distanceFromGround < settings.stepThreshold - 0.1)
            {
                stats.tempStats.correctionForce = settings.correctionSpeed * (1 - (distanceFromGround / settings.stepThreshold));
                stats.tempStats.moveVelocity.y += stats.tempStats.correctionForce;
            }
            else
            {
                stats.tempStats.correctionForce = 0;
                stats.tempStats.moveVelocity.y = 0;
            }
        }
        else
        {
            stats.tempStats.moveVelocity.y = Mathf.Lerp(stats.tempStats.moveVelocity.y, stats.tempStats.targetVelocity.y, settings.groundAcceleration * Time.fixedDeltaTime);
        }

    }
    private void GetGroundData()
    {
        Vector3 pointSum = Vector3.zero;
        int hitCount = 0;
        stats.tempStats.bottomCapsuleCenter = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y + settings.groundCheckOrigin, capsuleCollider.bounds.center.z);

        for (int i = 0; i < stats.cacheStats.groundCheckOffsets.Length; i++)
        {
            if (Physics.Raycast(stats.tempStats.bottomCapsuleCenter + stats.cacheStats.groundCheckOffsets[i], Vector3.down, out RaycastHit hit, settings.groundCheckDistance, layerData.ground))
            {
                pointSum += hit.point;
                stats.tempStats.hitPoints[hitCount] = hit.point;
                hitCount++;
            }

            if (Physics.Raycast(stats.tempStats.bottomCapsuleCenter + stats.cacheStats.groundCheckOffsets[i], Vector3.down, out RaycastHit groundHit, settings.groundCheckDistance, layerData.ground))
            {

            }
        }
        if (stats.tempStats.isGrounded)
        {
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
        DebugDraw.WireQuad(stats.tempStats.groundPlaneCentroid, Quaternion.FromToRotation(Vector3.forward, stats.tempStats.groundNormal), Vector3.one, color: Color.red, fromFixedUpdate: true);
    }
    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;

        for (int i = 0; i < stats.cacheStats.groundCheckOffsets.Length; i++)
        {
            Vector3 start = stats.tempStats.bottomCapsuleCenter + stats.cacheStats.groundCheckOffsets[i];

            bool hit = Physics.Raycast(start, Vector3.down, settings.groundCheckDistance, layerData.ground);

            Gizmos.color = hit ? Color.green : Color.red;
            Gizmos.DrawLine(start, start + Vector3.down * settings.groundCheckDistance);
        }

        Gizmos.color = stats.tempStats.isGrounded ? Color.cyan : Color.orange;
        Gizmos.DrawLine(stats.tempStats.bottomCapsuleCenter, stats.tempStats.bottomCapsuleCenter + Vector3.down * settings.stepThreshold);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Transform t = capsuleCollider.transform;

        Vector3 center = t.TransformPoint(capsuleCollider.center);

        float radius = capsuleCollider.radius;
        float height = Mathf.Max(capsuleCollider.height, radius * 2f);

        float halfHeight = height * 0.5f - radius;

        Vector3 top = center + Vector3.up * halfHeight;
        Vector3 bottom = center - Vector3.up * halfHeight;

        // Draw hemispheres
        Gizmos.DrawSphere(top, radius);
        Gizmos.DrawSphere(bottom, radius);
    }
}
