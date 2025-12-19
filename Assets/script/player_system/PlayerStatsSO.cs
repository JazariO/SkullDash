using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player System / PLayer Stats SO")]
public class PlayerStatsSO : ScriptableObject
{
    [Serializable] public struct TempStats
    {
        public Vector3[] hitPoints;
        public Quaternion moveDirection;
        public Vector3 moveVelocity;
        public Vector3 groundNormal;
        public Vector3 groundPlaneCentroid;
        public Vector3 targetVelocity;
        public float correctionForce;

        public float curTargetSpeed;
        public float lastJumpTime;
        public float speed;
        public float coyoteTimeElapsed;
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

    [Serializable] public struct CacheStats
    {
        public Vector3[] groundCheckOffsets;
        public Vector3[] wallCheckOffsets;
    }
    public CacheStats cacheStats;
}
