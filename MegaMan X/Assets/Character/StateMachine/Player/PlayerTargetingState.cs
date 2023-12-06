using EasyAudioManager;
using UnityEngine;


public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingHash = Animator.StringToHash("TargetingBlend");
    private const float CrossFadeDuration = 0.1f;
    private LayerMask LockOnTargetMask;
    private RaycastHit LockOnTargetHit;
    private Transform debugTransform;

    //shot values
    private float _lastFireTime = -1f;
    private float _coolDownTime = .1f;


    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.LockOnTargetMask = stateMachine.lockOnTargetColliderMask;
        debugTransform = stateMachine.LockOnSphere;
    }



    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(TargetingHash, CrossFadeDuration);



        //Debug.Log("PlayerTargeting State:: Entered Targeting State");
        stateMachine.InputReader.CancelEvent += OnCancel;
    }


    public override void Tick(float deltaTime)
    {
        if (stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwitchState(new Grounded(stateMachine));
            return;
        }

        stateMachine.Targeter.CycleTarget();

        Vector3 movement = TargetedMovement();

        Move(movement * stateMachine.LockOnMovementSpeed, deltaTime);

        FaceTarget();

        AnimatorValues();

        if (stateMachine.Targeter.CurrentTarget != null)
            RayCastDebug();

        if (stateMachine.InputReader.AttackButtonPressed)
        {
            ShotLevel(0, "BusterShot");
        }


    }


    public override void Exit()
    {
        stateMachine.Animator.SetFloat("StrafeMovement", 0);
        debugTransform.gameObject.SetActive(false);
        stateMachine.InputReader.CancelEvent -= OnCancel;
    }

    private void AnimatorValues()
    {
        stateMachine.Animator.SetFloat("ForwardSpeed", stateMachine.InputReader.MovementValue.y);
        stateMachine.Animator.SetFloat("StrafingSpeed", stateMachine.InputReader.MovementValue.x);
        stateMachine.Animator.SetFloat("StrafeMovement", stateMachine.InputReader.MovementValue.magnitude);
    }
    
    public void OnCancel()
    {
        debugTransform.gameObject.SetActive(false);
        stateMachine.Targeter.Cancel();
        stateMachine.InputReader.ResetCamera();
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

        float dist = Vector3.Distance(stateMachine.Targeter.CurrentTarget.transform.position, stateMachine.FirePoint.position);


        Vector3 faceTarget = distance;

        stateMachine.FirePoint.rotation =
            Quaternion.LookRotation(faceTarget);

        Debug.DrawRay(stateMachine.FirePoint.transform.position,
            stateMachine.FirePoint.transform.forward * dist, Color.yellow);

        distance = distance.normalized;        

        if (Physics.Raycast(stateMachine.FirePoint.position, distance, out LockOnTargetHit,
            dist, LockOnTargetMask))
        {
            //Debug.Log("Hit");

            debugTransform.gameObject.SetActive(true);
            debugTransform.position = LockOnTargetHit.point;           
        }
    }

    private void ShotLevel(int level, string sfx)
    {
        if (Time.time > _lastFireTime)
        {            
            MonoBehaviour.Instantiate(stateMachine.BusterShot[level], stateMachine.FirePoint.transform.position,
                stateMachine.FirePoint.rotation);
            UniversalAudioPlayer.PlayInGameSFX(sfx);
            _lastFireTime = Time.time + _coolDownTime;
        }
    }
}
