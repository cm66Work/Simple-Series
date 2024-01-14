using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public abstract class BaseState
  {
    protected string _name;
    protected PlayerStateMachineV2 _stateMachine;
    protected Rigidbody _rigidbody;
    protected StateFactory _stateFactory;

    public BaseState(string name, PlayerStateMachineV2 stateMachine, 
                      Rigidbody rigidbody, StateFactory stateFactory)
    {
      this._name = name;
      this._stateMachine = stateMachine;
      _rigidbody = rigidbody;
      _stateFactory = stateFactory;
    }

    protected abstract void Enter();
    public abstract void UpdateLogic();
    public abstract void UpdatePhysics();
    protected abstract void Exit();

    public void SwitchState(BaseState newState)
    {
      // exit from the current state 
      Exit();

      _stateMachine.CurrentState = newState;
      // enter the new state
      _stateMachine.CurrentState.Enter(); 
    }
  }
}
