using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpUpState : PlayerBaseState
{
    private readonly int WallJumpHash = Animator.StringToHash("JumpStart");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 Momentum;
    private Vector3 JumpVector;
    float AirMovementSpeed;

    public WallJumpUpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.AirMovementSpeed = stateMachine.AirMovementSpeed;
    }

    public override void Enter()
    {
        Debug.Log("Entered WallJumpUp State");
        stateMachine.ForceReceiver.Jump(stateMachine.WallRun.wallJumpUpForce);
        Momentum = stateMachine.CharacterController.velocity;
        Momentum.y = 0f;
        JumpVector = stateMachine.WallRun.WallJumpUp();
        stateMachine.Animator.CrossFadeInFixedTime(WallJumpHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {

        Vector3 Movement = CalculateMovement();
        Move( (Movement * AirMovementSpeed) + JumpVector + Momentum,deltaTime);

        if (stateMachine.CharacterController.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }

    }

    public override void Exit()
    {
        
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputReader.MovementValue.y +
               right * stateMachine.InputReader.MovementValue.x;
    }

    

    
}
