using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        Debug.Log("PlayerTargeting State:: Entered Targeting State");
        stateMachine.InputReader.CancelEvent += OnCancel;
    }


    public override void Tick(float deltaTime)
    {

    }

    public override void Exit()
    {
        stateMachine.InputReader.CancelEvent -= OnCancel;

    }

    public void OnCancel()
    {
        stateMachine.Targeter.Cancel();
        stateMachine.SwitchState(new Grounded(stateMachine));
    }
}
