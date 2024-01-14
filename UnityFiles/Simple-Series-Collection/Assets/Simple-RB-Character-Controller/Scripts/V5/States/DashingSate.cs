using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public class DashingSate : BaseState 
  {
    public DashingSate(
      PlayerStateMachineV2 stateMachine,
      Rigidbody rigidbody, StateFactory stateFactory)
      : base("Dashing", stateMachine, rigidbody, stateFactory)
    {

    }

    public override void UpdateLogic()
    {
    }


    public override void UpdatePhysics()
    {
      if(Time.time - m_timeDashStarted < _stateMachine.dashDuration)
      {
        DashMovement();
      }
      else
      {
        // we have finished dashing.
        SwitchState(_stateFactory.Idle());
      }
    }

    private float m_timeDashStarted;
    protected override void Enter()
    {
      Debug.Log($"{_name}: entered");

      m_timeDashStarted = Time.time;

      //if (m_dashCoroutine != null)
      //{
      //  _stateMachine.StopCoroutine(m_dashCoroutine);
      //}
      //m_dashCoroutine = _stateMachine.StartCoroutine(HandleDash());
    }

    protected override void Exit()
    {
      Debug.Log($"{_name}: exited");

      _stateMachine.playerInputActions.triggerDash = false;
      _stateMachine.currentDashCount = 0;
    }


    private Coroutine m_dashCoroutine;
    private IEnumerator HandleDash()
    {
      _stateMachine.currentDashCount++;
      _stateMachine.timeDashButtonWasPressed = Time.time;
      // reset the cooldown time.
      _stateMachine.dashCooldownTimer = _stateMachine.dashCooldown;

      //// set the velocity on the X and Z plane to zero, but keep
      //// their y velocity to preserver falling speed.
      //_rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);

      //_rigidbody.AddForce(
      //  CalculateDashDirection().normalized *
      //  _stateMachine.dashForce, ForceMode.Impulse);

      DashMovement();

      yield return new WaitForSeconds(_stateMachine.dashDuration);

      SwitchState(_stateFactory.Idle());
    }
    
    
    // this is trying to match the desiredVelocity that the player
    // wants to reach each frame.
    private Vector3 m_goalVelocity = Vector3.zero;
    private void DashMovement()
    {
      // the direction and speed the player wants to go this frame.
      Vector3 desiredVelocity = CalculateDashDirection().normalized *
                                _stateMachine.maxDashSpeed;
      // we need to move slightly faster when on a slope to keep the same
      // overall speed.
      if (_stateMachine.playerInputActions.onSlope) desiredVelocity *= 20f;
      else desiredVelocity *= 10f;

      desiredVelocity.y = _rigidbody.velocity.y;

      float velDot = Vector3.Dot(m_goalVelocity, desiredVelocity);
      // allow to massive increase in acceleration if the difference
      // between the current and desired movements gets to big.
      float accel = _stateMachine.dashAcceleration*
                    _stateMachine.dashAccelerationFactorFromDot.Evaluate(velDot);

      // try to reach the desired velocity using the amount of acceleration
      // we have available to us.
      m_goalVelocity = Vector3.MoveTowards(m_goalVelocity,
        desiredVelocity, accel * Time.fixedDeltaTime);

      // calculate how much force is needed to reach the desired velocity.
      // within one physics frame.
      Vector3 neededAccel = (m_goalVelocity - _rigidbody.velocity) / Time.fixedDeltaTime;

      // but we have to respect the Max acceleration that we can place on the
      // object, meaning we might not be able to reach the desired velocity this frame.
      neededAccel = Vector3.ClampMagnitude(neededAccel, _stateMachine.maxDashAccelForce);

      // apply the needed acceleration to the player.
      _rigidbody.AddForce(neededAccel * _rigidbody.mass, ForceMode.Force);
    }

    private Vector3 CalculateDashDirection()
    {
      bool input = _stateMachine.playerInputActions.MoveAction.ReadValue<Vector2>().magnitude > 0;

      return input ? _stateMachine.MoveDirection :
                      _stateMachine.orientation.forward;
    }
  }
}
