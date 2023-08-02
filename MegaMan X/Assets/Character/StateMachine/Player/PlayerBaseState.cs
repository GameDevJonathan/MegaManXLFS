using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(float deltaTime)
    {
        Move(Vector3.zero, deltaTime);
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.CharacterController.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }

    protected void FaceMovement(Vector3 movement, float deltatime)
    {
        stateMachine.transform.rotation =
            Quaternion.Lerp(stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltatime * stateMachine.RotationSmoothValue);
    }

    protected float GetNormalizedTime(Animator animator, string tag)
    {
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0;
        }
    }

    protected void ReturnToLocomotion()
    {
        stateMachine.SwitchState(new Grounded(stateMachine, true));
        if (stateMachine.Animator.applyRootMotion)
        {
            stateMachine.Animator.applyRootMotion = false;
        }
        //if (stateMachine.targeter.currentTarget != null)
        //{
        //    stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        //}
        //else
        //{
        //    stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        //}
    }
}
