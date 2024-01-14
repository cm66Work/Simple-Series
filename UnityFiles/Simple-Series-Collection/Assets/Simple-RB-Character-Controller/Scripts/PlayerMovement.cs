using UnityEngine;
using UnityEngine.InputSystem;

namespace com.ES.SimpleSystems.RBPlayerController
{
    public class PlayerMovement : MonoBehaviour
    {

        [Header("Input System")]
        [SerializeField] private PlayerInput m_playerInput;

        [Header("Rigidbody")]
        [SerializeField] private Rigidbody m_rigidBody;

        private void Start()
        {
            InitMovementVariables();
            InitJumpVariables();
        }

        private void FixedUpdate()
        {
            MovePlayer();

            if(m_jumpAction.triggered && m_readyToJump && m_grounded)
            {
                m_readyToJump = false;
                Jump();

                Invoke(nameof(ResetJump), m_jumpCoolDown);
            }
        }

        #region Movement

        [Header("Movement")]
        [SerializeField] private float m_walkSpeed;
        [SerializeField] private float m_sprintSpeed;
        private float m_moveSpeed = 10f;

        [SerializeField] private Transform m_orientation;

        private Vector3 m_moveDirection;
        
        private InputAction m_moveInput;

        [Header("Drag")]
        [SerializeField] private float m_groundDrag = 1f;

        [Header("Ground Check")]
        [SerializeField] private float m_playerHight;
        [SerializeField] private LayerMask m_whatIsGround;
        private bool m_grounded;

        private void InitMovementVariables()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            m_rigidBody.freezeRotation = true;

            m_moveInput = m_playerInput.actions["Movement"];
        }

        private void MovePlayer()
        {
            // check if we are grounded.
            m_grounded = Physics.Raycast(transform.position, Vector3.down, (m_playerHight * 0.5f) + 0.2f, m_whatIsGround);
                
            if (m_grounded)
                m_rigidBody.drag = m_groundDrag;
            else
                m_rigidBody.drag = 0f;


            Vector2 input = m_moveInput.ReadValue<Vector2>();
            // calculate movement direction
            m_moveDirection = m_orientation.forward * input.y + m_orientation.right * input.x;

            if (m_grounded)
                m_rigidBody.AddForce(m_moveDirection.normalized * m_moveSpeed * 10f, ForceMode.Force);
            else if (!m_grounded)
            {
                m_rigidBody.AddForce(m_moveDirection.normalized * m_moveSpeed * m_airMultiplier, ForceMode.Force);
            }  

        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
            
            // limit velocity if we are over the max move speed.
            if(flatVel.magnitude > m_moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * m_moveSpeed;
                m_rigidBody.velocity = new Vector3(limitedVel.x, m_rigidBody.velocity.y, limitedVel.z);
            }
        }
        #endregion

        #region Air Movment
        [Header("Jumping")]
        [SerializeField] private float m_jumpForce;
        [SerializeField] private float m_jumpCoolDown;
        [SerializeField] private float m_airMultiplier;

        private InputAction m_jumpAction;

        private bool m_readyToJump = true;

        private void InitJumpVariables()
        {
            m_jumpAction = m_playerInput.actions["Jump"];
        }

        private void Jump()
        {
            // set y velocity to 0 so we always jump the same height.
            m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);

            m_rigidBody.AddForce(transform.up * m_jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            m_readyToJump = true;
        }
        #endregion
    }
}
