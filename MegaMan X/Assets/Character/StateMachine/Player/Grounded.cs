using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : PlayerBaseState  
{
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("Movement");
    public float AnimatorDampTime = 0.05f;
    private float freeLookValue = 1;
    private float freeLookMoveSpeed;
    private bool shouldFade;
    private const float CrossFadeDuration = 0.1f;

    public Grounded(PlayerStateMachine stateMachine, bool shouldFade = false) : base(stateMachine)
    {
        this.freeLookMoveSpeed = stateMachine.FreeLookMovementSpeed;
        this.shouldFade = shouldFade;
    }

    public override void Enter()
    {
        if (!shouldFade)
            stateMachine.Animator.Play(FreeLookBlendTreeHash);
        else
            stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
        
        stateMachine.InputReader.JumpEvent += OnJump;
        //stateMachine.InputReader.AttackEvent += OnAttack; 
        

    }

  
    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.isAiming)
        {
            stateMachine.SwitchState(new AimingState(stateMachine));
            return;
        }
        
        

        if (stateMachine.InputReader.Modified)
        {
            //Debug.Log("Grounded State:: input reader value: " + stateMachine.InputReader.Modified);
        }
        else
        {
            //Debug.Log("Grounded State:: input reader value: " + stateMachine.InputReader.Modified);
        }     




        Vector3 movement = CalculateMovement();
        Move(movement * freeLookMoveSpeed, deltaTime);

        //if (stateMachine.InputReader.AttackButtonPressed)
        //{
        //    stateMachine.SwitchState(new AttackingState(stateMachine));
        //    return;
        //}
        
        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }


        freeLookValue = Mathf.Clamp(
            stateMachine.InputReader.MovementValue.magnitude, 0f, 1f);


        stateMachine.Animator.SetFloat(FreeLookSpeedHash, freeLookValue, AnimatorDampTime, deltaTime);
        FaceMovement(movement, deltaTime);
    }

    //public void OnAttack()
    //{
    //    stateMachine.SwitchState(new AttackingState(stateMachine,0));
    //    return;
    //}


    public void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
        return;
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnJump;
        //stateMachine.InputReader.AttackEvent -= OnAttack;
        

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
