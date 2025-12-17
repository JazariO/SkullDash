using UnityEngine;

public class JavelinSpawner : MonoBehaviour
{
    [SerializeField] GameObject javelin_prefab;
    [SerializeField] Transform hand_pivot_right;
    [SerializeField] InputDataSO input_data_SO;
    [SerializeField] float throw_power = 20f;

    private JavelinProjectile current_javelin;
    
    
    private void Start()
    {
        //TODO(Jazz): use pooling
        current_javelin = Instantiate(javelin_prefab, hand_pivot_right).GetComponent<JavelinProjectile>();
    }

    private void Update()
    {
        if(input_data_SO.attackInput)
        {
            // throw javelin
            current_javelin.transform.parent = null;
            current_javelin.ThrowJavelin(throw_power);
        }

        if(input_data_SO.reloadInput)
        {
            current_javelin.ResetJavelin();
            current_javelin.transform.parent = hand_pivot_right.transform;
            current_javelin.transform.localPosition = Vector3.zero;
        }
    }
}
