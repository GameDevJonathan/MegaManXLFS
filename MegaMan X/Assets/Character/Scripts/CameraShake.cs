using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _source;
    // Start is called before the first frame update
    

    public void Shake()
    {
        _source.GenerateImpulse();
    }
}
