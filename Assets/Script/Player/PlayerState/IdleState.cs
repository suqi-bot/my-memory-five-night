using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private PlayerFsmState fsmState;
    private Fsm fsm;


    public void OnEnter()
    {
        fsmState = PlayerFsmState.Instance;
        this.fsm = fsmState.fsm;
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

   
}
