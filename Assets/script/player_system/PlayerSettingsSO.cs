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
    public float jumpForce = 6;

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

    [Range(1, 5)]
    [Tooltip("How rapidly the players falls speed increases")]
    public float fallGravMultiplier = 1.5f;


    //COLLISION CHECK SETTINGS
    [Header("Collision Check Settings")]

    [Tooltip("Radius of the sphere used to detect the ground")]
    public float groundCheckRadius = 0.9f;

    [Tooltip("Distance below the player to check for ground contact")]
    public float groundCheckOrigin = 0.25f;

    [Tooltip("Distance of the ground check")]
    public float groundCheckDistance = 1;

    [Tooltip("Distance of the wall check")]
    public float wallCheckDistance = 0.5f;

    [Range(1, 5)]
    [Tooltip("The speed it takes to correct the distance from the ground")]
    public float correctionSpeed = 2;

    [Range(0, 90)]
    [Tooltip("The maximum angle in degrees the player can climb")]
    public float slopeAngleThreshold = 45;
}
