using UnityEngine;

public class MoveableController : MonoBehaviour
{
    private enum Axis { X = 0, Y = 1, Z = 2 }
    [SerializeField] Axis rotation_axis = Axis.Y;
    [SerializeField] bool rotate;
    [SerializeField] float rotate_speed = 2f;

    [SerializeField] bool move;
    [SerializeField] Transform[] waypoints;
    [SerializeField] float move_speed = 30f;
    [SerializeField] float waypoint_tolerance = 0.1f;

    private Vector3 rotation_axis_vector;
    private int current_waypoint_index;

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
    }

    private void FixedUpdate()
    {
        if(rotate)
        {
            transform.Rotate(rotation_axis_vector, rotate_speed * Time.fixedDeltaTime);
        }

        if(move)
        {
            Transform targetWaypoint = waypoints[current_waypoint_index];
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;

            // Move toward current waypoint
            transform.position += move_speed * Time.fixedDeltaTime * direction;

            // Check if close enough to switch to next waypoint
            if(Vector3.Distance(transform.position, targetWaypoint.position) <= waypoint_tolerance)
            {
                current_waypoint_index = (current_waypoint_index + 1) % waypoints.Length;
            }
        }
    }
}
