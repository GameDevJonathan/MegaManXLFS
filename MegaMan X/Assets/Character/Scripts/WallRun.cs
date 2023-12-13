using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public LayerMask GroundCheck;

    public float wallRunForce;
    public float wallRunTime;
    public float maxWallRunTime;
    public float wallRunTimer;

    [Header("WallJumping")]
    public float wallJumpForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallJumpForwardForce;
    public float wallJumpUpForwardForce;
    
    [Header("WallJump delay")]
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
    [SerializeField] public RaycastHit frontWallHit;
    public bool wallLeft;
    public bool wallRight;
    public bool wallFront;

    [Header("Ground Check")]
    public float debugWireSphere_Radius;
    public Color debugWireSphere_Color;
    public Transform debugWireSphere_Transform;
    

    [Header("References")]
    public Transform orientation;
    public CharacterController characterController;
    public PlayerStateMachine playerStateMachine;
    
    



    public void Update()
    {
        //Debug.Log($"WallRun Script:: Touching Ground {AboveGround()}");


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
        
        wallFront = Physics.Raycast(orientation.position, orientation.forward, out frontWallHit, wallCheckDistance, whatIsWall);
        Debug.DrawRay(orientation.position, orientation.forward * wallCheckDistance, Color.white); 


        Debug.DrawRay(transform.position, Vector3.down * minJumpHeight, Color.magenta);
    }

    public bool AboveGround()
    {

        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    public bool CheckForGround()
    {
        return Physics.CheckSphere(debugWireSphere_Transform.position, debugWireSphere_Radius, GroundCheck);
    }

    public bool HitWall()
    {
        if (lastJumpTime <=0 &&  wallFront/*(wallLeft || wallRight || wallFront)*/)
            return true;
        else
            return false;
    }

    public void WallHangMovement()
    {

        Vector3 wallNormal = frontWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        characterController.Move(-wallNormal * 100 * Time.deltaTime);
        //FaceMovement(wallForward, Time.deltaTime);
        FaceMovement(-wallNormal, Time.deltaTime);
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

    public  void ResetWallJumpTime(float time = 1.5f)
    {
        lastJumpTime = time;
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

    public Vector3 WallJumpUp()
    {
        Vector3 wallNormal = frontWallHit.normal;


        Vector3 ForceTopApply = wallNormal * wallJumpUpForwardForce + Vector3.up;

        return ForceTopApply;
    }

    private void OnDrawGizmos()
    {
        if (debugWireSphere_Transform)
        {
            Gizmos.color = debugWireSphere_Color;
            Gizmos.DrawWireSphere(debugWireSphere_Transform.position, debugWireSphere_Radius);

        }
                
    }
}
