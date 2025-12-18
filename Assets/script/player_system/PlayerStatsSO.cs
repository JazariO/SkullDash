using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player System / PLayer Stats SO")]
public class PlayerStatsSO : ScriptableObject
{
    [Serializable] public struct TempStats
    {     
        public Quaternion moveDirection;

        public Vector3 moveVelocity;
        public Vector3 groundNormal;
        public Vector3 groundPoint;
        public Vector3 groundPlaneCentroid;
        public Vector3 groundDistance;
        public Vector3 targetVelocity;
        public Vector3 curJumpForce;
        public Vector3 correctionForce;

        public float curTargetSpeed;
        public float lastJumpTime;
        public float speed;
        public float coyoteTimeElapsed;
        public float curGravityMultiplier;
        public float curPitch;
        public float curYaw;
        public float slope;
        public float curAccel;
        public float slopeDirection;

        public bool willJump;
        public bool coyoteJump;
        public bool isGrounded;

        public PlayerBrain.State curState;
        
    }
    public TempStats tempStats;
}
