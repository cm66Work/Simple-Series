using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.ES.SimpleSystems.RBPlayerController
{
    public class ThirdPersonCam : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_orientation;
        [SerializeField] private Transform m_player, m_playerObj;
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Transform m_combatLookAt;
        [SerializeField] private Camera m_camera;

        [Header("Settings")]
        [SerializeField] private float m_rotationSpeed;

        [Header("Player Input")]
        [SerializeField] private PlayerInput m_playerInput;
        private InputAction m_movementInput;

        [Header("CameraStyle")]
        public CameraStyle m_currentStyle;
        public enum CameraStyle
        {
            Basic,
            Combat,
            Topdown
        }

        private void Start()
        {
            m_movementInput = m_playerInput.actions["Movement"];

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // rotate orientation
            Vector3 viewDir = m_player.position - new Vector3(transform.position.x, m_player.position.y, transform.position.z);
            m_orientation.forward = viewDir.normalized;

            // rotate player object
            if(CameraStyle.Basic == m_currentStyle)
            {
                Vector2 cameraInput = m_movementInput.ReadValue<Vector2>();
                Vector3 inputDir = m_orientation.forward * cameraInput.y + m_orientation.right * cameraInput.x;

                if (Vector3.zero != inputDir)
                {
                    float blend = 1 - Mathf.Pow(0.5f, Time.deltaTime * m_rotationSpeed);
                    m_playerObj.forward = Vector3.Slerp(m_playerObj.forward, inputDir.normalized, blend);
                }
            }
            else if(CameraStyle.Combat == m_currentStyle)
            {
                Vector3 dirToCombat = m_combatLookAt.position -
                    new Vector3(
                        transform.position.x,
                        m_combatLookAt.position.y,
                        transform.position.z
                        );
                m_orientation.forward = dirToCombat.normalized;

                m_playerObj.forward = dirToCombat.normalized;
            }
            
        }

        [Header("FoV change when dashing")]
        [SerializeField] private float m_changeSpeed = 0.25f;

        private Coroutine m_fovChangeCoroutine;

        public void DoFOV(float targetFOV)
        {   
            if(null !=  m_fovChangeCoroutine)
            {
                StopCoroutine(m_fovChangeCoroutine);
            }
            m_fovChangeCoroutine = StartCoroutine(ChangeFOV(m_camera, targetFOV));
        }

        private IEnumerator ChangeFOV(Camera cam, float targetFOV)
        {
            while (Mathf.Approximately(cam.fieldOfView, targetFOV))
            {
                float blend = 1 - Mathf.Pow(0.5f, m_changeSpeed * Time.deltaTime);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, blend);
                yield return null;
            }
        }
    }
}
