using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float drag = 0.3f;
    [SerializeField] private bool isGrounded;
    [SerializeField] public bool useGravity = true;

    private Vector3 dampingVelocity;
    private Vector3 impact;
    private float verticalVelocity = 0f;
    [SerializeField] private float gravity;
    [SerializeField] private float Normal = 30f;
    [SerializeField] private float wallSlideSpeed = 1f;
    public Vector3 Movement => impact + Vector3.up * verticalVelocity;

    private void Start()
    {
        gravity = Normal;
    }

    private void Update()
    {

        if (useGravity)
        {
            if (verticalVelocity < 0f && controller.isGrounded)
            {
                verticalVelocity = -gravity * Time.deltaTime;

            }
            else
            {
                verticalVelocity += -gravity * Time.deltaTime;
            }
            
        }



        isGrounded = controller.isGrounded;


        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

        if (agent != null)
        {
            if (impact.sqrMagnitude <= 0.2f * 0.2f)
            {
                impact = Vector3.zero;
                agent.enabled = true;
            }
        }

    }

    public void Reset()
    {
        impact = Vector3.zero;
        verticalVelocity = 0f;
    }

    public void AddForce(Vector3 force)
    {
        impact += force;
        //Debug.Log($"force direction: {force}");
        if (agent != null)
        {
            agent.enabled = false;
        }
    }
    

    public void Jump(float jumpForce)
    {
        verticalVelocity += jumpForce;
    }

   
    /// <summary>    
    ///  Sets Gravity 
    ///  0: Normal Gravity 1: Wall Slide
    /// </summary>
    
    
    public void SetGravity(int setGravity)
    {
        switch (setGravity)
        {
            case 0:
                gravity = Normal; 
                break;
            case 1:
                gravity = wallSlideSpeed;
                break;

        }

    }


   
}
   
