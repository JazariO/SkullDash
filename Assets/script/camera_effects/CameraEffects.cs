using UnityEngine;

// To be attached to the camera object or called via a script attached to a camera object

namespace CameraEffects
{
    public class CameraEffects : MonoBehaviour
    {
        private Vector3 initialPosition;
        private Camera playerCamera;
        private float defaultFOV;

        void Awake()
        {
            initialPosition = transform.position;
            playerCamera = GetComponent<Camera>();
            defaultFOV = 60f;
            playerCamera.fieldOfView = defaultFOV;

        }

        // Can be called to induce a camera shake in scene
        // Needs to be added in Update
        public void ShakeCamera(float amount)
        {
            transform.position = initialPosition + Random.insideUnitSphere * amount;
        }

        public void GradualCameraShake(float startAmount = 0.001f, float endAmount = 0.05f, float speed = 5.0f)
        {
            transform.position = initialPosition + Random.insideUnitSphere * Mathf.Lerp(startAmount, endAmount, Time.deltaTime * speed);
        }

        public void CameraZoomIn(float startZoom, float targetZoom, float speed)
        {
            playerCamera.fieldOfView -= Mathf.Lerp(startZoom, targetZoom, speed);
        }

        public void CameraZoomOut()
        {
            playerCamera.fieldOfView = defaultFOV;
        }
        
        void Update()
        {
            // Debug
            if (Input.GetMouseButtonDown(1))
            {
                CameraZoomIn(defaultFOV, 5f, 0.5f);
            }
            if (Input.GetMouseButtonUp(1))
            {
                CameraZoomOut();
            }
        }
    }
}