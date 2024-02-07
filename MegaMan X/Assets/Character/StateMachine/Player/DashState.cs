using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAudioManager;

public class DashState : PlayerBaseState
{
    private readonly int DashStart = Animator.StringToHash("DashStart");
    private float crossFadeTime = 0.1f;
    private float previousFrameTime;
    private bool alreadyAppliedForce;
    private bool dashJump = false;
    
    private bool grounded => stateMachine.WallRun.CheckForGround();



    public DashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(DashStart, crossFadeTime);
        stateMachine.InputReader.isDashing = true;
        UniversalAudioPlayer.PlayInGameSFX("Dash");
        //UniversalAudioPlayer.PlayVO(0);
        stateMachine.ForceReceiver.useGravity = false;
        
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Dashing");        
            

        if(normalizedTime > previousFrameTime && normalizedTime < 1f) 
        {
            if (normalizedTime >= stateMachine.DashForceTime)
            {
                TryApplyForce();
            }

            if (stateMachine.InputReader.JumpButtonPressed 
                && (grounded)
                && !dashJump
                && normalizedTime < .95f)
            {
                dashJump = true;
                stateMachine.SwitchState(new PlayerJumpState(stateMachine));
                return;
            }
        }
        else if(normalizedTime > 1f) 
        {
            stateMachine.SwitchState(new Grounded(stateMachine,true));
            return;
        }


        previousFrameTime = normalizedTime;
        
    }

    public override void Exit()
    {
        stateMachine.ForceReceiver.Reset();
        stateMachine.ForceReceiver.useGravity = true;
        stateMachine.InputReader.isDashing = (dashJump) ? true : false;
    }

    public void TryApplyForce()
    {
        if (alreadyAppliedForce) return;
        stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * stateMachine.DashForce);
        alreadyAppliedForce = true;
    }

  
}
