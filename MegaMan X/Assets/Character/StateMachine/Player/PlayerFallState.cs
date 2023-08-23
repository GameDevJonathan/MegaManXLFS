using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private readonly int FallHash = Animator.StringToHash("JumpLoop");
    private const float CrossFadeDuration = 0.3f;
    private Vector3 Momentum;
    private float fallTime = 0f;
    private float fallTimeRate = 1f;
    
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.InputReader.isDashing = false;
        Momentum = stateMachine.CharacterController.velocity;
        Momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
        
    }

    public override void Tick(float deltaTime)
    {

        fallTime += fallTimeRate * Time.deltaTime;
        //Debug.Log($"FallTime:{fallTime}");
        Vector3 movement = CalculateMovement();

        //Move(Momentum, deltaTime);
        Move(movement + Momentum, deltaTime);
        
        if (movement != Vector3.zero )
            FaceMovement(movement, deltaTime);

        if (stateMachine.CharacterController.isGrounded)
        {
            stateMachine.SwitchState(new PlayerLandState(stateMachine,movement,fallTime));
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
