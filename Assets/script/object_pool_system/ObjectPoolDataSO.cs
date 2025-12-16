using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolDataSO", menuName = "Pooling/Smashable Pool Data")]
public class ObjectPoolDataSO : ScriptableObject
{
    [System.Serializable]
    public struct SmashablePool
    {
        public GameObject[] smashable_game_objects;
        public int defaultCapacity;
        public int maxSize;
    } 
    public SmashablePool smashable_pool;
}
