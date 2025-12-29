using Proselyte.DebugDrawer;
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
        Crouch,
        SlideJump,
    }

    [Header("Scriptable Objects")]
    [SerializeField] GameEventDataSO gameEventData;
    [SerializeField] PlayerSettingsSO settings;
    [SerializeField] PlayerStatsSO stats;
    [SerializeField] InputDataSO inputs;
    [SerializeField] LayerSettingsSO layerData;
    [SerializeField] UserSettingsSO userSettings;

    [Header("Components")]
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Transform camPivot;

    Collider[] colliders;
    private void OnValidate()
    {
        float[] minMaxCapsuleColliderYPos = GetCapsuleBottomAndTopY();
        stats.tempStats.bottomCapsuleYPos = minMaxCapsuleColliderYPos[0] + capsuleCollider.radius;
        stats.tempStats.groundPlaneCheckOrigin = new Vector3(capsuleCollider.transform.position.x, stats.tempStats.bottomCapsuleYPos, capsuleCollider.transform.position.z);
        stats.tempStats.topCapsuleYPos = minMaxCapsuleColliderYPos[1] - capsuleCollider.radius;

        float groundCheckOffset = capsuleCollider.radius * settings.groundCheckRadius;
        stats.cacheStats.checkOffsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(groundCheckOffset, 0, 0),
            new Vector3(-groundCheckOffset, 0, 0),
            new Vector3(0, 0, groundCheckOffset),
            new Vector3(0, 0, -groundCheckOffset)
        };

    }
    private void Awake()
    {

        stats.tempStats.lastJumpTime = -settings.jumpBufferTime;
        stats.tempStats.lastSlideTime = -settings.slideBufferTime;
        stats.tempStats.curYaw = transform.eulerAngles.y;
        stats.tempStats.moveRotationQuaternion = Quaternion.identity;
        stats.tempStats.targetGroundThreshold = settings.standingGroundTheshold;
        stats.tempStats.standingHeight = capsuleCollider.height;
        stats.tempStats.targetCamPivotPos = stats.cacheStats.startCamPivotPosition;


        stats.cacheStats.startCamPivotPosition = camPivot.localPosition;
        stats.cacheStats.startCapsulePosition = capsuleCollider.transform.localPosition;
        stats.cacheStats.startCapsuleHeight = capsuleCollider.height;
        stats.cacheStats.crouchThreshold = capsuleCollider.radius + settings.groundThresholdBuffer;
        colliders = new Collider[settings.maxCollisionCount];
        OnValidate();
    }
    private void Start()
    {
        stats.tempStats.hitPoints = new RaycastHit[stats.cacheStats.checkOffsets.Length];
    }
    private void Update()
    {
        ChooseState();
        UpdateState();
        UpdateRotation();
        UpdateCameraPivot();
        stats.tempStats.willJump = Time.time - stats.tempStats.lastJumpTime < settings.jumpBufferTime && stats.tempStats.curState != State.Jump;
        stats.tempStats.willSlide = Time.time - stats.tempStats.lastSlideTime < settings.slideBufferTime && stats.tempStats.curState != State.Slide;

        float[] minMaxCapsuleColliderYPos = GetCapsuleBottomAndTopY();
        stats.tempStats.bottomCapsuleYPos = minMaxCapsuleColliderYPos[0] + capsuleCollider.radius;
        stats.tempStats.topCapsuleYPos = minMaxCapsuleColliderYPos[1] - capsuleCollider.radius;
    }
    private void FixedUpdate()
    {
        transform.position += stats.tempStats.moveVelocity * Time.fixedDeltaTime;
        FixedUpdateState();
        FixedUpdateRotate();
        GetGroundData();
        GetCeilingCheck();
    }
    private void OnDisable()
    {
        stats.tempStats = default;
    }
    private void ChooseState()
    {
        bool startJump = stats.tempStats.isGrounded && stats.tempStats.willJump && Vector3.Angle(stats.tempStats.hitPoints[stats.tempStats.mostUpHitIndex].normal, Vector3.up) < settings.slopeAngleThreshold;
        bool jumpingInAir = stats.tempStats.moveVelocity.y > 0 && inputs.jumpHoldInput;

        bool startSlide = stats.tempStats.isGrounded && stats.tempStats.willSlide;
        bool stillSliding = inputs.crouchHoldInput && stats.tempStats.speed > settings.minimumSlideSpeed && stats.tempStats.curState != State.Crouch;
        if ((stillSliding && startJump) || (jumpingInAir && stats.tempStats.curState == State.SlideJump))
        {
            SetState(State.SlideJump);
        }
        else if (startJump || (jumpingInAir && stats.tempStats.curState == State.Jump) || stats.tempStats.coyoteJump)
        {
            SetState(State.Jump);
        }
        else if (!stats.tempStats.isGrounded)
        {
            SetState(State.Fall);
        }
        else if (startSlide || stillSliding)
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
       if (stats.tempStats.curState == State.SlideJump) Debug.Log(stats.tempStats.isGrounded + " | " + stats.tempStats.moveVelocity.y + " | " + inputs.jumpHoldInput);
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
                EnterJumpState();
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
            case State.SlideJump:
            {
                EnterJumpState();
                stats.tempStats.curTargetSpeed = settings.slideJumpSpeed;
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
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
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
                if (inputs.jumpPressedInput) stats.tempStats.lastJumpTime = Time.time;
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
                HorizontalVelocity(settings.groundAcceleration, inAirState: false);
            }
            break;
            case State.Walk:
            {
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration, inAirState: false);
            }
            break;
            case State.Run:
            {
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration,inAirState: false);
            }
            break;
            case State.Jump:
            {
                AirborneVerticalVelocity();
                HorizontalVelocity(settings.airAcceleration, inAirState: true);
            }
            break;
            case State.Fall:
            {
                float gravMultipler = stats.tempStats.moveVelocity.y > 0 ? settings.fallGravMultiplier : 1;
                AirborneVerticalVelocity(gravMultipler);
                HorizontalVelocity(settings.airAcceleration, inAirState: true);
            }
            break;
            case State.Slide:
            {
                GroundedVerticalVelocity(settings.slideAccelation);
                HorizontalVelocity(settings.slideAccelation, inAirState: false);
            }
            break;
            case State.Crouch:
            {
                GroundedVerticalVelocity(settings.groundAcceleration);
                HorizontalVelocity(settings.groundAcceleration, inAirState: false);
            }
            break;
            case State.SlideJump:
            {
                AirborneVerticalVelocity();
                HorizontalVelocity(settings.airAcceleration, inAirState: true);
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
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
            }
            break;
            case State.Walk:
            {
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;

            }
            break;
            case State.Run:
            {
                if (inputs.crouchPressedInput) stats.tempStats.lastSlideTime = Time.time;
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
            case State.SlideJump:
            {

            }
            break;
        }
    }
    private void FixedUpdateRotate()
    {
        transform.rotation = stats.tempStats.moveRotationQuaternion;
    }
    private void UpdateRotation()
    {
        float yaw = inputs.lookInput.x * userSettings.sensitivity;
        stats.tempStats.curYaw += yaw;
        stats.tempStats.moveRotationQuaternion = Quaternion.Euler(0.0f, stats.tempStats.curYaw, 0.0f);
    }
    private void HorizontalVelocity(float accellation, bool inAirState)
    {
        //Move Direction
        Vector3 dirGroundNormal = inAirState ? Vector3.up : stats.tempStats.groundNormal;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, dirGroundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, dirGroundNormal).normalized;
        stats.tempStats.moveDirection = forward * inputs.moveInput.y + right * inputs.moveInput.x;


        //Handling Slopes
        Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, stats.tempStats.groundNormal).normalized;
        stats.tempStats.slope = Vector3.Angle(stats.tempStats.groundNormal, Vector3.up) / settings.slopeAngleThreshold; // slope factor that finds the percentage of the ground angle compared to the angle threshold
        float slopeDot = Vector3.Dot(stats.tempStats.moveDirection, slopeDir); // -1 to 1 value to determine the angle of the ground relative to the movement direction
        stats.tempStats.slopeMultiplier = inAirState ? 1 : 1 + slopeDot * stats.tempStats.slope; // when slope is 0 the surface is flat. less than one the slope is uphill, more than 1 the slope is downhill
        stats.tempStats.targetVelocity = (stats.tempStats.moveDirection * stats.tempStats.curTargetSpeed * stats.tempStats.slopeMultiplier);


        if (Vector3.Angle(stats.tempStats.hitPoints[stats.tempStats.mostUpHitIndex].normal, Vector3.up) > settings.slopeAngleThreshold)
        {
            Vector3 slopeVelocity = slopeDir * stats.tempStats.slope;
            stats.tempStats.targetVelocity += slopeVelocity;
        }

        //Manually Handle Collisions
        Vector3 distance = stats.tempStats.moveVelocity * Time.fixedDeltaTime;
        Vector3 center = transform.position + distance + transform.rotation * capsuleCollider.transform.localPosition;
        float height = Mathf.Max(capsuleCollider.height, capsuleCollider.radius * 2f);
        float halfHeight = height * 0.5f - capsuleCollider.radius;
        Vector3 top = center + capsuleCollider.transform.up * halfHeight;
        Vector3 bottom = center - capsuleCollider.transform.up * halfHeight;


        stats.tempStats.wallNormal = Vector3.zero;
        int collisionCount = Physics.OverlapCapsuleNonAlloc(top, bottom, capsuleCollider.radius, colliders, layerData.ground, QueryTriggerInteraction.Ignore);
        if (collisionCount > 0)
        {
            Vector3 accumWallVectors = Vector3.zero;

            for (int i = 0; i < collisionCount; i++)
            {
                Collider col = colliders[i];
                if (Physics.ComputePenetration(capsuleCollider, capsuleCollider.transform.position, capsuleCollider.transform.rotation, col, col.transform.position, col.transform.rotation, out Vector3 dir, out float dist))
                {
                    accumWallVectors += dir * dist;
                    Rigidbody hitBody = col.attachedRigidbody;
                    if (hitBody != null && !hitBody.isKinematic)
                    {
                        Vector3 relativeVelocity = Vector3.Project(hitBody.linearVelocity, dir);
                        float massRatio = hitBody.mass / (settings.mass + hitBody.mass);
                        stats.tempStats.moveVelocity += relativeVelocity * massRatio;
                    }
                }
            }
            if (accumWallVectors != Vector3.zero)
            {
                stats.tempStats.wallNormal = Vector3.ProjectOnPlane(accumWallVectors, stats.tempStats.groundNormal).normalized;
                //stats.tempStats.moveVelocity -= Vector3.Project(stats.tempStats.moveVelocity, stats.tempStats.wallNormal);
                stats.tempStats.targetVelocity -= Vector3.Project(stats.tempStats.targetVelocity, stats.tempStats.wallNormal);
                transform.position += accumWallVectors;
            }
        }

        //Calculate non collision velocity
        float accel = accellation * Time.fixedDeltaTime;
        stats.tempStats.moveVelocity.x = Mathf.Lerp(stats.tempStats.moveVelocity.x, stats.tempStats.targetVelocity.x, accel);
        stats.tempStats.moveVelocity.z = Mathf.Lerp(stats.tempStats.moveVelocity.z, stats.tempStats.targetVelocity.z, accel);
        stats.tempStats.speed = stats.tempStats.moveVelocity.magnitude;
        
        if (stats.tempStats.moveVelocity != Vector3.zero) DebugDraw.WireArrow(capsuleCollider.transform.position, capsuleCollider.transform.position + stats.tempStats.moveVelocity, Vector3.up, color: Color.red, fromFixedUpdate: true);
        if (stats.tempStats.targetVelocity != Vector3.zero) DebugDraw.WireArrow(capsuleCollider.transform.position, capsuleCollider.transform.position + stats.tempStats.targetVelocity, Vector3.up, color: Color.orange, fromFixedUpdate: true);
    }
    private void AirborneVerticalVelocity(float gravityMultiplier = 1)
    {
        if (stats.tempStats.hitCeiling && stats.tempStats.moveVelocity.y > 0)
        {
            stats.tempStats.moveVelocity.y = 0;
            return;
        }
        float gravity = settings.gravity * Time.fixedDeltaTime * gravityMultiplier;
        stats.tempStats.moveVelocity.y -= gravity;
        stats.tempStats.moveVelocity.y = Mathf.Max(stats.tempStats.moveVelocity.y, settings.maxFallSpeed);
    }
    private void GroundedVerticalVelocity(float accellation)
    {
        float signedDistance = Vector3.Dot(stats.tempStats.groundPlaneCheckOrigin - stats.tempStats.groundPlaneCentroid, stats.tempStats.groundNormal);
        float distanceMargin = signedDistance - stats.tempStats.targetGroundThreshold;

        if (Mathf.Abs(distanceMargin) > settings.groundThresholdBuffer)
        {
            Debug.Log("adjusted postion");
            //transform.position = -stats.tempStats.groundNormal * distanceMargin * settings.correctionSpeed * Time.fixedDeltaTime;
        }
        stats.tempStats.moveVelocity.y = Mathf.Lerp(stats.tempStats.moveVelocity.y, stats.tempStats.targetVelocity.y, accellation * Time.fixedDeltaTime);
    }
    private void GetCeilingCheck()
    {
        stats.tempStats.ceilingCheckOrigin = new Vector3(capsuleCollider.transform.position.x, transform.position.y + settings.ceilingCheckOrigin, capsuleCollider.transform.position.z);

        float height = Mathf.Max(stats.tempStats.standingHeight, settings.ceilingCheckRadius * 2f);
        float halfHeight = height * 0.5f - settings.ceilingCheckRadius;

        Vector3 top = stats.tempStats.ceilingCheckOrigin + Vector3.up * halfHeight;
        Vector3 bottom = stats.tempStats.ceilingCheckOrigin - Vector3.up * halfHeight;

        if (Physics.CheckCapsule(top, bottom, settings.ceilingCheckRadius, layerData.ground))
        {
            stats.tempStats.hitCeiling = true;
        }
        else
        {
            stats.tempStats.hitCeiling = false;
        }
        DebugDraw.WireCapsule(top, bottom, settings.ceilingCheckRadius, color: stats.tempStats.hitCeiling ? Color.red : Color.green, fromFixedUpdate: true);
    }
    private void GetGroundData()
    {
        Vector3 pointSum = Vector3.zero;
        int hitCount = 0;
        stats.tempStats.groundPlaneCheckOrigin = new Vector3(capsuleCollider.transform.position.x, stats.tempStats.bottomCapsuleYPos, capsuleCollider.transform.position.z);
        float mostUpDot = -Mathf.Infinity;
        for (int i = 0; i < stats.cacheStats.checkOffsets.Length; i++)
        {
            if (Physics.Raycast(stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffsets[i], Vector3.down, out RaycastHit groundHit, settings.groundPlaneCheckDistance, layerData.ground))
            {
                stats.tempStats.hitPoints[hitCount] = groundHit;

                float dot = Vector3.Dot(groundHit.normal, Vector3.up);

                if (dot > mostUpDot)
                {
                    mostUpDot = dot;
                    stats.tempStats.mostUpHitIndex = hitCount;
                }
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            stats.tempStats.groundPlaneCentroid = Vector3.zero;

            for (int i = 0; i < hitCount; i++)
            {
                stats.tempStats.groundPlaneCentroid += stats.tempStats.hitPoints[i].point;
            }
            stats.tempStats.groundPlaneCentroid /= hitCount;


            if (hitCount > 1)
            {
                Vector3 planeNormal = Vector3.zero;
                for (int i = 0; i < hitCount; i++)
                {
                    Vector3 bitangent = stats.tempStats.hitPoints[i].point - stats.tempStats.groundPlaneCentroid;
                    int nextIndex = (i + 1) % hitCount;
                    Vector3 tangent = stats.tempStats.hitPoints[nextIndex].point - stats.tempStats.groundPlaneCentroid;
                    planeNormal += Vector3.Cross(bitangent, tangent);
                }
                stats.tempStats.groundNormal = planeNormal.normalized;
            }
            else
            {
                stats.tempStats.groundNormal = stats.tempStats.hitPoints[0].point.normalized;
            }
        }

        for (int i = 0; i < stats.cacheStats.checkOffsets.Length; i++)
        {
            if (Physics.Raycast(stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffsets[i], -stats.tempStats.groundNormal, out RaycastHit groundCheckHit, stats.tempStats.targetGroundThreshold, layerData.ground))
            {
                stats.tempStats.isGrounded = true;
                break;
            }
            else if (i == stats.cacheStats.checkOffsets.Length - 1)
            {
                stats.tempStats.isGrounded = false;
            }
        }

        bool inAirState = stats.tempStats.curState == State.Jump || stats.tempStats.curState == State.Fall;
        Vector3 curGroundNormal = inAirState ? Vector3.up : stats.tempStats.groundNormal;
        DebugDraw.WireQuad(stats.tempStats.groundPlaneCentroid, curGroundNormal, Vector3.one, color: inAirState ? Color.green : Color.red, fromFixedUpdate: true);
    }
    private void EnterCrouchState()
    {
        float trueHeight = Mathf.Max(settings.slideHeight, capsuleCollider.radius * 2);
        capsuleCollider.height = trueHeight;
        capsuleCollider.transform.localPosition = new Vector3(0, capsuleCollider.radius, 0);
        stats.tempStats.targetCamPivotPos.y = stats.tempStats.bottomCapsuleYPos;
        stats.tempStats.targetGroundThreshold = stats.cacheStats.crouchThreshold;
    }
    private void ExitCrouchState()
    {
        capsuleCollider.height = stats.cacheStats.startCapsuleHeight;
        capsuleCollider.transform.localPosition = stats.cacheStats.startCapsulePosition;
        stats.tempStats.targetCamPivotPos.y = stats.cacheStats.startCamPivotPosition.y;
        stats.tempStats.targetGroundThreshold = settings.standingGroundTheshold;
    }
    private void EnterJumpState()
    {
        stats.tempStats.moveVelocity.y = settings.jumpForce;
        stats.tempStats.coyoteTimeElapsed = settings.coyoteTime;
    }
    private void UpdateCameraPivot()
    {
        stats.tempStats.curCamPivotPos = Vector3.Lerp(stats.tempStats.curCamPivotPos, stats.tempStats.targetCamPivotPos, settings.camAcceleration * Time.deltaTime);
        camPivot.transform.localPosition = stats.tempStats.curCamPivotPos;
    }
    private float[] GetCapsuleBottomAndTopY()
    {

        float halfHeight = Mathf.Max(capsuleCollider.height * 0.5f - capsuleCollider.radius, 0f );

        float offset = halfHeight + capsuleCollider.radius;

        float bottomY = capsuleCollider.transform.position.y - offset;
        float topY = capsuleCollider.transform.position.y + offset;

        return new float[] { bottomY, topY };
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (capsuleCollider == null) return;
        DrawPlayer();
        DrawGroundPlaneCheck();
        DrawGroundThreshold();
    }

    private void DrawGroundPlaneCheck()
    {
        stats.tempStats.groundPlaneCheckOrigin = new Vector3(capsuleCollider.transform.position.x, stats.tempStats.bottomCapsuleYPos, capsuleCollider.transform.position.z);
        for (int i = 0; i < stats.cacheStats.checkOffsets.Length; i++)
        {
            Vector3 start = stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffsets[i];

            bool hit = Physics.Raycast(start, Vector3.down, settings.groundPlaneCheckDistance, layerData.ground);

            Gizmos.color = hit ? Color.green : Color.red;
            Gizmos.DrawLine(start, start + Vector3.down * settings.groundPlaneCheckDistance);
        }
    }
    private void DrawGroundThreshold()
    {
        Vector3 editorGroundNormal = Application.isPlaying ? -stats.tempStats.groundNormal : Vector3.down;
        float editorGroundThreshold = Application.isPlaying ? stats.tempStats.targetGroundThreshold : settings.standingGroundTheshold;
        for (int i = 0; i < stats.cacheStats.checkOffsets.Length; i++)
        {
            Vector3 start = stats.tempStats.groundPlaneCheckOrigin + stats.cacheStats.checkOffsets[i];

            Gizmos.color = stats.tempStats.isGrounded ? Color.cyan : Color.orange;
            Gizmos.DrawLine(start, start + editorGroundNormal * editorGroundThreshold);
        }
    }
    private void DrawPlayer()
    {
        Gizmos.color = Color.red;

        float radius = capsuleCollider.radius;
        float height = Mathf.Max(capsuleCollider.height, radius * 2f);
        float halfHeight = height * 0.5f - radius;

        Vector3 top = capsuleCollider.transform.position + Vector3.up * halfHeight;
        Vector3 bottom = capsuleCollider.transform.position - Vector3.up * halfHeight;

        // Draw hemispheres
        Gizmos.DrawSphere(top, radius);
        Gizmos.DrawSphere(bottom, radius);
    }
#endif
}
