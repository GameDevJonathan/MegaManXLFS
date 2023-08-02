using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class WallRunning : MonoBehaviour
{
    public float wallMaxDistance = 1;
    public float wallSpeedMultiplier = 1.2f;
    public float minimumHeight = 1.2f;
    public float maxAngeRoll = 20;
    [Range(0f, 1f)]
    public float normalizedAngleThreshold = 0.1f;

    public float jumpDuration = 1;
    public float wallBouncine = 3;
    public float cameraTransitionDuration = 1;

    public float wallGravityDownForce = 20f;

    public bool useSprint;

    public Volume wallRunVolume;    

    Vector3[] directions;
    RaycastHit[] hits;

    bool isWallRunning = false;
    Vector3 lastWallPosition;





    [Header("Input")]
    public float horizontalInput;
    public float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    

    [Header("References")]
    public CharacterController characterController;
    public Transform orientation;
    public Controller controller;





    



    void OnDrawGizmos()
    {
        //left
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(orientation.position, orientation.position + (-orientation.right * wallCheckDistance));

        //right
        Gizmos.color = Color.red;
        Gizmos.DrawLine(orientation.position, orientation.position + (orientation.right * wallCheckDistance));

        //down
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(orientation.position, orientation.position + (Vector3.down * minJumpHeight));



    }
}
