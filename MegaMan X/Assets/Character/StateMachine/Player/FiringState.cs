using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        float time = GetNormalizedTime(stateMachine.Animator, "BusterShot");
        
        
        if(stateMachine.InputReader.AttackButtonPressed && (time > .5f && time < .85f))
        {
            if (stateMachine.InputReader.mediumShot) return;
            if (stateMachine.InputReader.chargedShot) return;
            
            stateMachine.SwitchState(new FiringState(stateMachine));
            return;
        }
        
        
        if(GetNormalizedTime(stateMachine.Animator,"BusterShot") > 1f)
        {
            stateMachine.SwitchState(new Grounded(stateMachine,true));
        }
    }

    public override void Exit()
    {
        stateMachine.InputReader.mediumShot = false;
        stateMachine.InputReader.chargedShot = false;
    }

}
