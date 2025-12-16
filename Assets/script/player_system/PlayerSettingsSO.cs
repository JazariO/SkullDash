using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingsSO", menuName = "Player System / PLayer Settings SO")]
public class PlayerSettingsSO : ScriptableObject
{
    public float walkSpeed = 2;
    public float runSpeed = 4;
    public float groundAcceleration = 1;
    public float jumpSpeed = 6;
    public float gravity = 10;

    [Range(0,1)]public float anitGravMultiplier = 0.5f;
    public float maxFallSpeed = 10;
    public float antiGravApexThreshold = 3;

    [Header("Ground Check Settiings")]
    public float groundCheckRadius = 0.9f;
    public float groundCheckDistance = 0.25f;
}
