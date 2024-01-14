using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public class JumpingState : BaseState
  {

    /* this is the layer root state that all states that are responsible for running
     * whilst the player is on the ground.
     */

    public JumpingState(PlayerStateMachineV2 stateMachine, 
                   Rigidbody rigidbody, StateFactory stateFactory) 
      : base("Jumping", stateMachine, rigidbody, stateFactory)
    {
    }

    protected override void Enter()
    {
      Debug.Log($"{_name}: entered");

      HandleJump();

    }

    protected override void Exit()
    {
      Debug.Log($"{_name}: exited");
      
      _stateMachine.playerInputActions.triggerJump = false;
    }

    public override void UpdateLogic()
    {
      if(_stateMachine.IsGrounded())
      {
        SwitchState(_stateFactory.Idle());
      }
      else
      {
        CheckForExtraJumps();
      }
    }

    public override void UpdatePhysics()
    {
    }

    private void HandleJump()
    {
      _stateMachine.currentJumpCount++;
      _stateMachine.timeJumpButtonWasPressed = Time.time;
      // reset the cooldown time.
      _stateMachine.jumpCooldownTimer = _stateMachine.jumpCooldown;


      // before we jump set the y velocity to 0
      // so we will always jump the same height.
      _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

      // use ForceMode.Impulse because we only apply for force on that frame.
      _rigidbody.AddForce(
                  _stateMachine.transform.up * _stateMachine.jumpForce,
                  ForceMode.Impulse);
    }

    private void CheckForExtraJumps()
    {
      if (_stateMachine.playerInputActions.JumpAction.WasPerformedThisFrame())
      {
        // since we are ready in the jump we do not need to worry about the jump
        // cooldown like when we are not in the jump state.
        // gives the player teh ability to control when they want to
        // jump.
        if (_stateMachine.currentJumpCount < _stateMachine.maxNumberOfJumps)
        {
          HandleJump();
        }
      }
    }
  }
}
