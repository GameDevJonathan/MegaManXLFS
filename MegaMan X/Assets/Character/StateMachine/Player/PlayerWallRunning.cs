using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRunning : PlayerBaseState
{
    public PlayerWallRunning(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.ForceReceiver.Reset();
        if (stateMachine.WallRun.wallLeft)
        {
            stateMachine.Animator.Play("WallRunL");
        }

        if (stateMachine.WallRun.wallRight)
        {
            stateMachine.Animator.Play("WallRunR");
        }


    }
    public override void Tick(float deltaTime)
    {
        if(stateMachine.InputReader.MovementValue.y > 0)
        {
            stateMachine.WallRun.WallRunningMovement();

            if (stateMachine.InputReader.JumpButtonPressed)
            {
                stateMachine.ForceReceiver.Reset();
                stateMachine.WallRun.ResetWallJumpTime();                
                stateMachine.SwitchState(new WallJumpState(stateMachine));
                return;

            }
        }
        else
        {

            //stateMachine.CharacterController.Move(Vector3.zero);
            stateMachine.ForceReceiver.Reset();
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            
        }
        
    }

    public override void Exit()
    {
        
    }

}
