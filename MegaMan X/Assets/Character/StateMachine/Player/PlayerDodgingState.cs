using System.Configuration;
using Unity.Mathematics;
using UnityEngine;

public class PlayerDodgingState : PlayerBaseState
{
    private float angle;
    private Vector2 dodgeInput;
    private readonly int DodgeForwardHash = Animator.StringToHash("DodgeForward");
    private readonly int DodgeRightHash = Animator.StringToHash("DodgeRight");
    private readonly int DodgeLeftHash = Animator.StringToHash("DodgeLeft");
    private readonly int DodgeBackHash = Animator.StringToHash("DodgeBack");
    private const float CrossFadeDuration = 0.1f;

    public PlayerDodgingState(PlayerStateMachine stateMachine, Vector2 dodgeInput, float angle) : base(stateMachine)
    {
        this.angle = angle;
        this.dodgeInput = dodgeInput;

    }

    public override void Enter()
    {
        Debug.Log(angle);




        if (dodgeInput == Vector2.zero)
            setAnimProperties(3);


        /*if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 315))*/ // right quadrent
        if ((angle <= 45 || angle >= 315)) // right quadrent
        {
            setAnimProperties(4);//dodging right
        }
        else

        if (angle > 45 && angle < 135) // forward quadrent
        {
            setAnimProperties(1);//
        }
        else

        if (angle > 135 && angle < 225) //left quadrent
        {
            setAnimProperties(2);//
        }
        else

        if (angle > 225 && angle < 315) // back quadrent
        {
            setAnimProperties(3);//
        }

        //if (dodgingInput.y < -.3f && (dodgingInput.x > -.4f && dodgingInput.x < .4f))
        //{
        //    setAnimProperties(0, -1);//dodging backwards
        //}

        //if ((dodgingInput.y > .5 && dodgingInput.x > .5) || (dodgingInput.y > .5 && dodgingInput.x < -.5))
        //    setAnimProperties(0, 1);

        //if (dodgingInput.x > .3f && (dodgingInput.y > -.5f && dodgingInput.y < .5f))
        //{
        //    setAnimProperties(1, 0); //dodging right
        //}

        //if (dodgingInput.x < -.3f && (dodgingInput.y > -.5f && dodgingInput.y < .5f))
        //{
        //    setAnimProperties(-1, 0); //dodging left
        //}


    }

    public override void Tick(float deltaTime)
    {
        if (GetNormalizedTime(stateMachine.Animator, "Dodge") > 1)
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.Animator.applyRootMotion = false;
    }

    /// <summary>
    /// Set animation Roll 1: Forward |  
    ///  2: Left |
    ///  3: Back |
    ///  4: Right
    /// </summary>
    /// 
    /// 
    private void setAnimProperties(int direction)
    {
        switch (direction)
        {
            case 1: //forard
                stateMachine.Animator.CrossFadeInFixedTime(DodgeForwardHash, CrossFadeDuration);
                break;
            case 2: //Left
                stateMachine.Animator.CrossFadeInFixedTime(DodgeLeftHash, CrossFadeDuration);
                break;
            case 3:
                stateMachine.Animator.CrossFadeInFixedTime(DodgeBackHash, CrossFadeDuration);
                break;
            case 4:
                stateMachine.Animator.CrossFadeInFixedTime(DodgeRightHash, CrossFadeDuration);
                break;
        }        
        stateMachine.Animator.applyRootMotion = true;
    }
}
