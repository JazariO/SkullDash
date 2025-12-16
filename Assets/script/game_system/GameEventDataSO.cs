using Proselyte.Sigils;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEventDataSO", menuName = "Game System / Game Event Data SO")]
public class GameEventDataSO : ScriptableObject
{
    public GameEvent OnGameReset;
}
