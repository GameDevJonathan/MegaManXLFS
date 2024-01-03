using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private PlayerStateMachine stateMachine;

    public Vector2 MovementValue { get; private set; }
    public Vector2 CameraValue { get; private set; }

    private Controls controls;

    //[SerializeField] Animator animator;
    [SerializeField, Range(0, 20)] private float _lerpTime = 5f;

    [Header("Gameplay bools")]
    public bool Modified;

    public bool JumpButtonPressed => controls.Player.Jump.WasPressedThisFrame();
    public bool AttackButtonPressed => controls.Player.Attack.WasPressedThisFrame();

    [HideInInspector] public bool shoot;
    [HideInInspector] public bool charge;
    [HideInInspector] public bool fire;
    [HideInInspector] public bool mediumShot = false;
    [HideInInspector] public bool chargedShot = false;

    [Header("Aiming/TargetSelection")]
    [field: SerializeField] public bool isDashing;
    [field: SerializeField] public bool isAiming { get; private set; }
    [field: SerializeField] public Vector2 SelectionValue { get; private set; }
    public bool Targeting;

    [field: Space]
    [field: Header("Weapons")]
    [field: SerializeField] public bool SaberEquiped { get; private set; } = false;
    [field: SerializeField] public bool equipingWeapon { get; private set; } = false;


    [Header("Shader Glow")]
    public string AnimChargeRef = "ChargeLevel";
    public int chargeLevel;
    public float chargeAmount;
    public float chargeRate;
    public float _minRate = 25f;
    public float _midRate = 50f;
    public float _maxRate = 100f;

    [Header("Energy Gathering Effect")]
    [SerializeField] private GameObject _maxChargeEffect;
    [SerializeField] private GameObject _minChargeEffect;


    //Actions
    public event Action JumpEvent;
    public event Action DashEvent;
    public event Action EquipEvent;
    public event Action TargetEvent;
    public event Action CancelEvent;
    public event Action MeleeEvent;
    public event Action DodgeEvent;
    public event Action SpecialBeamEvent;
    public Transform Player;
    //public event Action AttackEvent;

    [field: Header("Attacking")]
    [field: SerializeField] public bool isAttacking { get; private set; }
    //camera    
    //Transform cam;

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

    [Tooltip("Flag for setting and resting camera")]
    public bool _resetCamera = false;
    private float dampening;
    [SerializeField] private float dampTime = 0.1f;

    // cinemachine
    [SerializeField] private float _cinemachineTargetYaw;
    [SerializeField] private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    //Material
    [Header("Material Glow")]
    [SerializeField] private Material _material;
    [SerializeField] private bool _canFlicker;
    [SerializeField] public float _targetValue;
    [SerializeField] private float _minTarget;
    [SerializeField] private float _midTarget;
    [SerializeField] private float _maxTarget;
    [SerializeField] private string _boolRef;
    [SerializeField] private string _targetRef;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        controls = new Controls();
        controls.Player.SetCallbacks(this);

        controls.Player.Enable();

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _material.SetFloat(_targetRef, _targetValue);
    }

    private void Update()
    {
        _targetValue = Mathf.Clamp(_targetValue, .1f, 1f);

        Debug.Log(stateMachine.transform.forward);

        

        //if (_resetCamera && CinemachineCameraTarget.transform.forward != stateMachine.transform.forward)
        //{
        //    SoftReset();
        //}

        //Debug.Log($"camera Target rotation y: {CinemachineCameraTarget.transform.rotation.y}");
        //Debug.Log($"Megaman X rotation y: {stateMachine.transform.rotation.y}");

        

        FlickerMaterial(_material, _boolRef, _canFlicker);

        _canFlicker = chargeLevel >= 1 ? true : false;

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
        if (stateMachine.SpecialMove || Targeting) return;
        CameraRotation();


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //MovementValue = context.ReadValue<Vector2>();
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

    public void SoftReset()
    {

        //float rot = CinemachineCameraTarget.transform.rotation.y;
        //rot = Mathf.Lerp(CinemachineCameraTarget.transform.rotation.y,
        //    stateMachine.transform.rotation.y, t);
        //CinemachineCameraTarget.transform.rotation.y += rot;
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

    public void FlickerMaterial(Material mat, string ShaderBoolRef, bool on_off)
    {
        //Debug.Log($"on/off: {mat.GetInt(ShaderBoolRef)}");

        mat.SetInt(ShaderBoolRef, Convert.ToInt32(on_off));

        if (chargeAmount >= _minRate && chargeAmount < _midRate)
        {
            chargeLevel = 1;
            Mathf.Clamp(_targetValue, 0, _minTarget);

            if (_targetValue < _minTarget)
                _targetValue += Time.deltaTime;

            _material.SetFloat(_targetRef, _targetValue);
        }

        if (chargeAmount >= _midRate && chargeAmount < _maxRate)
        {
            chargeLevel = 2;
            //_minChargeEffect?.SetActive(true);

            if (_targetValue < _midTarget)
                _targetValue += Time.deltaTime;

            _material.SetFloat(_targetRef, _targetValue);
        }

        if (chargeAmount >= _maxRate)
        {
            chargeLevel = 3;
            //_minChargeEffect?.SetActive(false);
            //_maxChargeEffect?.SetActive(true);

            if (_targetValue < _maxTarget)
                _targetValue += Time.deltaTime;

            _material.SetFloat(_targetRef, _targetValue);

        }

        if (chargeAmount == 0)
        {
            //_maxChargeEffect?.SetActive(false);
            //_minChargeEffect?.SetActive(false);
            chargeLevel = 0;
            _targetValue = 0.1f;
            _material.SetFloat(_targetRef, _targetValue);

        }


        //CHARGE THROUGH ANIMATIONS
        //if (chargeLevel < 1 && (chargeAmount > 0 && chargeAmount < _minRate))
        //{
        //    chargeLevel = 1;
        //    animator.SetInteger(AnimChargeRef, chargeLevel);

        //}
        //else if (chargeLevel < 2 && (chargeAmount >= _minRate && chargeAmount < _maxRate))
        //{
        //    chargeLevel = 2;
        //    animator.SetInteger(AnimChargeRef, chargeLevel);
        //}
        //else if (chargeLevel < 3 && chargeAmount >= _maxRate)
        //{
        //    chargeLevel = 3;
        //    animator.SetInteger(AnimChargeRef, chargeLevel);
        //}
        //else if (chargeLevel != 0 && chargeAmount == 0)
        //{
        //    chargeLevel = 0;
        //    animator.SetInteger(AnimChargeRef, chargeLevel);
        //}

    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (context.performed && !Targeting)
        {
            TargetEvent?.Invoke();
        }
        else if (context.performed && Targeting)
        {
            Targeting = false;
            CancelEvent?.Invoke();
        }

    }

    public void OnTargetSelection(InputAction.CallbackContext context)
    {
        SelectionValue = context.ReadValue<Vector2>();
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MeleeEvent?.Invoke();
        }
    }

    public void OnSpecialBeam(InputAction.CallbackContext context)
    {
        Debug.Log("Context...:" + context);
        if(context.performed)
        {
            SpecialBeamEvent?.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DodgeEvent?.Invoke();
        }

    }

    public void OnResetCamera(InputAction.CallbackContext context)
    {
        if (_resetCamera) return;
        _resetCamera = true;
    }
}
