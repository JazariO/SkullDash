using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingsSO", menuName = "Player System / Player Settings SO")]
public class PlayerSettingsSO : ScriptableObject
{
    //MAIN SETTINGS
    [Header("Main Settings")]

    [Tooltip("Movement speed when walking")]
    public float walkSpeed = 2;

    [Tooltip("Movement speed when running")]
    public float runSpeed = 4;

    [Tooltip("How quickly the player accelerates to the target movement speed while grounded")]
    public float groundAcceleration = 1;

    [Tooltip("How quickly the player accelerates to the target movement speed while airborne")]
    public float airAcceleration = 0.1f;

    [Tooltip("Initial upward velocity applied when jumping")]
    public float jumpSpeed = 6;

    [Tooltip("Time window (in seconds) after pressing jump where the jump will still trigger before landing")]
    public float jumpBufferTime = 0.2f;

    [Tooltip("Time window (in seconds) while falling where the jump will still trigger")]
    public float coyoteTime = 0.3f;

    //CAMERA SETTINGS
    [Header("Camera Settings")]

    [Range(0, 90)]
    [Tooltip("Maximum vertical camera rotation angle (up and down) in degrees")]
    public float clampedPitch = 70;


    //GRAVITY SETTINGS
    [Header("Gravity Settings")]

    [Tooltip("Base gravity force applied to the player")]
    public float gravity = 10;

    [Tooltip("Maximum downward velocity the player can reach while falling")]
    public float maxFallSpeed = 10;

    [Range(0, 1)]
    [Tooltip("Multiplier applied to gravity near the jump apex (lower values create a floatier jump)")]
    public float antiGravMultiplier = 0.5f;

    [Tooltip("Vertical speed threshold around the jump apex where reduced gravity is applied")]
    public float antiGravApexThreshold = 1;

    [Range(1, 3)]
    [Tooltip("What the gravity increases to on the frames where the velocity is increasing but the jump has cancelled")]
    public float earlyFallGravMultiplier = 3;

    [Range(1, 3)]
    [Tooltip("What the gravity increases to when falling")]
    public float fallGravMultiplier = 1.5f;


    //GROUND CHECK SETTINGS
    [Header("Ground Check Settings")]

    [Tooltip("Radius of the sphere used to detect the ground")]
    public float groundCheckRadius = 0.9f;

    [Tooltip("Distance below the player to check for ground contact")]
    public float groundCheckDistance = 0.25f;
}
