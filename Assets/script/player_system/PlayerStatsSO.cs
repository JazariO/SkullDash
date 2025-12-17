using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player System / PLayer Stats SO")]
public class PlayerStatsSO : ScriptableObject
{
    [Serializable] public struct TempStats
    {     

        public float curTargetSpeed;

        public Vector3 moveVelocity;
        public float speed;
        public Quaternion moveDirection;
        
        public bool isGrounded;
        
        public bool willJump;
        public float lastJumpTime;
        public bool lastJumpInput;
        public bool coyoteJump;
        public float coyoteTimeElapsed;
        
        public float curPitch;
        public float curYaw;
        
        public float slope;
        
        public PlayerBrain.State curState;
    }
    public TempStats tempStats;
}
