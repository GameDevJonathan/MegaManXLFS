using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkourState : PlayerBaseState
{
    private const float CrossFadeDuration = 0.1f;
    string animName;
    private int ParkourHash;
    private bool Rotate;
    private Quaternion TargetRotation;
    private bool TargetMatching;
    private Vector3 MatchPos;
    private ParkourAction Action;
    private IEnumerator coroutine;

    public PlayerParkourState(PlayerStateMachine stateMachine, string animName,
        bool rotate, Quaternion targetRotation, bool targetMatching, Vector3 matchPos,
        ParkourAction action) : base(stateMachine)
    {
        this.animName = animName;
        this.Rotate = rotate;
        this.TargetRotation = targetRotation;
        this.TargetMatching = targetMatching;
        this.MatchPos = matchPos;
        this.Action = action;
    }

    public override void Enter()
    {
        stateMachine.Animator.applyRootMotion = true;        
        ParkourHash = Animator.StringToHash(animName);
        stateMachine.Animator.CrossFadeInFixedTime(ParkourHash, CrossFadeDuration);



    }


    public override void Tick(float deltaTime)
    {
        var animState = stateMachine.Animator.GetNextAnimatorStateInfo(0);
        bool transition = stateMachine.Animator.IsInTransition(0);

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            if (Rotate)
                stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation, TargetRotation, stateMachine.RotationSmoothValue * Time.deltaTime);

            if (TargetMatching && !transition)
                MatchTarget(Action);


        }


        if (GetNormalizedTime(stateMachine.Animator, "Parkour") > 1)
        {
            stateMachine.SwitchState(new Grounded(stateMachine));
            return;
        }

    }
    public override void Exit()
    {
        stateMachine.CharacterController.enabled = true;
        stateMachine.Animator.applyRootMotion = false;
    }


    
    void MatchTarget(ParkourAction action)
    {
        if (stateMachine.Animator.isMatchingTarget) return;
        stateMachine.CharacterController.enabled = false;
        stateMachine.Animator.MatchTarget(action.MatchPos, stateMachine.transform.rotation,
           action.MatchBodyPart, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), action.MatchStartTime, action.MatchEndTime);
    }
}
