using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFsm
{
    Idle,
    Run,
    Move,
    LeftHand,
    RightHand,
    Jump
}

public class PlayerBlackBoard : BlackBoard
{
    public PlayerController player;
}

public class PlayerFsmState : MonoBehaviour
{
    public static PlayerFsmState Instance => instance;
    private static PlayerFsmState instance;
    public PlayerBlackBoard playerBlackBoard;
    private PlayerFsm playerFsmType;
    public Fsm fsm;

    private PlayerFsmState(){
        if(instance == null)
        {
            instance = this;
            playerBlackBoard = new PlayerBlackBoard();
            playerFsmType = new PlayerFsm();
            fsm = new Fsm(playerBlackBoard);
        }
    }

    void Start()
    {
        //fsm.AddState(playerFsm, );
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }
}
