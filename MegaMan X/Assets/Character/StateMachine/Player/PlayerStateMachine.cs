using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Transform MainCameraTransform { get; private set; }
    [field: Header("Required Components")]
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public WallRun WallRun { get; private set; }
    [field: SerializeField] public Attacks[] Attacks { get; private set; }

    [field: SerializeField] public Transform FirePoint { get; private set; }
    [field: SerializeField] public GameObject[] BusterShot { get; private set; }

    [field: Space]
    [field: Header("Movement Values")]
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RotationSmoothValue { get; private set; }

    [field: SerializeField] public float DashForceTime { get; private set; }
    [field: SerializeField] public float DashForce { get; private set; }

    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        SwitchState(new Grounded(this));
    }

    public void ChargedLevel()
    {
        if(InputReader.chargedShot)
        Instantiate(BusterShot[2], FirePoint.position, FirePoint.rotation);
        else if(InputReader.mediumShot)
        Instantiate(BusterShot[1], FirePoint.position, FirePoint.rotation);
        else
        Instantiate(BusterShot[0], FirePoint.position, FirePoint.rotation);

    }
    
    public void FireBullet()
    {
        Invoke("ChargedLevel", 0);

    }
}
