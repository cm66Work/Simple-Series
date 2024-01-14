using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.ES.SimpleSystems.RBPlayerController
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Input System")]
        [SerializeField] private PlayerInput m_playerInput;

        private InputAction m_cameraDelta;
        private InputAction m_aimButton;

        [Header("Orientation")]
        [SerializeField] private Transform m_cameraMount;

        [Header("Camera")]
        [SerializeField] private Camera m_camera;


        private Vector2 m_rotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_cameraDelta = m_playerInput.actions["Look"];
            m_aimButton = m_playerInput.actions["Aim"];
        }

        private void Update()
        {
            UpdateCameraRotation();

            if (m_aimButton.triggered)
                ToggleCameraPosition();

            UpdateCameraPosition();
        }

        #region Camera Rotation
        [Header("Settings")]
        [SerializeField] private Vector2 m_sensitivity;
        [SerializeField] private Vector2 m_verticalLookClampAngle;

        private void UpdateCameraRotation()
        {
            // Get the mouse delta from the Input System.
            Vector2 mouseInput = m_cameraDelta.ReadValue<Vector2>() *
                Time.deltaTime * m_sensitivity;
            // Add the X value whilst subtracting the Y value, else we have inverted Y axis.
            // we are recording the x and y backwards because y rotates on the horizontal
            // and x rotates on the vertical in Unity.
            m_rotation += new Vector2(-mouseInput.y, mouseInput.x);
            // Clamp how high up we can look to prevent the player from looking through the ground,
            // or flipping the camera 180 degrees around to see their own face.
            m_rotation.x = Mathf.Clamp(m_rotation.x, m_verticalLookClampAngle.x, m_verticalLookClampAngle.y);

            // rotate the camera mount based on the rotation.
            this.transform.rotation = Quaternion.Euler(m_rotation);
        }
        #endregion

        #region Camera Zooming
        [Header("Over The Shoulder")]
        [SerializeField] private float m_acceptableMountDeviationFromTarget = 0.02f;
        [Space(1)]
        [SerializeField] private Transform m_defaultCameraMount;
        [SerializeField] private Transform m_overTheShoulderMount;

        private bool m_isZoomed = false;

        [SerializeField] private float m_zoomLerpSeed = 30f;
        // the amount the camera will move closer along its offset towards the camera mount.
        // or in other words, how close the camera will get to the camera mount.
        [SerializeField] private float m_zoomRatio = 1f;

        private Coroutine m_zoomCameraCoroutine = null;

        [Header("Camera Zooming")]
        [SerializeField] private float m_defaultFOV = 60f;
        [SerializeField] private float m_zoomedFOV = 45f;

        public void ToggleCameraPosition()
        {
            // stop the camera from moving if we currently moving.
            if(null != m_zoomCameraCoroutine) StopCoroutine(m_zoomCameraCoroutine);
            // toggled between camera mounts.
            if (false == m_isZoomed)
            {
                m_isZoomed = true;
                m_zoomCameraCoroutine = StartCoroutine(ZoomCameraToTarget(m_overTheShoulderMount, m_zoomedFOV));
            }
            else
            {
                m_isZoomed = false;
                m_zoomCameraCoroutine = StartCoroutine(ZoomCameraToTarget(m_defaultCameraMount, m_defaultFOV));
            }
        }

        private IEnumerator ZoomCameraToTarget(Transform targetMount, float targetFieldOfView)
        {
            // move the camera mount towards the new target mount
            while(Vector3.Distance(m_cameraMount.localPosition, targetMount.localPosition) > m_acceptableMountDeviationFromTarget)
            {
                // calculate the time steps needed to move camera to target position.
                float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * m_zoomLerpSeed);
                // move camera to target based on blend time steps.
                m_cameraMount.localPosition = Vector3.Lerp(m_cameraMount.localPosition, targetMount.localPosition, blend);
                // zoom the camera in or out by changing its field of view.
                m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, targetFieldOfView, blend);
                yield return new WaitForEndOfFrame();
            }
            m_camera.fieldOfView = targetFieldOfView;
            m_cameraMount.localPosition = targetMount.localPosition;
            m_zoomCameraCoroutine = null;
        }
        #endregion

        #region Camera Movement
        [Header("Player Tracking")]
        [SerializeField] private Transform m_playerTargetPoint;
        private void UpdateCameraPosition()
        {
            /* having a camera attached to a rigidbody can cause gitterning and late updates.
             * so have the camera follow the player.
             */
            this.transform.position = m_playerTargetPoint.position;
        }
        #endregion
    }
}
