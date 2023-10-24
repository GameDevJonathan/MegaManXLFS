using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAudioManager;

public class MusicManager : MonoBehaviour
{

    public bool PlayMusic = true;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayMusic)
        UniversalAudioPlayer.PlayMusic(0);
    }

    
}
