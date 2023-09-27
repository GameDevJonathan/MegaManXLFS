using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAudioManager;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UniversalAudioPlayer.PlayMusic(0);
    }

    
}
