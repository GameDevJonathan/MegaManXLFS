using EasyAudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Audio : MonoBehaviour
{
    // C.G. --- JUST ADDED A FUNCTION TO PLAY A FOOTSTEP SOUND WHEN CHARACTER MOVES

    public void FootstepSFX()
    {
        UniversalAudioPlayer.PlayInGameSFX("Footstep");

    }
}
