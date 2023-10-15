using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallHang : PlayerBaseState
{
    private readonly int WallHangHash = Animator.StringToHash("WallHang");
    public PlayerWallHang(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.Play(WallHangHash);
        stateMachine.ForceReceiver.Reset();
        stateMachine.ForceReceiver.SetGravity(1);
    }  

    public override void Tick(float deltaTime)
    {
        stateMachine.WallRun.WallHangMovement();
        Move(deltaTime);      

        if (stateMachine.InputReader.MovementValue.y <=0)
        {
            stateMachine.ForceReceiver.Reset();
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }

        if (stateMachine.InputReader.JumpButtonPressed)
        {
            stateMachine.ForceReceiver.Reset();
            stateMachine.WallRun.ResetWallJumpTime(.3f);
            stateMachine.SwitchState(new WallJumpUpState(stateMachine));
            return;

        }

        if ( stateMachine.InputReader.MovementValue.y > 0 && stateMachine.CharacterController.isGrounded)
        {
            stateMachine.SwitchState(new Grounded(stateMachine));
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.ForceReceiver.Reset();
        stateMachine.ForceReceiver.SetGravity(0);
       
    }

    
}
