using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAudioManager;
using UnityEngine.Animations.Rigging;
using Unity.VisualScripting;

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

    [field: Header("Weapons")]
    [field: SerializeField] public Transform FirePoint { get; private set; }
    [field: SerializeField] public Transform[] Sockets { get; private set; }
    [field: SerializeField] public GameObject[] BusterShot { get; private set; }
    [field: SerializeField] public LightSaber LightSaber { get; private set; }

    [field: Header("Inverse Kinimatics")]

    [field: SerializeField] public Rig rig;
    [field: SerializeField] public Transform RightHandPlacement { get; private set; }
    [field: SerializeField] public Transform RightHandHint { get; private set; }
    [field: SerializeField] public Transform AimTarget { get; private set; }




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

    public void SlotWeapon(int slot)
    {
        LightSaber.transform.parent = Sockets[slot].transform;
        switch (slot)
        {
            case 0:
                LightSaber.transform.localPosition = new Vector3(0.013f, -0.1f, -0.3f);
                LightSaber.transform.localRotation = Quaternion.identity;
                break;

            case 1:
                SaberOn();
                LightSaber.transform.localPosition = Vector3.zero;
                LightSaber.transform.localRotation = Quaternion.identity;
                break;

        }
        //LightSaber.TurnOn();
    }


    public void SaberOn()
    {
        LightSaber.TurnOn();
    }
   
}
