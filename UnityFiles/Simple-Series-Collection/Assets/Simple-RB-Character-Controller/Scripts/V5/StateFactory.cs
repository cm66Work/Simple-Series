using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.RigidbodyController
{
  public class StateFactory
  {
    private PlayerStateMachineV2 m_stateMachine;
    private Rigidbody m_rigidbody;

    public StateFactory(PlayerStateMachineV2 stateMachine, Rigidbody rigidbody)
    {
      m_stateMachine = stateMachine;
      m_rigidbody = rigidbody;
    }

    // All grounded States.
    public GroundedStateLayer Idle() => new IdleState(m_stateMachine, m_rigidbody, this);
    public GroundedStateLayer Moving() => new MovingState(m_stateMachine, m_rigidbody, this);

    // All in Falling/ in air state
    public FallStateLayer FallMovement() => new FallingMovementState(m_stateMachine, m_rigidbody, this);

    // all global states.
    public BaseState Jump() => new JumpingState(m_stateMachine, m_rigidbody, this);
    public BaseState Dash() => new DashingSate(m_stateMachine, m_rigidbody, this);
  }
}
