using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private enum Axis { X = 0, Y = 1, Z = 2 }
    [SerializeField] Axis rotation_axis = Axis.Y;
    [SerializeField] bool rotate;
    [SerializeField] float rotate_speed = 2f;
    [Space]
    [SerializeField] bool move;
    [SerializeField] Transform[] waypoints;
    [SerializeField] float move_speed = 30f;
    [SerializeField] float waypoint_tolerance = 0.1f;
    [SerializeField] Rigidbody rb_platform;
    [SerializeField] PlayerStatsSO player_stats_SO;
    private Vector3 rotation_axis_vector;
    private int current_waypoint_index;
    [SerializeField] Rigidbody rb_player;

    [SerializeField] float rotation_offset;

    [System.Serializable]
    struct PlatformData
    {
        public Quaternion last_platform_rotation;
        public Vector3 last_platform_position;
        public float previous_player_yaw;
        public float player_yaw_delta;
    }
    [SerializeField] PlatformData platform_data;

    private void Start()
    {
        switch(rotation_axis)
        {
            case Axis.X:
                rotation_axis_vector = Vector3.right;
            break;
            case Axis.Y:
                rotation_axis_vector = Vector3.up;
            break;
            case Axis.Z:
                rotation_axis_vector = Vector3.forward;
            break;
        }

        // Configure Rigidbody constraints
        RigidbodyConstraints constraints = RigidbodyConstraints.None;

        // Freeze rotation except on the chosen axis
        switch(rotation_axis)
        {
            case Axis.X:
                constraints |= RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                break;
            case Axis.Y:
                constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                break;
            case Axis.Z:
                constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                break;
        }

        // Freeze positions if movement is disabled
        if(!move)
        {
            constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }

        // Apply constraints to Rigidbody
        rb_platform.constraints = constraints;

    }

    private void FixedUpdate()
    {
        if(rotate)
        {
            Quaternion deltaRotation = Quaternion.AngleAxis(rotate_speed * Time.fixedDeltaTime, rotation_axis_vector);
            rb_platform.MoveRotation(rb_platform.rotation * deltaRotation);
        }

        if(move)
        {
            Transform targetWaypoint = waypoints[current_waypoint_index];

            // Calculate new position toward waypoint
            Vector3 newPosition = Vector3.MoveTowards(
                rb_platform.position,
                targetWaypoint.position,
                move_speed * Time.fixedDeltaTime
            );

            // Move using Rigidbody method
            rb_platform.MovePosition(newPosition);

            // Check if close enough to switch to next waypoint
            if(Vector3.Distance(rb_platform.position, targetWaypoint.position) <= waypoint_tolerance)
            {
                current_waypoint_index = (current_waypoint_index + 1) % waypoints.Length;
            }
        }

        if(rb_player != null)
        {
            // Orbit player around platform using yaw delta
            float platform_yaw_delta = Mathf.DeltaAngle(platform_data.last_platform_rotation.eulerAngles.y, rb_platform.rotation.eulerAngles.y );
            Quaternion platform_rotation_delta = Quaternion.AngleAxis(platform_yaw_delta, Vector3.up);
            player_stats_SO.tempStats.curPitch += platform_yaw_delta;

            // Movement
            Vector3 deltaPos = rb_platform.position - platform_data.last_platform_position;
            Vector3 relativePos = rb_player.position - platform_data.last_platform_position;
            relativePos = platform_rotation_delta * relativePos;
            Vector3 newPlayerPos = platform_data.last_platform_position + relativePos + deltaPos;
            rb_player.MovePosition(newPlayerPos);
        }

        platform_data.last_platform_position = rb_platform.position;
        platform_data.last_platform_rotation = rb_platform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player Parented");
            rb_player = other.attachedRigidbody;
            PlayerBrain player_brain = rb_player.GetComponent<PlayerBrain>();

            Debug.Log("AttachedRigidbody go: " + other.attachedRigidbody.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            rb_player = null;
        }
    }
}
