using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyAudioManager;


public class Audio : MonoBehaviour
{
    // C.G. --- JUST ADDED A FUNCTION TO PLAY A FOOTSTEP SOUND WHEN CHARACTER MOVES

    public void Start()
    {
        //UniversalAudioPlayer.PlayMusic(0);
    }


    public void FootstepSFX()
    {
        UniversalAudioPlayer.PlayInGameSFX("Footstep");
    }    
}
