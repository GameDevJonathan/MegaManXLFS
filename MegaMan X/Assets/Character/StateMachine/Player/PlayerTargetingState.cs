using EasyAudioManager;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingHash = Animator.StringToHash("TargetingBlend");
    private const float CrossFadeDuration = 0.1f;
    private LayerMask LockOnTargetMask;
    private RaycastHit LockOnTargetHit;
    private Transform debugTransform;
    Vector2 delta;
    private float angle;
    
    

    //shot values
    private float _lastFireTime = -1f;
    private float _coolDownTime = .1f;


    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.LockOnTargetMask = stateMachine.lockOnTargetColliderMask;
        debugTransform = stateMachine.debugTransform;        
    }



    public override void Enter()
    {
        
        stateMachine.Animator.CrossFadeInFixedTime(TargetingHash, CrossFadeDuration);
        stateMachine._TargetCamUtil.SetActive(true);
        //Debug.Log("PlayerTargeting State:: Entered Targeting State");        
        stateMachine.rig.weight = .6f;
        stateMachine.InputReader.CancelEvent += OnCancel;
        stateMachine.InputReader.DodgeEvent += OnDodge;
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

        
        delta = Vector2.zero - stateMachine.InputReader.MovementValue;
        angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        angle += 180;

        FaceTarget();

        AnimatorValues();

        if (stateMachine.Targeter.CurrentTarget != null)
        {
            RayCastDebug();
            if (stateMachine.Targeter.CurrentTarget.type == Target.Type.large)
                stateMachine._TargetCamUtil.gameObject.transform.localScale = Vector3.one * 2.5f;
            else
                stateMachine._TargetCamUtil.gameObject.transform.localScale = Vector3.one;

        }

        #region Shooting Mechanics

        if (stateMachine.InputReader.AttackButtonPressed)
        {
            ShotLevel(0, "BusterShot");
        }

        if (stateMachine.InputReader.mediumShot)
        {

            ShotLevel(1, "ChargedShot");
            stateMachine.InputReader.mediumShot = false;
        }

        if (stateMachine.InputReader.chargedShot)
        {
            ShotLevel(2, "MaxShot");
            stateMachine.InputReader.chargedShot = false;

        }
        #endregion
    }

    public override void Exit()
    {
        stateMachine.Animator.SetFloat("StrafeMovement", 0);
        stateMachine._TargetCamUtil.SetActive(false);
        debugTransform.gameObject.SetActive(false);
        stateMachine.rig.weight = 0f;
        stateMachine.InputReader.Targeting = false;        
        stateMachine.InputReader.CancelEvent -= OnCancel;
        stateMachine.InputReader.DodgeEvent -= OnDodge;
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
        //stateMachine.InputReader.ResetCamera();
        stateMachine.SwitchState(new Grounded(stateMachine));
    }

    public void OnDodge()
    {
        Debug.Log($"Joystick angle: {angle}");
        Vector2 move = stateMachine.InputReader.MovementValue;
        stateMachine.SwitchState(new PlayerDodgingState(stateMachine,move,angle));
        return;
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
            stateMachine._TargetCamUtil.transform.position = LockOnTargetHit.point;
            stateMachine._TargetCamUtil.transform.LookAt(stateMachine.transform.position);
            debugTransform.gameObject.SetActive(true);
            debugTransform.position = LockOnTargetHit.point;
        }
    }

    private void ShotLevel(int level, string sfx)
    {
        if (Time.time > _lastFireTime)
        {
            var shot =
            MonoBehaviour.Instantiate(stateMachine.BusterShot[level], stateMachine.FirePoint.transform.position,
                stateMachine.FirePoint.rotation);

            switch (level)
            {
                case 0:
                    shot.name = "Normal";
                    break;
                case 1:
                    shot.name = "Medium";
                    break;
                case 2:
                    shot.name = "Charged";
                    break;
            }
            
            UniversalAudioPlayer.PlayInGameSFX(sfx);
            _lastFireTime = Time.time + _coolDownTime;
        }
    }
}
