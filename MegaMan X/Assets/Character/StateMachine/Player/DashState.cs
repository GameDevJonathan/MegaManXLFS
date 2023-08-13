using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerBaseState
{
    private readonly int DashStart = Animator.StringToHash("DashStart");
    private float crossFadeTime = 0.1f;
    private float previousFrameTime;
    private bool alreadyAppliedForce;


    public DashState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(DashStart, crossFadeTime);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Dashing");
        //Debug.Log(normalizedTime);
            

        if(normalizedTime > previousFrameTime && normalizedTime < 1f) 
        {
            if (normalizedTime >= stateMachine.DashForceTime)
            {
                TryApplyForce();
            }
        }
        else if(normalizedTime > 1f) 
        {
            stateMachine.SwitchState(new Grounded(stateMachine,true));
        }


        previousFrameTime = normalizedTime;
        
    }

    public override void Exit()
    {
        
    }

    public void TryApplyForce()
    {

        if (alreadyAppliedForce) return;
        stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * stateMachine.DashForce);
        alreadyAppliedForce = true;

    }

  
}
