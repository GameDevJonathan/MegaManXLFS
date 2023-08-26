using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringState : PlayerBaseState
{
    private readonly int FiringHash = Animator.StringToHash("BusterNormal");
    private const float CrossFadeDuration = 0.05f;

    public FiringState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(FiringHash,CrossFadeDuration);
    }
    public override void Tick(float deltaTime)
    {
        if(GetNormalizedTime(stateMachine.Animator,"BusterShot") > 1f)
        {
            stateMachine.SwitchState(new Grounded(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.InputReader.mediumShot = false;
        stateMachine.InputReader.chargedShot = false;
    }

}
