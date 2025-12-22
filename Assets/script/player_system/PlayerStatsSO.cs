using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Player System / PLayer Stats SO")]
public class PlayerStatsSO : ScriptableObject
{
    [Serializable] public struct TempStats
    {
        public Vector3[] hitPoints;

        public Quaternion moveRotationQuaternion;
        public Vector3 moveDirection;
        public Vector3 moveVelocity;
        public Vector3 groundNormal;
        public Vector3 wallNormal;
        public Vector3 groundPlaneCentroid;
        public Vector3 targetVelocity;
        public Vector3 groundPlaneCheckOrigin;
        public Vector3 correctionVelocity;
        public Vector3 targetCamPivotPos;
        public Vector3 curCamPivotPos;
        public Vector3 centerGroundNormal;

        public float curTargetSpeed;
        public float lastJumpTime;
        public float lastSlideTime;
        public float speed;
        public float coyoteTimeElapsed;
        public float curPitch;
        public float curYaw;
        public float slope;
        public float curAccel;
        public float slopeDirection;
        public float curStepTheshold;
        public float curGroundThreshold;
        public float targetGroundThreshold;
        public float slopeMultiplier;

        public bool willJump;
        public bool willSlide;
        public bool coyoteJump;
        public bool isGrounded;
        public bool hitCeiling;

        public PlayerBrain.State curState;
        
    }
    public TempStats tempStats;

    [Serializable] public struct CacheStats
    {
        public Vector3[] checkOffets;
        public Vector3 startCamPivotPosition;
    }
    public CacheStats cacheStats;
}
