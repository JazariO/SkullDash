using Unity.VisualScripting;
using UnityEngine;

public class JavelinProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float force = 5f;
    [SerializeField] float rotation_speed = 5f;
    [SerializeField] InputDataSO input_data_SO;

    private bool force_queued;
    private bool reset_queued;
    private Vector3 reset_position;
    private bool aim_down;
    private bool aim_up;
    private bool has_hit_surface;

    private void Start()
    {
        reset_position = transform.position;
    }

    private void Update()
    {
        if(input_data_SO.interactInput)
        {
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
    }

    private void FixedUpdate()
    {
        if(reset_queued)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.position = reset_position;
            rb.rotation = Quaternion.identity;
            reset_queued = false;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out SmashableItem smashable))
        {
            smashable.Hit(collision.contacts[0].point, transform.forward);
        } else if (has_hit_surface)
        {
            // stick into surface

            // get contact point closest to the front
            float distance_infront = float.MaxValue;
            for(int i = 0; i < collision.contacts.Length; i++)
            {
                //distance_infront = Vector3.Distance(collision.contacts[i].point, rigidbody
            }
        }
    }
}
