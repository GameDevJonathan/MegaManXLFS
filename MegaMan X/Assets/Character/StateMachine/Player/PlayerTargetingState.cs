using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingHash = Animator.StringToHash("TargetingBlend");
    private const float CrossFadeDuration = 0.1f;
    private LayerMask LockOnTargetMask;
    private RaycastHit LockOnTargetHit;
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.LockOnTargetMask = stateMachine.lockOnTargetColliderMask;
    }



    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(TargetingHash, CrossFadeDuration);



        Debug.Log("PlayerTargeting State:: Entered Targeting State");
        stateMachine.InputReader.CancelEvent += OnCancel;
    }


    public override void Tick(float deltaTime)
    {
        if (stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwitchState(new Grounded(stateMachine));
            return;
        }

        Vector3 movement = TargetedMovement();

        Move(movement * stateMachine.LockOnMovementSpeed, deltaTime);
        FaceTarget();
        stateMachine.Animator.SetFloat("ForwardSpeed", stateMachine.InputReader.MovementValue.y);
        stateMachine.Animator.SetFloat("StrafingSpeed", stateMachine.InputReader.MovementValue.x);
        stateMachine.Animator.SetFloat("StrafeMovement", stateMachine.InputReader.MovementValue.magnitude);

        if (stateMachine.Targeter.CurrentTarget != null)
            RayCastDebug();


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

    private void RayCastDebug()
    {
        Vector3 distance = stateMachine.Targeter.CurrentTarget.transform.position - stateMachine.FirePoint.position;
        
        distance = distance.normalized;

        Physics.Raycast(stateMachine.FirePoint.position,
        (stateMachine.Targeter.CurrentTarget.transform.position - stateMachine.FirePoint.position).normalized, out RaycastHit targetHit,
        distance.magnitude, LockOnTargetMask);

        Vector3 faceTarget = stateMachine.Targeter.CurrentTarget.transform.position - stateMachine.FirePoint.position;

        stateMachine.FirePoint.rotation =
            Quaternion.LookRotation(faceTarget);




        Debug.DrawRay(stateMachine.FirePoint.transform.position,
            distance * distance.magnitude,Color.yellow);
    }
}
