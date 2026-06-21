using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BlackBoard
{

}
public class Fsm
{
    private IState curState;
    private Dictionary<Enum, IState> stateDic;
    public BlackBoard blackBoard;
    public Fsm(BlackBoard blackBoard){
        stateDic = new Dictionary<Enum, IState>();
        this.blackBoard = blackBoard;
    }

    public void AddState(Enum iEnum, IState state)
    {
        if (stateDic.ContainsKey(iEnum))
        {
            curState = stateDic[iEnum];
        }else
        {
            stateDic.Add(iEnum, state);
            curState = state;
        }
        
    }

    public void Update()
    {
        if (curState != null)
            curState.OnUpdate();
    }

    public void SwitchState(Enum stateType)
    {
        if (!stateDic.ContainsKey(stateType))
        {
            Debug.Log("未知状态"+ stateType);
            return;
        }
        if (curState != null)
        {
            curState.OnExit();
        }
        curState = stateDic[stateType];
        curState.OnEnter();
    }
}
