using UnityEngine;

[CreateAssetMenu(fileName = "InputDataSO", menuName = "Input Data / Input Data SO")]
public class InputDataSO : ScriptableObject
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool crouchInput;
    public bool sprintInput;
    public bool jumpInput;
    public bool interactInput;
    public string InputControlScheme;
}
