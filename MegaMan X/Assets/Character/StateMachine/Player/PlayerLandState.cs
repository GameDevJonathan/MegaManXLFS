using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerBaseState
{
    Vector2 inputMovement;
    private readonly int LandHash = Animator.StringToHash("JumpEnd");
    private readonly int RollingLandHash = Animator.StringToHash("LandingRoll");
    private const float CrossFadeDuration = 0.3f;
    private float falltime;
    Vector3 Momentum;

    public PlayerLandState(PlayerStateMachine stateMachine, Vector2 inputMovement, float fallTime) : base(stateMachine)
    {
        this.inputMovement = stateMachine.InputReader.MovementValue;
        this.falltime = fallTime;

    }

    public override void Enter()
    {
        //stateMachine.InputReader.isDashing = false;
        //Momentum = new Vector3(0f, 0f, stateMachine.CharacterController.velocity.z*.7f);
        
        //if fall time > max fall time
        if (falltime >= 1.5f) 
        {
            if(inputMovement != Vector2.zero) //roll
            {
                Debug.Log("Rolling Land");
                stateMachine.Animator.CrossFadeInFixedTime(RollingLandHash, 0.01f);
                stateMachine.Animator.applyRootMotion = true;

            }

            if(inputMovement == Vector2.zero) //todo hard land
            {
                //land
                stateMachine.Animator.CrossFadeInFixedTime(LandHash, CrossFadeDuration);
            }
            
        }


        
        if (falltime < 1.5f)
        {
            stateMachine.Animator.CrossFadeInFixedTime(LandHash, CrossFadeDuration);

        }
    }

    public override void Tick(float deltaTime)
    {
        AnimatorStateInfo currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);

        if (stateMachine.Animator.IsInTransition(0) && nextInfo.IsName("JumpEnd"))
        {
            if(inputMovement != Vector2.zero)
            {
                ReturnToLocomotion();
                return;
            }
        }




        if (GetNormalizedTime(stateMachine.Animator, "Landing") < 1f) { return; }
        ReturnToLocomotion();


    }

    public override void Exit()
    {
        stateMachine.MeshTrail.isTrailActive = false;

    }


}
