using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingsSO", menuName = "Player System / Player Settings SO")]
public class PlayerSettingsSO : ScriptableObject
{
    //MAIN SETTINGS
    [Header("Walking and Running")]
    [Tooltip("Movement speed when walking")]
    public float walkSpeed = 2;
    [Tooltip("Movement speed when running")]
    public float runSpeed = 4;
    [Tooltip("How quickly the player accelerates to the target movement speed while grounded")]
    public float groundAcceleration = 1;

    [Header("Jumping and Falling")]
    [Tooltip("Initial upward velocity applied when jumping")]
    public float jumpForce = 6;
    [Tooltip("Time window (in seconds) after pressing jump where the jump will still trigger before landing")]
    public float jumpBufferTime = 0.2f;
    [Tooltip("Time window (in seconds) while falling where the jump will still trigger")]
    public float coyoteTime = 0.3f;
    [Tooltip("How quickly the player accelerates to the target movement speed while airborne")]
    public float airAcceleration = 0.1f;
    [Tooltip("Base gravity force applied to the player")]
    public float gravity = 10;
    [Tooltip("Maximum downward velocity the player can reach while falling")]
    public float maxFallSpeed = 10;
    [Range(1, 5)]
    [Tooltip("How rapidly the players falls speed increases")]
    public float fallGravMultiplier = 1.5f;

    [Header("Sliding and Crouching")]
    [Tooltip("Movement speed when sliding")]
    public float slideSpeed = 6;
    [Tooltip("How long it take to slow down when sliding")]
    public float slideAccelation = 1;
    [Tooltip("The horizontal force that is given when jumping out of slide")]
    public float slideJumpSpeed;
    [Tooltip("Time window (in seconds) after pressing slide where the slide will still trigger before landing")]
    public float slideBufferTime = 0.1f;
    [Tooltip("Minimum speed to still be sliding")]
    public float minimumSlideSpeed = 0.01f;
    [Tooltip("Movement speed when crouching")]
    public float crouchSpeed = 0.1f;
    [Tooltip("The height of the collider when sliding")]
    public float slideHeight = 1f;


    //CAMERA SETTINGS
    [Header("Camera Settings")]
    [Range(0, 90)]
    [Tooltip("Maximum vertical camera rotation angle (up and down) in degrees")]
    public float clampedPitch = 70;
    [Tooltip("How quickly the camera gets to the target position")]
    public float camAcceleration = 3;


    //COLLISION CHECK SETTINGS
    [Header("Collisions")]
    [Tooltip("Maxmimum wall collisions per frame")]
    public int maxCollisionCount = 8;
    [Tooltip("Radius of the sphere used to detect the ground")]
    public float groundCheckRadius = 0.9f;
    [Tooltip("Distance to check the ground plane normals")]
    public float groundPlaneCheckDistance = 1;
    [Tooltip("Maximum height off the ground to still be considered grounded")]
    public float standingGroundTheshold = 1f;
    [Tooltip("Maximum height off the ground while crouched to still be considered grounded")]
    public float crouchGroundThreshold = 0.3f;
    [Tooltip("The buffer that lifts the player collider off the ground")]
    public float groundThresholdBuffer = 0.2f;
    [Tooltip("Distance to check the ceiling")]
    public float ceilingCheckRadius = 1;
    public float ceilingCheckOrigin = 2;
    [Tooltip("Weight in KG")]
    public float mass = 80;
    [Range(1, 10)]
    [Tooltip("The speed it takes to correct the distance from the ground")]
    public float correctionSpeed = 2;
    [Range(0, 90)]
    [Tooltip("The maximum angle in degrees the player can climb")]
    public float slopeAngleThreshold = 45;
}
