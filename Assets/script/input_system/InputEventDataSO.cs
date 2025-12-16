using UnityEngine;
using Proselyte.Sigils;

[CreateAssetMenu(fileName = "InputEventDataSO", menuName = "Input Data / Input Event Data SO")]
public class InputEventDataSO : ScriptableObject
{
    public GameEvent OnPauseInputEvent;
    public GameEvent OnQuickLoadInputEvent;
    public GameEvent OnQuickSaveInputEvent;
}
