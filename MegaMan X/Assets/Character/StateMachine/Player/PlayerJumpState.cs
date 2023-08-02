using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private readonly int JumpHash = Animator.StringToHash("JumpStart");
    private readonly int WallJumpHash = Animator.StringToHash("WallJump");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 Momentum;    
    public PlayerJumpState(PlayerStateMachine stateMachine ) : base(stateMachine)
    {
        
    }

    public override void Enter()
    {

        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        Momentum = stateMachine.CharacterController.velocity;
        Momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(JumpHash,CrossFadeDuration);
        
       

        
    }
    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Move(movement + Momentum, deltaTime);
        
        if(movement != Vector3.zero)
        {
            FaceMovement(movement, deltaTime);
        }

        if (stateMachine.CharacterController.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }

        if(stateMachine.WallRun.AboveGround() && stateMachine.WallRun.HitWall())
        {
            Debug.Log(stateMachine.WallRun.HitWall());
            //Debug.Log(stateMachine.WallRun.AboveGround());

            if(stateMachine.InputReader.MovementValue.y > 0)
            {
                Debug.Log("Can Enter Wall Run State");
                stateMachine.SwitchState(new PlayerWallRunning(stateMachine));
                return;

            }
        }
        
    }

    public override void Exit()
    {
        stateMachine.ForceReceiver.Reset();
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

    public void WallJump()
    {
        Vector3 wallNormal = stateMachine.WallRun.wallRight ? 
            stateMachine.WallRun.rightWallHit.normal : stateMachine.WallRun.leftWallHit.normal;

        //Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        //    wallForward = -wallForward;

        Vector3 Velocity = new Vector3(stateMachine.CharacterController.velocity.x, 0f, stateMachine.CharacterController.velocity.z);
        Vector3 forceToApply = stateMachine.transform.up * stateMachine.WallRun.wallJumpForce + wallNormal * stateMachine.WallRun.wallJumpSideForce;

        
    }



}
