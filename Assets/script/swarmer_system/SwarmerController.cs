using UnityEngine;

public class SwarmerController : MonoBehaviour
{
    [SerializeField] float correction_rate = 0.25f;

    [SerializeField] float input_speed = 10f;
    [SerializeField] float turn_speed = 15f;
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider coll;

    [SerializeField] Transform target;
    private Vector3 target_direction;
    private Vector3 cross_torque;

    private void FixedUpdate()
    {
        // calculate target direction
        target_direction = target.position - rb.position;

        Vector3 corrected_target_vector = Vector3.Lerp(transform.forward, target_direction, correction_rate);
        corrected_target_vector.Normalize();

        float direction_dot = Vector3.Dot(rb.transform.forward, target_direction);

        cross_torque = Vector3.Cross(rb.transform.forward, target_direction) * turn_speed;
        rb.AddTorque(cross_torque);

        rb.AddForce(corrected_target_vector * input_speed, ForceMode.Force);
        if(rb.linearVelocity.sqrMagnitude != 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, 0.1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(rb.position, target.position);

        Gizmos.color = Color.lightGreen;
        Gizmos.DrawWireSphere(coll.transform.position, coll.radius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position, rb.position + rb.transform.right);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(rb.position, rb.position + rb.transform.up);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rb.position, rb.position + rb.transform.forward);

    }
}
