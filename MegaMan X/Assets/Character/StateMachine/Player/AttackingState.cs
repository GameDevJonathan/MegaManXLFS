using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : PlayerBaseState
{
    Attacks attack;
    float previousFrameTime;
    
    public AttackingState(PlayerStateMachine stateMachine, int AttackIndex) : base(stateMachine)
    {
        attack = stateMachine.Attacks[AttackIndex];
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName,attack.TransitionDuration);
        stateMachine.Animator.applyRootMotion = true;
        
        
    }
    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator,"Attack");

        if (normalizedTime > previousFrameTime && normalizedTime < 1f)
        {
            if (stateMachine.InputReader.AttackButtonPressed)
            {
                TryComboAttack(normalizedTime);
            }

        }
        else
        {
            if (normalizedTime > 1f)
            {
                stateMachine.SwitchState(new Grounded(stateMachine, true));
                return;
            }
        }


        previousFrameTime = normalizedTime;
        
    }

   

    public override void Exit()
    {
        stateMachine.Animator.applyRootMotion = false;
    }

   

    private void TryComboAttack(float normalizedTime)
    {
        if(attack.ComboStateIndex == -1) { return; }

        if(normalizedTime < attack.ComboAttackTime) { return; }

        stateMachine.SwitchState(new AttackingState(stateMachine, attack.ComboStateIndex));        
    }





}
