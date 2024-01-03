using UnityEngine;

public class PlayerHyperBeam : PlayerBaseState
{
    private readonly int HyperBeamHash = Animator.StringToHash("HyperBeam");
    private const float CrossFadeDuration = 0.05f;
    private GameObject[] SpecialBeam;


    public PlayerHyperBeam(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.SpecialBeam = stateMachine.SpecialBeam;
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(HyperBeamHash, CrossFadeDuration);
        foreach (var part in SpecialBeam)
        {
            if (part.activeSelf == false)
            {
                part.SetActive(true);
            }
        }

    }

    public override void Tick(float deltaTime)
    {
        if (GetNormalizedTime(stateMachine.Animator, "HyperBeam") > 1f && SpecialBeam[0].activeSelf == false)
        {
            stateMachine.SwitchState(new Grounded(stateMachine,true));
        }
    }



    public override void Exit()
    {

    }
}
