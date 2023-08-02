using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Controller : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool canJump;
    [SerializeField] public bool playerLock;
    [SerializeField] float freeLookSpeed;

    [Header("WallRunning")]
    public bool isWallrunning;


    private float verticalVelocity = 0f;

    [SerializeField] private float rotationSpeed = 11f;

    [Tooltip("How Fast the player Moves")]
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerWalkSpeed;
    [SerializeField] private float playerRunSpeed;
    [SerializeField] private float jumpForce = 15f;

    [Tooltip("Character Jump Height")]
    [SerializeField] private float jumpHeight = 15f;

    private Vector3 playerMovement;

    [SerializeField] private Vector2 moveInput;

    [SerializeField] private Vector2 cameraRot;
    [SerializeField] public Transform raycastOrientation;
    

    public Vector3 Movement => Vector3.up * verticalVelocity;
    Transform cam;

    [Header("Cinemachine")]
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
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    private Vector2 CameraValue => cameraRot;

    

    private void Start()
    {
        cam = Camera.main.transform;
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

    }

    private void Update()
    {
        cameraRot.x = Input.GetAxis("RHorizontal");
        cameraRot.y = Input.GetAxis("RVertical");
        isGrounded = controller.isGrounded;
        
        if (playerLock) return;
        //sprinting = Input.GetButton("Fire1");
        



        //if(Input.GetButton("Fire1") && playerMovement != Vector3.zero)
        //{
        //    freeLookSpeed = 2f;
        //    playerSpeed = playerRunSpeed;
        //}
        //else
        //{
            freeLookSpeed = playerMovement.magnitude;
            playerSpeed = playerWalkSpeed;
        //}

        freeLookSpeed = Mathf.Clamp(freeLookSpeed, 0f, 2f);

        animator.SetFloat("Speed", freeLookSpeed);
        //Debug.Log(freeLookSpeed);

        if (playerMovement != Vector3.zero)
            FaceMovement(playerMovement);

        playerMovement = CalculateMovement();
        controller.Move((playerMovement + Movement) * Time.deltaTime * playerSpeed);
        //controller.Move( ( (playerMovement * Time.deltaTime * playerSpeed) + Movement) );
        JumpAndGravity();


    }

    private void LateUpdate()
    {
        CameraRotation();
    }   

    private Vector3 CalculateMovement()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * moveInput.y +
            right * moveInput.x;
    }

    private void JumpAndGravity()
    {

        if (isGrounded)
        {
            if (Input.GetButtonDown("Fire2") && canJump)
            {
                if (!isJumping)
                {
                    animator.Play("JumpStart");
                    canJump = false;

                }
            }
        }




        if (verticalVelocity < 0f && controller.isGrounded && isJumping)
        {
            verticalVelocity = -2f;

            if (playerMovement != Vector3.zero)
            {
                animator.Play("LandingRoll");
                playerLock = true;
                animator.applyRootMotion = true;
            }
            else
            {
                animator.Play("JumpEnd");
                playerLock = true;
            }
            isJumping = false;

        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    private void FaceMovement(Vector3 movement)
    {

        transform.rotation =
            Quaternion.Lerp(this.transform.rotation,
            Quaternion.LookRotation(movement), Time.deltaTime * rotationSpeed);

        raycastOrientation.rotation = transform.rotation;
           


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

            _cinemachineTargetYaw -= CameraValue.x * deltaTimeMultiplier * CameraRotationSpeed;
            _cinemachineTargetPitch -= CameraValue.y * deltaTimeMultiplier * CameraRotationSpeed;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);

    }

    public void AddForce()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        //verticalVelocity = jumpForce;
        isJumping = true;
    }

    public void ReturnToMovement()
    {
        canJump = true;
        playerLock = false;
        animator.Play("Movement");
        animator.applyRootMotion = false;
    }

   

}
