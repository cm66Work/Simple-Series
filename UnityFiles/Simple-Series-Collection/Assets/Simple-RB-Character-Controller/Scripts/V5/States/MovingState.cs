using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public class MovingState : GroundedStateLayer
  {
    public MovingState(PlayerStateMachineV2 stateMachine, 
                        Rigidbody rigidbody, StateFactory stateFactory) 
      : base("Moving", stateMachine, rigidbody, stateFactory)
    {
    }

    private void CheckSwitchState()
    {
      if (_rigidbody.velocity.x == 0 && _rigidbody.velocity.z == 0)
      {
        SwitchState(_stateFactory.Idle());
      }
    }

    protected override void Enter()
    {
      base.Enter();
    }

    protected override void Exit()
    {
      base.Exit();
    }

    public override void UpdateLogic()
    {
      base.UpdateLogic();


      CheckSwitchState();
    }

    // this is trying to match the desiredVelocity that the player
    // wants to reach each frame.
    private Vector3 m_goalVelocity = Vector3.zero;
    public override void UpdatePhysics()
    {
      base.UpdatePhysics();

      // gets called after the PlayerGroundedStates FixedUpdateState().
      // so we already have the correct movement direction.
      // regardless of if we are on a Slope or not.

      // the direction and speed the player wants to go this frame.
      Vector3 desiredVelocity = _stateMachine.MoveDirection.normalized *
                                _stateMachine.maxMovementSpeed;
      // we need to move slightly faster when on a slope to keep the same
      // overall speed.
      if (_stateMachine.playerInputActions.onSlope) desiredVelocity *= 20f;
      else desiredVelocity *= 10f;

      desiredVelocity.y = _rigidbody.velocity.y;

      float velDot = Vector3.Dot(m_goalVelocity, desiredVelocity);
      // allow to massive increase in acceleration if the difference
      // between the current and desired movements gets to big.
      float accel = _stateMachine.Acceleration *
                    _stateMachine.AccelerationFactorFromDot.Evaluate(velDot);

      // try to reach the desired velocity using the amount of acceleration
      // we have available to us.
      m_goalVelocity = Vector3.MoveTowards(m_goalVelocity,
        desiredVelocity, accel * Time.fixedDeltaTime);

      // calculate how much force is needed to reach the desired velocity.
      // within one physics frame.
      Vector3 neededAccel = (m_goalVelocity - _rigidbody.velocity) / Time.fixedDeltaTime;

      // but we have to respect the Max acceleration that we can place on the
      // object, meaning we might not be able to reach the desired velocity this frame.
      neededAccel = Vector3.ClampMagnitude(neededAccel, _stateMachine.MaxAccelForce);

      // apply the needed acceleration to the player.
      _rigidbody.AddForce(neededAccel * _rigidbody.mass, ForceMode.Force);
    }
  }
}
