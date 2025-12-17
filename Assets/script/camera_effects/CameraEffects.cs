using System.Numerics;
using UnityEngine;

// To be attached to the camera object or called via a script attached to a camera object

namespace Visual
{
    public class CameraEffects : MonoBehaviour
    {
        private Camera playerCamera;
        private float defaultFOV;

        void Awake()
        {
            playerCamera = GetComponent<Camera>();
            defaultFOV = 60f;
            playerCamera.fieldOfView = defaultFOV;

        }

        public void CameraZoomIn(float startZoom, float targetZoom, float speed)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startZoom, targetZoom, speed);
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
                CameraZoomIn(0, 30f, 1f);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                CameraZoomOut();
            }
        }
    }
}