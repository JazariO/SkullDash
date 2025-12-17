using UnityEngine;

public class JavelinProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider coll;

    private float _throw_force;
    private bool force_queued;
    private bool reset_queued;
    private Vector3 reset_position;
    private Quaternion reset_rotation;
    private bool has_hit_surface;

    private void Start()
    {
        reset_position = transform.localPosition;
        reset_rotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        if(reset_queued)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            transform.localPosition = reset_position;
            transform.localRotation = reset_rotation;
            reset_queued = false;
            has_hit_surface = false;
            return;
        }

        if(force_queued)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(transform.forward * _throw_force, ForceMode.Impulse);
            force_queued = false;
        }

        if(rb.linearVelocity.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.TryGetComponent(out SmashableItem smashable))
        {
            smashable.Hit(collision.contacts[0].point, transform.forward);
        } else if (!has_hit_surface)
        {
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

            if(closest_contact_index != -1)
            {
                rb.useGravity = false;
                rb.isKinematic = true;

                Vector3 contact_point = collision.contacts[closest_contact_index].point;
                Vector3 to_contact = contact_point - coll.bounds.center;

                if(Vector3.Dot(to_contact, transform.forward) > 0f)
                {
                    //embed javelin
                    transform.position = contact_point - transform.forward; // snap to contact

                    transform.position += transform.forward * 0.05f;
                }
            }
        }
    }

    public void ThrowJavelin(float throw_force)
    {
        force_queued = true;
        _throw_force = throw_force;
    }

    public void ResetJavelin()
    {
        reset_queued = true;
    }
}
