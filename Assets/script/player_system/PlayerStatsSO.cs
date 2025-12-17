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
        public bool coyoteJump;
        public float coyoteTimeElapsed;
        public float curGravityMultiplier;
        
        public float curPitch;
        public float curYaw;
        
        public float slope;
        public Vector3 groundNormal;
        public Vector3 groundPoint;
        
        public PlayerBrain.State curState;
    }
    public TempStats tempStats;
}
