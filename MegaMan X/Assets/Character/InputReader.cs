using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public Vector2 MovementValue { get; private set; }
    public Vector2 CameraValue { get; private set; }

    private Controls controls;

    //[SerializeField] Animator animator;
    [SerializeField, Range(0, 20)] private float _lerpTime = 5f;

    public bool Modified;

    public bool JumpButtonPressed => controls.Player.Jump.WasPressedThisFrame();
    public bool AttackButtonPressed => controls.Player.Attack.WasPressedThisFrame();

    public bool shoot;
    public bool charge;
    public bool fire;
    public bool mediumShot = false;
    public bool chargedShot = false;

    [Header("Aiming")]

    [field: SerializeField] public bool isDashing;
    [field: SerializeField] public bool isAiming { get; private set; }
    [field: Space]
    [field: Header("Weapons")]
    [field: SerializeField] public bool SaberEquiped { get; private set; } = false;
    [field: SerializeField] public bool equipingWeapon { get; private set; } = false;

    public float chargeAmount;
    public float chargeRate;
    public float _minRate = 25f;
    public float _maxRate = 100f;

    public event Action JumpEvent;
    public event Action DashEvent;
    public event Action EquipEvent;
    public Transform Player;
    //public event Action AttackEvent;





    [field: Header("Attacking")]
    [field: SerializeField] public bool isAttacking { get; private set; }
    //camera    
    Transform cam;

    [Header("Cinemachine")]
    [Tooltip("Camera Sensitivity")]
    public float Sensitivity = 1;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;


    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("Camera Rotation Speed")]
    public float CameraRotationSpeed = 3f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    [SerializeField] private float _cinemachineTargetYaw;
    [SerializeField] private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    private void Start()
    {
        //animator = GetComponent<Animator>(); 
        controls = new Controls();
        controls.Player.SetCallbacks(this);

        controls.Player.Enable();

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        //if (isAiming)
        //{
        //    animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f,
        //        Time.deltaTime * _lerpTime));
        //}
        //else
        //{
        //    animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f,
        //        Time.deltaTime * _lerpTime));
        //}

        if (charge)
        {
            chargeAmount += chargeRate * Time.deltaTime;
        }

        if (!charge && chargeAmount > 0)
        {
            chargeAmount = 0;
        }

        chargeAmount = Mathf.Clamp(chargeAmount, 0, 100);
    }



    private void LateUpdate()
    {

        CameraRotation();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        JumpEvent?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            shoot = true;
            fire = false;
            Debug.Log("Firing");
            mediumShot = false;
            chargedShot = false;
        }
        else
        if (context.performed)
        {
            shoot = false;
            charge = true;
            if (chargeAmount > _minRate)
                Debug.Log("Charging");


        }
        else
        if (context.canceled)
        {
            charge = false;
            fire = true;
            if (chargeAmount >= _minRate && chargeAmount < _maxRate)
            {
                Debug.Log("Meduim Shot");
                mediumShot = true;


            }
            else if (chargeAmount >= _maxRate)
            {
                Debug.Log("Max Charge Shot");

                chargedShot = true;
            }
            chargeAmount = 0;
        }
    }

    public void OnAIm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;

            CinemachineCameraTarget.transform.rotation = Player.transform.rotation;

        }
        else if (context.canceled)
        {
            isAiming = false;
        }

    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        DashEvent?.Invoke();
    }

    public void OnModifier(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Modified = true;
        }
        else if (context.canceled)
        {
            Modified = false;
        }

        Debug.Log("Modified: " + Modified);
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        CameraValue = context.ReadValue<Vector2>();
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void CameraRotation()
    {
        if (CameraValue.sqrMagnitude >= _threshold)
        {
            float deltaTimeMultiplier = Time.deltaTime;

            if (isAiming)
            {
                _cinemachineTargetYaw += CameraValue.x * Sensitivity;
                _cinemachineTargetPitch -= CameraValue.y * Sensitivity;
            }
            else
            {
                _cinemachineTargetYaw += CameraValue.x;
                _cinemachineTargetPitch -= CameraValue.y;
            }
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    public void ResetCamera()
    {
        _cinemachineTargetYaw = 180;
        _cinemachineTargetPitch = 0;
    }

    public void OnEquip(InputAction.CallbackContext context)
    {

        if (equipingWeapon) return;
        if (isAiming) return;
        if (context.performed)
        {
            EquipEvent?.Invoke();
            SaberEquiped = !SaberEquiped;
            equipingWeapon = true;            

        }
    }

    public void EquipingWeapon()
    {

        equipingWeapon = false;
    }
}
