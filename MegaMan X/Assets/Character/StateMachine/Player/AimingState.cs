using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingState : PlayerBaseState
{
    private readonly int AimingHash = Animator.StringToHash("Aiming");
    private const float CrossFadeDuration = 0.1f;
    public AimingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(AimingHash,CrossFadeDuration);
        
    }
    public override void Tick(float deltaTime)
    {
        if(stateMachine.InputReader.isAiming == false)
        {
            stateMachine.SwitchState(new Grounded(stateMachine, true));
            return;
        }
        
    }

    public override void Exit()
    {
        
    }


}
