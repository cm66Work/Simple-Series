using com.ES.SimpleSystems.RigidbodyController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

public class FallingMovementState : FallStateLayer
{
  public FallingMovementState(PlayerStateMachineV2 stateMachine, Rigidbody rigidbody, 
                              StateFactory stateFactory) 
    : base("Fall moving", stateMachine, rigidbody, stateFactory)
  {
  }

  protected override void Enter()
  {
    Debug.Log($"{_name}: entered");
  }

  protected override void Exit()
  {
    Debug.Log($"{_name}: exited");
  }


  public override void UpdateLogic()
  {
  }

  public override void UpdatePhysics()
  {
    HandleGravity();
  }

  private void HandleGravity()
  {
    Vector3 gravity = _stateMachine.GlobalGravity * 
                      _stateMachine.GravityScale * Vector3.up;
    if (_rigidbody.velocity.y < 0)
    {
      gravity *= _stateMachine.FallGravityMultiplier;
    }
    _rigidbody.AddForce(gravity, ForceMode.Acceleration);
  }

}
