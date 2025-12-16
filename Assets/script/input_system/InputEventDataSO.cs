using UnityEngine;
using Proselyte.Sigils;

[CreateAssetMenu(fileName = "InputEventDataSO", menuName = "Input System / Input Event Data SO")]
public class InputEventDataSO : ScriptableObject
{
    public GameEvent OnPauseInputEvent;
    public GameEvent OnQuickLoadInputEvent;
    public GameEvent OnQuickSaveInputEvent;
}
