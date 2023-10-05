using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAudioManager;
using System;
using UnityEngine.UIElements;
using System.Net.Sockets;

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

    [field:Header("Weapons")]
    [field: SerializeField] public Transform FirePoint { get; private set; }
    [field: SerializeField] public Transform[] Sockets { get; private set; }
    [field: SerializeField] public GameObject[] BusterShot { get; private set; }
    [field: SerializeField] public LightSaber LightSaber { get; private set; }
    [SerializeField] public enum EquipSocket {Back, Hand};
    [field: SerializeField] public EquipSocket Socket { get; private set; } = EquipSocket.Back;
    


    [field: Space]
    [field: Header("Movement Values")]
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RotationSmoothValue { get; private set; }

    [field: SerializeField] public float DashForceTime { get; private set; }
    [field: SerializeField] public float DashForce { get; private set; }

    [field: Header("Cameras")]
    [field: SerializeField] public GameObject _thirdPersonCam { get; private set; }
    [field: SerializeField] public GameObject _AimCam { get; private set; }
    [field: SerializeField] public LayerMask aimColliderMask { get; private set; }
    [field: SerializeField] public Transform debugTransform { get; private set; }


    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        SwitchState(new Grounded(this));
    }

    public void ChargedLevel()
    {
        if (InputReader.chargedShot)
        {
            Instantiate(BusterShot[2], FirePoint.position, FirePoint.rotation);
            UniversalAudioPlayer.PlayInGameSFX("MaxShot");
            return;


        }
        else if (InputReader.mediumShot)
        {
            Instantiate(BusterShot[1], FirePoint.position, FirePoint.rotation);
            UniversalAudioPlayer.PlayInGameSFX("ChargedShot");
            return;

        }
        else
        {
            Instantiate(BusterShot[0], FirePoint.position, FirePoint.rotation);
            UniversalAudioPlayer.PlayInGameSFX("BusterShot");
            return;

        }

    }

    public void FireBullet()
    {
        Invoke("ChargedLevel", 0);

    }

    public void WeaponSocket(string socket)
    {
        socket.ToLower();
        switch (socket)
        {
            case "back":
                Socket = EquipSocket.Back;
                break;

            case "hand":
                Socket = EquipSocket.Hand;
                break;
        }
        
        

    }
}
