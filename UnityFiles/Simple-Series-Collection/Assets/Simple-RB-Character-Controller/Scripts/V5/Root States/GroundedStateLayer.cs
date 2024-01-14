using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public class GroundedStateLayer : BaseState
  {

    /* this is the layer root state that all states that are responsible for running
     * whilst the player is on the ground.
     */

    public GroundedStateLayer(string name, PlayerStateMachineV2 stateMachine,
                    Rigidbody rigidbody, StateFactory stateFactory)
      : base(name, stateMachine, rigidbody, stateFactory)
    {
    }

    private void CheckSwitchStates()
    {
      if (_stateMachine.playerInputActions.triggerJump)
      {
        SwitchState(_stateFactory.Jump());
      }
      else if(_stateMachine.playerInputActions.triggerDash)
      {
        SwitchState(_stateFactory.Dash());
      }
    }

    protected override void Enter()
    {
      Debug.Log($"{_name}: entered");
      _stateMachine.currentJumpCount = 0;
    }

    protected override void Exit()
    {
      Debug.Log($"{_name}: exited");
    }


    public override void UpdateLogic()
    {

      CheckSwitchStates();

      // Get the moment inputs.
      Vector2 moveInputs = _stateMachine.playerInputActions
                            .MoveAction.ReadValue<Vector2>();
      // calculate the movement direction and force.
      Vector3 moveDir = _stateMachine.orientation.forward * moveInputs.y +
                         _stateMachine.orientation.right * moveInputs.x;


      // update the movement direction if we are on a
      // slope, so that when the player is walking
      // up the slope they will still move at a
      // predictable speed and direction.
      if (OnSlope(_stateMachine.transform))
      {
        _stateMachine.playerInputActions.onSlope = true;
        _stateMachine.MoveDirection = GetSlopeMoveDirection(moveDir);
      }
      else
      {
        _stateMachine.playerInputActions.onSlope = false;
        _stateMachine.MoveDirection = moveDir;
      }


      LimitCurrentSpeedToMaxSpeed();
    }

    public override void UpdatePhysics()
    {
      if (false == _stateMachine.playerInputActions.triggerJump)
          FloatPlayer();
    }

    private void FloatPlayer()
    {
      Vector3 rayOrigen = _stateMachine.transform.position +
                          new Vector3(0f, _stateMachine.playerHeight, 0f);
      RaycastHit hit;
      float rayDistance = _stateMachine.playerHeight * 2f;
      Debug.DrawRay(rayOrigen, Vector3.down * rayDistance, Color.yellow);
      if (Physics.Raycast(rayOrigen, Vector3.down, out hit,
                        _stateMachine.playerHeight * 2f,
                        _stateMachine.groundLayer))
      {
        Vector3 vel = _rigidbody.velocity;
        float velDot = Vector3.Dot(Vector3.down, vel);

        float deviation = hit.distance - _stateMachine.playerHeight;
        float springForce = (deviation * _stateMachine.floatSpringStrength) -
                            (velDot * _stateMachine.floatSpringResistance);

        _rigidbody.AddForceAtPosition(Vector3.down * springForce, hit.point);
      }
    }

    private void LimitCurrentSpeedToMaxSpeed()
    {
      // limit the speed on slope.
      if (OnSlope(_stateMachine.transform))
      {
        if (_rigidbody.velocity.magnitude > _stateMachine.maxMovementSpeed)
        {
          _rigidbody.velocity = _rigidbody.velocity.normalized * _stateMachine.maxMovementSpeed;
        }
      }
      // limit the speed on the ground;
      else
      {
        // if we are moving faster then the max speed then
        // we need to clamp the speed to the max speed.

        // first we take the velocity on the x and z plane.
        Vector3 flatVel = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

        // limit velocity if we are moving faster then max speed.
        if (flatVel.magnitude > _stateMachine.maxMovementSpeed)
        {
          Vector3 limitVel = flatVel.normalized * _stateMachine.maxMovementSpeed;
          _rigidbody.velocity = new Vector3(limitVel.x, _rigidbody.velocity.y, limitVel.z);
        }
      }
    }


    private RaycastHit m_slopeHit;
    private bool OnSlope(Transform transform)
    {
      if (Physics.Raycast(
          transform.position + new Vector3(0, 0.1f, 0),
          Vector3.down,
          out m_slopeHit,
          _stateMachine.playerHeight * 0.5f + 0.3f
          ))
      {
        float angle = Vector3.Angle(
            Vector3.up,
            m_slopeHit.normal
            );
        return angle < _stateMachine.maxSlopeAngle && angle != 0;
      }
      return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 moveDir)
    {
      return Vector3.ProjectOnPlane(moveDir, m_slopeHit.normal)
                    .normalized;
    }

  } 
} 