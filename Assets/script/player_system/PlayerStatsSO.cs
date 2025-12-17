using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player System / PLayer Stats SO")]
public class PlayerStatsSO : ScriptableObject
{
    internal bool coyoteJump;
    internal float coyoteTimeElapsed;
    internal float curTargetSpeed;
    internal Vector3 moveVelocity;
    internal Quaternion moveDirection;
    internal bool isGrounded;
    internal bool willJump;
    internal float lastJumpTime;
    internal float curPitch;
    internal float curYaw;
    internal float slope;
    internal PlayerBrain.State curState;
}
