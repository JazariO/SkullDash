using Proselyte.DebugDrawer;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
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
        Crouch,
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
    [SerializeField] Transform camPivot;
    private void OnValidate()
    {
        stats.tempStats.groundPlaneCheckOrigin = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y + settings.groundCheckOrigin, capsuleCollider.bounds.center.z);
    }
    private void Awake()
    {
        stats.tempStats.lastJumpTime = -settings.jumpBufferTime;
        stats.tempStats.lastSlideTime = -settings.slideBufferTime;
        stats.tempStats.curYaw = transform.eulerAngles.y;
        stats.tempStats.moveRotationQuaternion = Quaternion.identity;
        stats.tempStats.targetGroundThreshold = settings.standingGroundTheshold;

        float groundCheckOffset = capsuleCollider.radius * settings.groundCheckRadius;
        stats.cacheStats.checkOffets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(groundCheckOffset, 0, 0),
            new Vector3(-groundCheckOffset, 0, 0),
            new Vector3(0, 0, groundCheckOffset),
            new Vector3(0, 0, -groundCheckOffset)
        };
        stats.cacheStats.startCamPivotPosition = camPivot.localPosition;
        stats.tempStats.targetCamPivotPos = stats.cacheStats.startCamPivotPosition;
    }

    private void Start()
    {
        stats.tempStats.hitPoints = new Vector3[stats.cacheStats.checkOffets.Length];
        
    }
    private void Update()
    {
        ChooseState();
        UpdateState();
        UpdateRotation();
        UpdateCameraPivot();
        stats.tempStats.willJump = Time.time - stats.tempStats.lastJumpTime < settings.jumpBufferTime && stats.tempStats.curState != State.Jump;
        stats.tempStats.willSlide = Time.time - stats.tempStats.lastSlideTime < settings.slideBufferTime && stats.tempStats.curState != State.Slide;
    }
    private void FixedUpdate()
    {
        rigidBody.linearVelocity = stats.tempStats.moveVelocity;
        FixedUpdateState();
        FixedUpdateRotate();
        GetGroundData();
        //GetCeilingData();

    }
    private void OnDisable()
    {
        stats.tempStats = default;
    }
    private void ChooseState()
    {
        bool startJump = stats.tempStats.isGrounded && stats.tempStats.willJump && stats.tempStats.slope < 1;
        bool jumpingInAir = stats.tempStats.moveVelocity.y > 0 && inputs.jumpHoldInput && stats.tempStats.curState != State.Fall && stats.tempStats.correctionVelocity == Vector3.zero;

        bool stillSliding = inputs.crouchHoldInput && stats.tempStats.speed > settings.minimumSlideSpeed && stats.tempStats.curState != State.Crouch;

        if (startJump || jumpingInAir || stats.tempStats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (!stats.tempStats.isGrounded || stats.tempStats.slope > 1)
        {
            SetState(State.Fall);
        }
        else if (stats.tempStats.willSlide || stillSliding)
        {
            SetState(State.Slide);
        }
        else if (inputs.crouchHoldInput || stats.tempStats.hitCeiling)
        {
            SetState(State.Crouch);
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
                stats.tempStats.correctionVelocity = Vector3.zero;
            }
            break;
            case State.Fall:
            {
            }
            break;
            case State.Slide:
            {
                EnterCrouchState();
                stats.tempStats.moveVelocity = settings.slideSpeed * stats.tempStats.moveDirection * stats.tempStats.slopeMultiplier;
                stats.tempStats.curTargetSpeed = 0;
            }
            break;
            case State.Crouch:
            {
                EnterCrouchState();
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
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration, stats.tempStats.groundNormal, inAirState: false);
            }
            break;
            case State.Walk:
            {
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration, stats.tempStats.groundNormal, inAirState: false);
            }
            break;
            case State.Run:
            {
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration, stats.tempStats.groundNormal, inAirState: false);
            }
            break;
            case State.Jump:
            {
                AirborneVerticalVelocity();
                HorizontalVelocity(settings.airAcceleration, Vector3.up, inAirState: true);
            }
            break;
            case State.Fall:
            {
                float gravMultipler = stats.tempStats.moveVelocity.y > 0 ? settings.fallGravMultiplier : 1;
                AirborneVerticalVelocity(gravMultipler);
                HorizontalVelocity(settings.airAcceleration, Vector3.up, inAirState: true);
            }
            break;
            case State.Slide:
            {
                GroundedVerticalVelocity(settings.slideAccelation);
                HorizontalVelocity(settings.slideAccelation, stats.tempStats.groundNormal, inAirState: false);
            }
            break;
            case State.Crouch:
            {
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration, stats.tempStats.groundNormal, inAirState: false);
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
                ExitCrouchState();
            }
            break;
            case State.Crouch:
            {
                ExitCrouchState();
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
    }
    private void HorizontalVelocity(float accellation, Vector3 groundNormal, bool inAirState)
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, groundNormal).normalized;
        stats.tempStats.moveDirection = forward * inputs.moveInput.y + right * inputs.moveInput.x;

        Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
        stats.tempStats.slope = Vector3.Angle(groundNormal, Vector3.up) / settings.slopeAngleThreshold; // slope factor that finds the percentage of the ground angle compared to the angle threshold
        float slopeDot = Vector3.Dot(stats.tempStats.moveDirection, slopeDir); // -1 to 1 value to determine the angle of the ground relative to the movement direction
        stats.tempStats.slopeMultiplier = inAirState ? 1 : 1 + slopeDot * stats.tempStats.slope; // slope becomes a multiplier with 1 being the flat surface. less than one the slope is uphill, more than 1 the slope is downhill
        stats.tempStats.targetVelocity = (stats.tempStats.moveDirection * stats.tempStats.curTargetSpeed * stats.tempStats.slopeMultiplier) + stats.tempStats.correctionVelocity;


        float accel = accellation * Time.fixedDeltaTime;
        stats.tempStats.moveVelocity.x = Mathf.Lerp(stats.tempStats.moveVelocity.x, stats.tempStats.targetVelocity.x, accel);
        stats.tempStats.moveVelocity.z = Mathf.Lerp(stats.tempStats.moveVelocity.z, stats.tempStats.targetVelocity.z, accel);

        float distance = (stats.tempStats.moveVelocity.magnitude * Time.fixedDeltaTime) + capsuleCollider.radius;
        Vector3 center = capsuleCollider.transform.TransformPoint(capsuleCollider.center);
        if (Physics.Raycast(center, stats.tempStats.moveDirection, out RaycastHit wallHit, distance, layerData.ground))
        {
            stats.tempStats.moveVelocity = Vector3.ProjectOnPlane(stats.tempStats.moveVelocity, wallHit.normal);
        }
        stats.tempStats.speed = stats.tempStats.moveVelocity.magnitude;
        
        if (stats.tempStats.moveVelocity != Vector3.zero) DebugDraw.WireArrow(capsuleCollider.bounds.center, capsuleCollider.bounds.center + stats.tempStats.moveVelocity, Vector3.up, color: Color.red, fromFixedUpdate: true);
        if (stats.tempStats.targetVelocity != Vector3.zero) DebugDraw.WireArrow(capsuleCollider.bounds.center, capsuleCollider.bounds.center + stats.tempStats.targetVelocity, Vector3.up, color: Color.orange, fromFixedUpdate: true);
    }
    private void AirborneVerticalVelocity(float gravityMultiplier = 1)
    {
        float gravity = settings.gravity * Time.fixedDeltaTime * gravityMultiplier;
        stats.tempStats.moveVelocity.y -= gravity;
        stats.tempStats.moveVelocity.y = Mathf.Max(stats.tempStats.moveVelocity.y, settings.maxFallSpeed);
    }
    private void GroundedVerticalVelocity(float accellation)
    {
        float targetDistanceMargin = stats.tempStats.targetGroundThreshold - Vector3.Distance(stats.tempStats.groundPlaneCheckOrigin, stats.tempStats.groundPlaneCentroid);

        if (Mathf.Abs(targetDistanceMargin) < settings.groundThresholdBuffer)
        {
            stats.tempStats.correctionVelocity = Vector3.zero;
        }
        else
        {
            stats.tempStats.correctionVelocity = stats.tempStats.groundNormal * targetDistanceMargin * settings.correctionSpeed;
            Debug.Log(stats.tempStats.correctionVelocity);
        }
        stats.tempStats.moveVelocity.y = Mathf.Lerp(stats.tempStats.moveVelocity.y, stats.tempStats.targetVelocity.y, accellation * Time.fixedDeltaTime);
    }
    private void GetGroundData()
    {
        Vector3 pointSum = Vector3.zero;
        int hitCount = 0;
        stats.tempStats.groundPlaneCheckOrigin = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y + settings.groundCheckOrigin, capsuleCollider.bounds.center.z);

        for (int i = 0; i < stats.cacheStats.checkOffets.Length; i++)
        {
            if (Physics.Raycast(stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffets[i], Vector3.down, out RaycastHit groundNormalHit, settings.groundPlaneCheckDistance, layerData.ground))
            {
                pointSum += groundNormalHit.point;
                stats.tempStats.hitPoints[hitCount] = groundNormalHit.point;
                hitCount++;
            }
        }

        if (hitCount > 0)
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

        stats.tempStats.curGroundThreshold = Mathf.Lerp(stats.tempStats.curGroundThreshold, stats.tempStats.targetGroundThreshold, Time.fixedDeltaTime);
        for (int i = 0; i < stats.cacheStats.checkOffets.Length; i++)
        {
            if (Physics.Raycast(stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffets[i], -stats.tempStats.groundNormal, out RaycastHit groundCheckHit, settings.standingGroundTheshold, layerData.ground))
            {
                stats.tempStats.isGrounded = true;
                break;
            }
            else if (i == stats.cacheStats.checkOffets.Length - 1)
            {
                stats.tempStats.isGrounded = false;
            }
        }

        bool inAirState = stats.tempStats.curState == State.Jump || stats.tempStats.curState == State.Fall;
        Vector3 curGroundNormal = inAirState ? Vector3.up : stats.tempStats.groundNormal;
        DebugDraw.WireQuad(stats.tempStats.groundPlaneCentroid, Quaternion.FromToRotation(Vector3.forward, curGroundNormal), Vector3.one, color: inAirState ? Color.green : Color.red, fromFixedUpdate: true);
    }
    private void GetCeilingData()
    {
        Vector3 topCapsuleCenter = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y - settings.ceilingCheckOrigin, capsuleCollider.bounds.center.z);

        for (int i = 0; i < stats.cacheStats.checkOffets.Length; i++)
        {
            if (Physics.Raycast(topCapsuleCenter + stats.cacheStats.checkOffets[i], Vector3.up, out RaycastHit groundNormalHit, settings.ceilingCheckDistance, layerData.ground))
            {
                stats.tempStats.hitCeiling = true;
                break;
            }
            else if (i == stats.cacheStats.checkOffets.Length - 1)
            {
                stats.tempStats.hitCeiling = false;
            }
        }
    }
    private void EnterCrouchState()
    {
        float trueHeight = Mathf.Max(settings.slideHeight, capsuleCollider.radius * 2);
        capsuleCollider.height = trueHeight;
        float yOffset = Mathf.Min(trueHeight * 0.5f - 1, capsuleCollider.radius * 2);
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, yOffset, capsuleCollider.center.z);
        stats.tempStats.targetCamPivotPos.y = yOffset;
        stats.tempStats.targetGroundThreshold = settings.crouchGroundThreshold;
    }
    private void ExitCrouchState()
    {
        capsuleCollider.height = settings.standingHeight;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, 0, capsuleCollider.center.z);
        stats.tempStats.targetCamPivotPos.y = stats.cacheStats.startCamPivotPosition.y;
        stats.tempStats.targetGroundThreshold = settings.standingGroundTheshold;
    }
    private void UpdateCameraPivot()
    {
        stats.tempStats.curCamPivotPos = Vector3.Lerp(stats.tempStats.curCamPivotPos, stats.tempStats.targetCamPivotPos, settings.camAcceleration * Time.deltaTime);
        camPivot.transform.localPosition = stats.tempStats.curCamPivotPos;
    }
    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;
        stats.tempStats.groundPlaneCheckOrigin = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y + settings.groundCheckOrigin, capsuleCollider.bounds.center.z);
        for (int i = 0; i < stats.cacheStats.checkOffets.Length; i++)
        {
            Vector3 start = stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffets[i];

            bool hit = Physics.Raycast(start, Vector3.down, settings.groundPlaneCheckDistance, layerData.ground);

            Gizmos.color = hit ? Color.green : Color.red;
            Gizmos.DrawLine(start, start + Vector3.down * settings.groundPlaneCheckDistance);
        }
        Vector3 editorGroundNormal = Application.isPlaying ? -stats.tempStats.groundNormal : Vector3.down;
        for (int i = 0; i < stats.cacheStats.checkOffets.Length; i++)
        {
            Vector3 start = stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffets[i];

            bool hit = Physics.Raycast(start, editorGroundNormal, settings.standingGroundTheshold, layerData.ground);

            Gizmos.color = hit ? Color.cyan : Color.orange;
            Gizmos.DrawLine(start, start + editorGroundNormal * settings.standingGroundTheshold);
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 center = capsuleCollider.transform.TransformPoint(capsuleCollider.center);

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
