using UnityEngine;
using EasyAudioManager;

public class AimingState : PlayerBaseState
{
    private readonly int AimingHash = Animator.StringToHash("Aiming");
    private const float CrossFadeDuration = 0.1f;
    private LayerMask aimColliderMask;
    private Transform debugTransform;
    private float _coolDownTime = .1f;
    private float _lastFireTime  = -1f;
    private Vector3 AimPosition = Vector3.zero;
    
    
    public AimingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.aimColliderMask = stateMachine.aimColliderMask;
        this.debugTransform = stateMachine.debugTransform;
    }

    public override void Enter()
    {
        
        stateMachine.Animator.CrossFadeInFixedTime(AimingHash,CrossFadeDuration);
        stateMachine._thirdPersonCam.SetActive(false);
        stateMachine._AimCam.SetActive(true);
        

    }
    public override void Tick(float deltaTime)
    {

        #region Aim and Rotation

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            debugTransform.position = raycastHit.point;
            AimPosition = raycastHit.point;
        }

        Vector3 worldAimTarget = AimPosition;
        worldAimTarget.y = stateMachine.transform.position.y;

        Vector3 faceDirection = (worldAimTarget - stateMachine.transform.position).normalized;
        stateMachine.transform.forward = Vector3.Lerp(
            stateMachine.transform.forward, faceDirection, deltaTime * 20f);
        #endregion

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

        if (stateMachine.InputReader.isAiming == false)
        {
            //stateMachine.InputReader.CinemachineCameraTarget.transform.rotation = stateMachine.transform.rotation;
            //stateMachine.InputReader.ResetCamera();
            stateMachine.SwitchState(new Grounded(stateMachine, true));
            return;
        }        
    }

    public override void Exit()
    {
        stateMachine._thirdPersonCam.SetActive(true);
        stateMachine._AimCam.SetActive(false);
    }

    private void ShotLevel(int level, string sfx)
    {
        if (Time.time > _lastFireTime)
        {
            Vector3 aimDir = (AimPosition - stateMachine.FirePoint.position).normalized;
            MonoBehaviour.Instantiate(stateMachine.BusterShot[level], stateMachine.FirePoint.transform.position,
                Quaternion.LookRotation(aimDir, Vector3.up));
            UniversalAudioPlayer.PlayInGameSFX(sfx);
            _lastFireTime = Time.time + _coolDownTime;
        }
    }

   
}
