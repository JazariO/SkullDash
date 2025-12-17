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
        stats.tempStats.lastJumpTime = -settings.jumpBufferTime;
        stats.tempStats.curYaw = transform.eulerAngles.y;
        stats.tempStats.moveDirection = Quaternion.identity;
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
        UpdateHorizontalVelocity();
        UpdateVerticalVelocity();
        GetGroundHit();
    }

    private void OnDisable()
    {
        stats.tempStats = default;
    }
    private void SelectState()
    {   
        if ((stats.tempStats.isGrounded && stats.tempStats.willJump) || (stats.tempStats.moveVelocity.y > 0 && inputs.jumpHoldInput && stats.tempStats.curState != State.Fall) || stats.tempStats.coyoteJump)
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
                stats.tempStats.moveVelocity.y = settings.jumpSpeed;
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
    private void UpdateHorizontalVelocity()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, stats.tempStats.groundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, stats.tempStats.groundNormal).normalized;

        Vector3 inputDir = forward * inputs.moveInput.y + right * inputs.moveInput.x;
        stats.tempStats.targetHorVelocity = inputDir * stats.tempStats.curTargetSpeed;

        float accel = (stats.tempStats.isGrounded ? settings.groundAcceleration : settings.airAcceleration) * Time.fixedDeltaTime;

        stats.tempStats.moveVelocity.x = Mathf.Lerp(stats.tempStats.moveVelocity.x, stats.tempStats.targetHorVelocity.x, accel);
        stats.tempStats.moveVelocity.z = Mathf.Lerp(stats.tempStats.moveVelocity.z, stats.tempStats.targetHorVelocity.z, accel);
        stats.tempStats.speed = new Vector3(stats.tempStats.moveVelocity.x, 0, stats.tempStats.moveVelocity.z).magnitude;
        rigidBody.linearVelocity = stats.tempStats.moveVelocity;
    }

    private void UpdateVerticalVelocity()
    {
        if (stats.tempStats.isGrounded && stats.tempStats.moveVelocity.y <= stats.tempStats.targetHorVelocity.y)
        {
            stats.tempStats.curGravityMultiplier = 0;
            stats.tempStats.moveVelocity.y = stats.tempStats.targetHorVelocity.y;
        }
        else
        {
            if (!inputs.jumpHoldInput && stats.tempStats.moveVelocity.y > 0)
            {
                stats.tempStats.curGravityMultiplier = settings.earlyFallGravMultiplier;
            }
            else if (stats.tempStats.moveVelocity.y > 0 && stats.tempStats.moveVelocity.y < settings.antiGravApexThreshold)
            {
                stats.tempStats.curGravityMultiplier = settings.antiGravMultiplier;
            }
            else if (stats.tempStats.moveVelocity.y < 0)
            {
                stats.tempStats.curGravityMultiplier = settings.fallGravMultiplier;
            }
            else if (!stats.tempStats.isGrounded)
            {
                stats.tempStats.curGravityMultiplier = 1;
            }

            stats.tempStats.moveVelocity.y -= settings.gravity * stats.tempStats.curGravityMultiplier * Time.fixedDeltaTime;
            stats.tempStats.moveVelocity.y = Mathf.Max(stats.tempStats.moveVelocity.y, settings.maxFallSpeed);
        }
    }

    private void GetGroundHit()
    {
        Vector3[] offsets =
        {
            Vector3.zero,
            new Vector3(capsuleCollider.radius, 0, 0),
            new Vector3(-capsuleCollider.radius, 0, 0),
            new Vector3(0, 0, capsuleCollider.radius),
            new Vector3(0, 0, -capsuleCollider.radius)
        };

        RaycastHit closestHit = default;
        float closestDistance = float.MaxValue;
        bool hasGroundHit = false;

        for (int i = 0; i < offsets.Length; i++)
        {
            if (Physics.Raycast(capsuleCollider.bounds.center + offsets[i], Vector3.down, out RaycastHit hit, settings.groundCheckDistance, layerData.ground))
            {
                hasGroundHit = true;

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestHit = hit;
                }
            }
        }

        if (hasGroundHit)
        {
            stats.tempStats.groundNormal = closestHit.normal;
            stats.tempStats.groundPoint = closestHit.point;
        }
        else
        {
            stats.tempStats.groundNormal = Vector3.up;
            stats.tempStats.groundPoint = Vector3.zero;
        }

        stats.tempStats.isGrounded = hasGroundHit;
        stats.tempStats.slope = Vector3.Angle(stats.tempStats.groundNormal, Vector3.up);
    }


    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;
        Gizmos.color = Color.yellow;

        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,                                
            new Vector3(capsuleCollider.radius, 0, 0f), 
            new Vector3(-capsuleCollider.radius, 0f, 0f),
            new Vector3(0f, 0f, capsuleCollider.radius), 
            new Vector3(0f, 0f, -capsuleCollider.radius) 
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 start = capsuleCollider.bounds.center + offsets[i];

            bool hit = Physics.Raycast(start, Vector3.down, settings.groundCheckDistance, layerData.ground);

            Gizmos.color = hit ? Color.red : Color.blue;
            Gizmos.DrawLine(start, start + Vector3.down * settings.groundCheckDistance);
        }
    }

}
