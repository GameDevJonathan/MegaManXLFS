using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;

    public float wallRunForce;
    public float wallRunTime;
    public float maxWallRunTime;
    public float wallRunTimer;

    public float wallJumpForce;
    public float wallJumpSideForce;
    public float wallJumpForwardForce;
    [SerializeField,Range(0f,2f)]
    private float lastJumpTime;

    [Header("Input")]
    public InputReader inputReader;

    [Header("Detection")]
    public float RotationSmoothValue; 
    public float wallCheckDistance;
    public float minJumpHeight;
    [SerializeField] public RaycastHit leftWallHit;
    [SerializeField] public RaycastHit rightWallHit;
    public bool wallLeft;
    public bool wallRight;

    [Header("References")]
    public Transform orientation;
    public CharacterController characterController;
    public PlayerStateMachine playerStateMachine;
    
    

    public void Start()
    {
        

    }


    public void Update()
    {
        CheckForWall();
        if(lastJumpTime > 0f)
        {
            lastJumpTime -= Time.deltaTime;
        }
    }

    public void CheckForWall()
    {

        wallRight = Physics.Raycast(orientation.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        Debug.DrawRay(orientation.position, orientation.right * wallCheckDistance, Color.blue);


        wallLeft = Physics.Raycast(orientation.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
        Debug.DrawRay(orientation.position, -orientation.right * wallCheckDistance, Color.red);


        Debug.DrawRay(transform.position, Vector3.down * minJumpHeight, Color.magenta);
    }

    public bool AboveGround()
    {

        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    public bool HitWall()
    {
        if (lastJumpTime <=0 && (wallLeft || wallRight))
            return true;
        else
            return false;
    }

    public void WallRunningMovement()
    {

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);


        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        characterController.Move(wallForward * wallRunForce * Time.deltaTime);
        //push to wall
        characterController.Move(-wallNormal * 100 * Time.deltaTime);
        FaceMovement(wallForward, Time.deltaTime);        
    }

   

    protected void FaceMovement(Vector3 movement, float deltatime)
    {
        transform.rotation =
            Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(movement),
            deltatime * RotationSmoothValue);
    }

    public  void ResetWallJumpTime()
    {
        lastJumpTime = 1.5f;
    }

    public Vector3 WallJump()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);


        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;


        Vector3 ForceTopApply =  wallForward * wallJumpForwardForce + wallNormal * wallJumpSideForce;

        return ForceTopApply;
    }
}
