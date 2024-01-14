using Codice.CM.WorkspaceServer;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.ES.SimpleSystems.RigidbodyController
{

  public class PlayerInputActions
  {
    public PlayerInput playerInput;

    private InputAction m_moveAction;
    public InputAction MoveAction
    {
      get { return m_moveAction; }
      private set { m_moveAction = value; }
    }
    private InputAction m_jumpAction;
    public InputAction JumpAction
    {
      get { return m_jumpAction; }
      private set { m_jumpAction = value; }
    }
    private InputAction m_dashAction;
    public InputAction DashAction
    {
      get { return m_dashAction; }
      private set { m_dashAction = value; }
    }
    private InputAction m_sprintAction;
    public InputAction SprintAction
    {
      get { return m_sprintAction; }
      private set { m_sprintAction = value; }
    }

    public bool triggerJump;
    public bool triggerDash;

    public bool onSlope;
    
    public PlayerInputActions(PlayerInput inputs)
    {
      playerInput = inputs; 

      // Init the movement variables.
      MoveAction = playerInput.actions["Movement"];
      JumpAction = playerInput.actions["Jump"];
      DashAction = playerInput.actions["Dash"];
      SprintAction = playerInput.actions["Dash"];
    }
  }



  [RequireComponent(typeof(Rigidbody))]
  public class PlayerStateMachineV2 : MonoBehaviour
  {
    [Header("Global Settings")]
    public float playerHeight;
    public LayerMask groundLayer;
    public float GlobalGravity = -9.81f;
    public float GravityScale;
    public float FallGravityMultiplier;
    public float floatSpringStrength;
    public float floatSpringResistance;

    public Transform orientation;

    [Header("Movement Settings")]
    public bool enableMovement = true;
    public float maxMovementSpeed;
    public float maxSlopeAngle;

    public float Acceleration; // how fast we can reach max speed.
    // allows for modification of the acceleration forces 
    // based on difference between the current and desired 
    // movement direction.
    public AnimationCurve AccelerationFactorFromDot;
    public float MaxAccelForce; // how much force can be applied per frame.


    private Vector3 m_moveDirection;
    public Vector3 MoveDirection
    {
      get { return m_moveDirection; }
      set { m_moveDirection = value; }
    }

    [Header("Jump Settings")]
    public bool enableJumping = true;
    public int maxNumberOfJumps = 1;
    [HideInInspector] public int currentJumpCount = 0;
    [HideInInspector] public float timeJumpButtonWasPressed;
    public float jumpCooldown;
    [HideInInspector] public float jumpCooldownTimer;
    public float jumpForce;
    [field: Space(10), Header("CoyoteTime Settings")]
    public bool enableCoyoteTime = true;
    public float coyoteTimeWindow;

    [Header("Dash Settings")]
    public bool enableDashing = true;
    public bool canDashInAllDirections = true;
    public int maxNumberOfDashes = 1;
    [HideInInspector] public int currentDashCount;
    [HideInInspector] public float timeDashButtonWasPressed;
    public float dashBuffer;
    public float dashCooldown;
    [HideInInspector] public float dashCooldownTimer;
    public float dashForce;
    public float dashDuration;
    public float maxDashSpeed;
    public float dashAcceleration; // how fast we can reach max speed.
    // allows for modification of the acceleration forces 
    // based on difference between the current and desired 
    // movement direction.
    public AnimationCurve dashAccelerationFactorFromDot;
    public float maxDashAccelForce; // how much force can be applied per frame.



    [Space(10)]
    [SerializeField] private PlayerInput m_playerInputs;
    [HideInInspector] public PlayerInputActions playerInputActions;

    private void Awake()
    {
      playerInputActions = new PlayerInputActions(m_playerInputs);
    }


    private BaseState m_currentState;
    public BaseState CurrentState
    {
      get { return m_currentState; }
      set { m_currentState = value; }
    }
    private StateFactory m_stateFactory;
    private Rigidbody m_rigidbody;

    private void OnEnable()
    {
      m_rigidbody = this.GetComponent<Rigidbody>();
      m_rigidbody.freezeRotation = true;
      m_stateFactory = new StateFactory(this, m_rigidbody);

      /* a bad workaround for calling Enter on the Idle state
       * with out needed to make Idles Enter method public,
       * since this is the only instance where we need to 
       * directly call Enter on the new state.
       */
      CurrentState = m_stateFactory.Idle();
      CurrentState.SwitchState(m_stateFactory.Idle());
    }

    private void CheckForDash()
    {
    
    }
    
    private void Update()
    {
       
      CheckForDash();
      JumpLogic.UpdateCooldowns(this);
      JumpLogic.CheckForJump(this);

      DashLogic.UpdateCooldowns(this);
      DashLogic.CheckForDash(this);


      m_currentState.UpdateLogic();
    }

    private void FixedUpdate()
    {
      m_currentState.UpdatePhysics();
    }




    /* Helper functions that all states have access to.
     */
    #region Helper Functions

    /// <summary>
    /// Returns true if rigidbody.y velocity is negative and
    /// our float recast is touching the ground.
    /// </summary>
    /// <returns>true if player is touching the ground</returns>
    public bool IsGrounded()
    {
      bool grounded = false;
      // check if player is falling, using a small negative number since
      // the player bounces due to FloatPlayer function.
      if(m_rigidbody.velocity.y < -0.01f)
      {
        // if we are touching the ground then we need to add a dragging force to
        // stop the player from accelerating for ever.
        Vector3 rayOrigen = this.transform.position +
                      new Vector3(0f, playerHeight, 0f);
        return Physics.Raycast(
        rayOrigen, Vector3.down, playerHeight + 0.2f, groundLayer);
      }

      return grounded;
    }

    #endregion
  }

  /* Code for the jump logic was getting used to many times
   * with little alteration, so made a wrapper class for all the
   * jump logic so it can be kept in one place.
   */
  /// <summary>
  /// wrapper class for jump logic.
  /// </summary>
  public static class JumpLogic
  {
    public static void UpdateCooldowns(PlayerStateMachineV2 sm)
    {
      if (sm.jumpCooldownTimer > 0) 
        sm.jumpCooldownTimer -= Time.time;
    }

    public static void CheckForJump(PlayerStateMachineV2 sm)
    {
      /* Since the jump action can be performed in all states
       * we want to let the state machine keep track of 
       * jumping inputs from the player.
       * 
       * jumpButtonPressed will be reset after a jump has been performed.
       * 
       * Switching to the jumping state will still be handled
       * by the actual states.
       */
      if (sm.playerInputActions.JumpAction.WasPerformedThisFrame() &&
         false == sm.playerInputActions.triggerJump)
      {
        if (sm.jumpCooldownTimer <= 0 && 
            sm.currentJumpCount < sm.maxNumberOfJumps)
        {
          /* is NOT responsible to trigger the switch to the jumping state,
           * should ONLY be responsible to letting the state machine
           * know that they player can jump and that they want to jump.
           */
          sm.playerInputActions.triggerJump = true;
        }
      }
    }
  }


  /// <summary>
  /// wrapper class for dash logic.
  /// </summary>
  public static class DashLogic
  {
    public static void UpdateCooldowns(PlayerStateMachineV2 sm)
    {
      if (sm.dashCooldownTimer> 0) 
        sm.dashCooldownTimer -= Time.time;
    }

    public static void CheckForDash(PlayerStateMachineV2 sm)
    {
      /* Since the dahs action can be performed in all states
       * we want to let the state machine keep track of 
       * dash inputs from the player.
       * 
       * dashButtonPressed will be reset after a dash has been performed.
       * 
       * Switching to the dash state will still be handled
       * by the actual states.
       */
      if (sm.playerInputActions.DashAction.WasPerformedThisFrame() &&
         false == sm.playerInputActions.triggerDash)
      {
        if (sm.playerInputActions.triggerJump)
          return;

        if (sm.dashCooldownTimer <= 0 && 
            sm.currentDashCount < sm.maxNumberOfDashes)
        {
          /* is NOT responsible to trigger the switch to the jumping state,
           * should ONLY be responsible to letting the state machine
           * know that they player can jump and that they want to jump.
           */
          sm.playerInputActions.triggerDash = true;
        }
      }
    }
  }
}
