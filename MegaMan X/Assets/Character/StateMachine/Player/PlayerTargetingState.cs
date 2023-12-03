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

        Vector3 movement = TargetedMovement();

        Move(movement * stateMachine.LockOnMovementSpeed, deltaTime);
        FaceTarget();
        stateMachine.Animator.SetFloat("ForwardSpeed", stateMachine.InputReader.MovementValue.y);
        stateMachine.Animator.SetFloat("StrafingSpeed", stateMachine.InputReader.MovementValue.x);

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

    private Vector3 TargetedMovement()
    {
        Vector3 movement = new Vector3();

        movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;
        movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;



        return movement;
    }
}
