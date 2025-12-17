using UnityEngine;

// To be attached to the camera object or called via a script attached to a camera object

namespace CameraEffects
{
    public class CameraEffects : MonoBehaviour
    {
        [SerializeField] private float shakeMagnitude = 0.02f; // Debug
        private Vector3 initialPosition;

        void Awake()
        {
            initialPosition = transform.position;
        }

        void Update()
        {
            // Debug
            //transform.position = initialPosition + Random.insideUnitSphere * shakeMagnitude;
        }

        // Can be called to induce a camera shake in scene
        // Needs to be added in Update
        public void ShakeCamera(float amount)
        {
            shakeMagnitude = amount;
            transform.position = initialPosition + Random.insideUnitSphere * shakeMagnitude;
        }
    }
}