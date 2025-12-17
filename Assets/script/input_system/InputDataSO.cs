using UnityEngine;

[CreateAssetMenu(fileName = "InputDataSO", menuName = "Input System / Input Data SO")]
public class InputDataSO : ScriptableObject
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool crouchInput;
    public bool sprintInput;
    public bool jumpHoldInput;
    public bool jumpPressedInput;
    public bool interactInput;
    public bool attackInput;
    public bool reloadInput;
    public string InputControlScheme;
}
