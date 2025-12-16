using UnityEngine;

public class JavelinProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider coll;
    [SerializeField] float force = 5f;
    [SerializeField] float rotation_speed = 5f;
    [SerializeField] InputDataSO input_data_SO;

    private bool force_queued;
    private bool reset_queued;
    private Vector3 reset_position;
    private Quaternion reset_rotation;
    private bool aim_down;
    private bool aim_up;
    private bool aim_left;
    private bool aim_right;
    private bool has_hit_surface;

    private void Start()
    {
        reset_position = transform.position;
        reset_rotation = transform.rotation;
    }

    private void Update()
    {
        if(input_data_SO.interactInput)
        {
            reset_position = transform.position;
            reset_rotation = transform.rotation;
            force_queued = true;
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            reset_queued = true;
            Debug.Log("Reset queued");
        }

        if(Input.GetKey(KeyCode.S))
        {
            aim_down = true;
        }
        else
        {
            aim_down = false;
        }

        if(Input.GetKey(KeyCode.W))
        {
            aim_up = true;
        }
        else
        {
            aim_up = false;
        }

        if(Input.GetKey(KeyCode.A))
        {
            aim_left = true;
        }
        else
        {
            aim_left = false;
        }

        if(Input.GetKey(KeyCode.D))
        {
            aim_right = true;
        }
        else
        {
            aim_right = false;
        }
    }

    private void FixedUpdate()
    {
        if(reset_queued)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.position = reset_position;
            rb.rotation = reset_rotation;
            reset_queued = false;
            has_hit_surface = false;
            return;
        }

        if(force_queued)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(transform.forward * force, ForceMode.Impulse);
            force_queued = false;
        }

        if(aim_down)
        {
            transform.Rotate(Vector3.right * rotation_speed * Time.fixedDeltaTime);
        }
        if(aim_up)
        {
            transform.Rotate(Vector3.left * rotation_speed * Time.fixedDeltaTime);
        }

        if(rb.linearVelocity.sqrMagnitude != 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
        }
        if(aim_right)
        {
            transform.Rotate(Vector3.up * rotation_speed * Time.fixedDeltaTime);

        }
        if(aim_left)
        {
            transform.Rotate(Vector3.down * rotation_speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.TryGetComponent(out SmashableItem smashable))
        {
            smashable.Hit(collision.contacts[0].point, transform.forward);
        } else if (!has_hit_surface)
        {
            Debug.Log("Hit surface infront with linear velocity: " + rb.linearVelocity.magnitude);
            has_hit_surface = true;

            // stick into surface

            // get contact point closest to the front
            float distance_infront = float.MaxValue;
            int closest_contact_index = -1;

            Vector3 frontPoint = coll.bounds.center + transform.forward * coll.bounds.extents.z;

            for(int i = 0; i < collision.contacts.Length; i++)
            {
                float contact_distance = Vector3.Distance(collision.contacts[i].point, frontPoint);
                if(contact_distance < distance_infront)
                {
                    distance_infront = contact_distance;
                    closest_contact_index = i;
                }
            }

            Debug.Log("Closest Index");

            if(closest_contact_index != -1)
            {
                rb.useGravity = false;
                rb.isKinematic = true;

                Vector3 contact_point = collision.contacts[closest_contact_index].point;
                Vector3 to_contact = contact_point - coll.bounds.center;

                if(Vector3.Dot(to_contact, transform.forward) > 0f)
                {
                    //embed javelin
                    transform.position = contact_point - transform.forward * 0.1f; // snap to contact

                    transform.position += transform.forward * 0.05f;
                }
            }
        }
    }
}
