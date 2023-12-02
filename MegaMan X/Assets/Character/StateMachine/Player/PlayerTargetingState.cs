using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingHash = Animator.StringToHash("TargetingBlend");
    private const float CrossFadeDuration = 0.1f;
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(TargetingHash, CrossFadeDuration);



        Debug.Log("PlayerTargeting State:: Entered Targeting State");
        stateMachine.InputReader.CancelEvent += OnCancel;
    }


    public override void Tick(float deltaTime)
    {
        if(stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwitchState(new Grounded(stateMachine));
            return;
        }

        FaceTarget();

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
