using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using EasyAudioManager;
public class DemoScene : MonoBehaviour
{
   
    public void PlayInGameSFX(string SFXName)
    {
        UniversalAudioPlayer.PlayInGameSFX(SFXName);
    }

    public void PlayUISFX(string SFXName)
    {
        UniversalAudioPlayer.PlayUISound(SFXName);
    }

    public void PlayMusic(int audioTrackID)
    {
        UniversalAudioPlayer.PlayMusic(audioTrackID); 
    }

    public void StopMusic() 
    {
        UniversalAudioPlayer.StopMusic();
    }

    public void PlayVO(int VOID)
    {
        UniversalAudioPlayer.PlayVO(VOID);
    }

    public void StopVO()
    {
        UniversalAudioPlayer.StopVO();
    }


}
