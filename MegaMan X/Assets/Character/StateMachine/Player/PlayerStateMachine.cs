using UnityEngine;
using EasyAudioManager;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;

public class PlayerStateMachine : StateMachine
{

    #region Components
    public Transform MainCameraTransform { get; private set; }
    [field: Header("Required Components")]
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public WallRun WallRun { get; private set; }
    [field: SerializeField] public Attacks[] Attacks { get; private set; }
    [field: SerializeField] public List<ParkourAction> ParkourActions { get; private set; }
    [field: SerializeField] public Targeter Targeter { get; private set; }
    [field: SerializeField] public EnviromentScaner EnviromentScaner { get; private set; }
    #endregion

    #region Weapons, Start Positions and IK

    [field: Header("Weapons")]
    [field: SerializeField] public Transform FirePoint { get; private set; }
    [field: SerializeField] public Transform[] Sockets { get; private set; }
    [field: SerializeField] public GameObject[] BusterShot { get; private set; }
    [field: SerializeField] public LightSaber LightSaber { get; private set; }

    [field: Header("Transforms For Start Positions")]
    [field: SerializeField] Transform[] startTransform;
    [SerializeField] enum enStartPositions { Stairs = 0, Highway, TollBooth }
    [SerializeField] enStartPositions enStartPosition = enStartPositions.TollBooth;


    [field: Header("Special Beam")]
    [field: SerializeField] public GameObject[] SpecialBeam { get; private set; }
    [field: SerializeField] public bool SpecialMove;

    [field: Header("Inverse Kinimatics")]

    [field: SerializeField] public Rig rig;
    [field: SerializeField] public Transform RightHandPlacement { get; private set; }
    [field: SerializeField] public Transform RightHandHint { get; private set; }
    [field: SerializeField] public Transform AimTarget { get; private set; }
    #endregion

    #region Movement Values
    [field: Space]
    [field: Header("Movement Values")]
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
    [field: SerializeField] public float LockOnMovementSpeed { get; private set; }
    [field: SerializeField] public float AirMovementSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RotationSmoothValue { get; private set; }

    [field: SerializeField] public float DashForceTime { get; private set; }
    [field: SerializeField] public float DashForce { get; private set; }
    #endregion

    #region Camera's and VFX

    [field: Header("Cameras")]
    [field: SerializeField] public GameObject _thirdPersonCam { get; private set; }
    [field: SerializeField] public GameObject _AimCam { get; private set; }
    [field: SerializeField] public GameObject _AimCamUtil { get; private set; }
    [field: SerializeField] public GameObject _TargetCamUtil { get; private set; }
    [field: SerializeField] public LayerMask aimColliderMask { get; private set; }
    [field: SerializeField] public LayerMask lockOnTargetColliderMask { get; private set; }
    [field: SerializeField] public bool DebugRayCastHit { get; private set; }
    [field: SerializeField] public Transform debugTransform { get; private set; }
    [field: SerializeField] public Transform LockOnSphere { get; private set; }

    [field: Header("VFX")]
    [field: SerializeField] public GameObject[] _thrusters { get; private set; }
    #endregion

    private void Awake()
    {
        GetComponents();
    }



    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        SwitchState(new Grounded(this));

        if (startTransform[0] != null)
        {
            switch (enStartPosition)
            {
                case enStartPositions.TollBooth:
                    transform.parent.position = startTransform[(int)enStartPositions.TollBooth].position;
                    break;

                case enStartPositions.Highway:
                    transform.parent.position = startTransform[(int)enStartPositions.Highway].position;
                    break;

                case enStartPositions.Stairs:
                    transform.parent.position = startTransform[(int)enStartPositions.Stairs].position;
                    break;
            }
        }
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

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {


        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
        {
            return;
        }

        Debug.Log(body);

        body.TryGetComponent<HighWayInteraction>(out HighWayInteraction highWayInteraction);

        if (highWayInteraction != null && highWayInteraction.Enemy == false)
            body.useGravity = true;



    }

    private void GetComponents()
    {
        InputReader = GetComponent<InputReader>();
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
        ForceReceiver = GetComponent<ForceReceiver>();
        WallRun = GetComponent<WallRun>();
        Targeter = GetComponentInChildren<Targeter>();
        EnviromentScaner = GetComponent<EnviromentScaner>();
    }

   


}
