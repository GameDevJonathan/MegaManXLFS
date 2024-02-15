using UnityEngine;


public class Grounded : PlayerBaseState
{
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("Movement");
    private readonly int EquipHash = Animator.StringToHash("EquipSword");
    private readonly int SheathHash = Animator.StringToHash("SheathSword");
    public float AnimatorDampTime = 0.05f;
    private float freeLookValue = 1;
    private float freeLookMoveSpeed;
    private bool shouldFade;
    private const float CrossFadeDuration = 0.1f;
    private bool grounded => stateMachine.WallRun.CheckForGround();
    


    public Grounded(PlayerStateMachine stateMachine, bool shouldFade = false) : base(stateMachine)
    {
        this.freeLookMoveSpeed = stateMachine.FreeLookMovementSpeed;
        this.shouldFade = shouldFade;
    }

    public override void Enter()
    {
        
        stateMachine.rig.weight = 0f;

        if (!shouldFade)
            stateMachine.Animator.Play(FreeLookBlendTreeHash);
        else
            stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);

        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.DashEvent += OnDash;
        stateMachine.InputReader.EquipEvent += OnEquip;
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.MeleeEvent += OnMelee;
        stateMachine.InputReader.SpecialBeamEvent += InputReader_SpecialBeamEvent;
    }

    public override void Tick(float deltaTime)
    {
        var hitData = stateMachine.EnviromentScaner.ObstacleCheck();        

        if (grounded)
        {
            //Debug.Log("Grounded State :: I am grounded");
            if (hitData.forwardHitFound)
            {
                
                if (stateMachine.InputReader.MovementValue.magnitude > 0.1f)
                {
                    foreach (var action in stateMachine.ParkourActions)
                    {
                        if(action.CheckIfPossible(hitData,stateMachine.transform))
                        Debug.Log(action.CheckIfPossible(hitData, stateMachine.transform));
                        if (action.CheckIfPossible(hitData, stateMachine.transform))
                        {
                            Debug.Log("Obstacle Found" + hitData.forwardHit.transform.name);
                            stateMachine.SwitchState(new PlayerParkourState(stateMachine, action.AnimName, action.RotateToObstacle,
                                action.TargetRotation, action.EnableTargetMatching, action.MatchPos, action));
                            return;
                        }
                    }

                }

            }
        }



        #region Inputs
        if (!stateMachine.InputReader.equipingWeapon)
        {
            if (stateMachine.InputReader.isAiming)
            {
                stateMachine.SwitchState(new AimingState(stateMachine));
                return;
            }

            if (stateMachine.InputReader.AttackButtonPressed)
            {
                stateMachine.SwitchState(new FiringState(stateMachine));
                return;
            }

            if (stateMachine.InputReader.mediumShot)
            {
                stateMachine.SwitchState(new FiringState(stateMachine));
                return;
            }

            if (stateMachine.InputReader.chargedShot)
            {
                stateMachine.SwitchState(new FiringState(stateMachine));
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

        }
        #endregion

        #region Movement
        Vector3 movement = CalculateMovement();
        Move(movement * freeLookMoveSpeed, deltaTime);



        //Debug.Log($"Grounded State::{grounded}");
        if (!grounded)
        {
            Debug.Log("Not Touching Ground");
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }


        freeLookValue = Mathf.Clamp(
            stateMachine.InputReader.MovementValue.magnitude, 0f, 1f);


        stateMachine.Animator.SetFloat(FreeLookSpeedHash, freeLookValue, AnimatorDampTime, deltaTime);
        FaceMovement(movement, deltaTime);
        #endregion






    }
    //public void OnAttack()
    //{
    //    stateMachine.SwitchState(new AttackingState(stateMachine,0));
    //    return;
    //}



    public void OnMelee()
    {
        stateMachine.SwitchState(new AttackingState(stateMachine, 0));
        return;
    }


    public void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
        return;
    }

    private void OnDash()
    {
        stateMachine.SwitchState(new DashState(stateMachine));
        return;
    }

    private void InputReader_SpecialBeamEvent()
    {
        stateMachine.SwitchState(new PlayerHyperBeam(stateMachine));
    }

    private void OnEquip()
    {
        switch (stateMachine.InputReader.SaberEquiped)
        {
            case false:
                stateMachine.Animator.Play(EquipHash, 1);
                break;

            case true:
                stateMachine.Animator.Play(SheathHash, 1);
                break;
        }
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.DashEvent -= OnDash;
        stateMachine.InputReader.EquipEvent -= OnEquip;
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.MeleeEvent -= OnMelee;
        stateMachine.InputReader.SpecialBeamEvent -= InputReader_SpecialBeamEvent;

    }

    public void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) return;
        stateMachine.InputReader.Targeting = true;
        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        return;

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
