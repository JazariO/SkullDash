using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance { get; private set; }

    public ObjectPool<GameObject> smashedItemPool;
    [SerializeField] GameObject smashedItemPrefab;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Create smashed item pool
        smashedItemPool = new ObjectPool<GameObject>(
            () => CreateGameObjectPrefab(smashedItemPrefab),
            actionOnGet: ActivateGameObjectItem,
            actionOnRelease: DeactivateGameObjectItem,
            actionOnDestroy: DestroyGameObjectItem,
            maxSize: 39);
    }

    private GameObject CreateGameObjectPrefab(GameObject item)
    {
        return Instantiate(item, transform);
    }

    public void ActivateGameObjectItem(GameObject item)
    {
        item.SetActive(true);
    }

    public void DeactivateGameObjectItem(GameObject item)
    {
        item.SetActive(false);
    }

    private void DestroyGameObjectItem(GameObject item)
    {
        Destroy(item);
    }
}