using Proselyte.DebugDrawer;
using UnityEngine;

#if UNITY_EDITOR
public class PlayerMovementDebugger : MonoBehaviour
{
    [SerializeField] PlayerStatsSO player_stats_SO;
    struct DebugData
    {
        public Vector3 slope_normal;
        public Vector3 centroid_position;
        public Vector3 velocity;
        public bool grounded;
        public bool initialized;
    }
    private DebugData[] debug_data;

    [SerializeField] bool debug_enabled = true;
    
    [SerializeField, Tooltip("How many historical frames to track.")] 
    int data_size = 18;

    [SerializeField, Tooltip("Number of fixedupdate frames between redrawing debug shapes.")]
    private int debug_max_tickrate = 10; // frames per tick
    private int debug_counter = 0;
    private int debug_tick_counter = 0;

    private void Start()
    {
        debug_data = new DebugData[data_size];
        
        // Initialize debug_data
        for(int i = 0; i < debug_data.Length; i++)
        {
            DebugData data = debug_data[i];
            data.slope_normal = Vector3.up;
            data.centroid_position = Vector3.zero;
            data.velocity = Vector3.zero;
            data.initialized = false;
            debug_data[i] = data;
        }
    }

    private void FixedUpdate()
    {
        if(!debug_enabled) return;

        debug_tick_counter++;
        if(debug_tick_counter >= debug_max_tickrate)
        {
            // Sample current player status data
            DebugData current_data = new DebugData
            {
                slope_normal = player_stats_SO.tempStats.groundNormal,
                centroid_position = player_stats_SO.tempStats.groundPlaneCentroid,
                velocity = player_stats_SO.tempStats.moveVelocity,
                grounded = player_stats_SO.tempStats.isGrounded,
                initialized = true
            };

            debug_data[debug_counter] = current_data;
            debug_counter = (debug_counter + 1) % data_size;

            debug_tick_counter = 0;
        }

        for(int i = 0; i < data_size; i++)
        {
            DebugData data = debug_data[i];

            if(!data.initialized) continue;
            Debug.Log("Drawing quad: " + i);
            DebugDraw.WireQuad
            (
                data.centroid_position,
                data.slope_normal,
                Vector3.one,
                color: data.grounded ? Color.green : Color.red,
                fromFixedUpdate: true
            );

            if(data.velocity.sqrMagnitude >= 0.001f)
            {
                // Small elevation along the ground plane's normal
                Vector3 elevationOffset = data.slope_normal * 0.1f;

                Vector3 arrowStart = data.centroid_position + elevationOffset;
                Vector3 arrowEnd = arrowStart + data.velocity.normalized;

                DebugDraw.WireArrow
                (
                    arrowStart,
                    arrowEnd,
                    Vector3.up,
                    color: Color.orange,
                    fromFixedUpdate: true
                );
            }
        }
    }
}
#endif //UNITY_EDITOR