using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public class IdleState : GroundedStateLayer
  {
    public IdleState(PlayerStateMachineV2 stateMachine, 
                      Rigidbody rigidbody, StateFactory stateFactory) 
      : base("Idle", stateMachine, rigidbody, stateFactory)
    {
    }

    private void CheckSwitchState()
    {
      if(_stateMachine.MoveDirection != Vector3.zero || 
         _rigidbody.velocity.x != 0 && _rigidbody.velocity.z != 0)
      {
        SwitchState(_stateFactory.Moving());
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

    public override void UpdatePhysics()
    {
      base.UpdatePhysics();
    }
  }
}
