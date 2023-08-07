using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


namespace EasyAudioManager
{
    public class UniversalAudioPlayer : MonoBehaviour
    {
        public static AudioManager audioManager;

        public static void PlayInGameSFX(string sfxName)
        {
            if (audioManager != null && sfxName != "") audioManager.playInGameSFX(sfxName);
        }

        public static void PlayUISound(string sfxName)
        {
            if (audioManager != null && sfxName != "") audioManager.playUISFX(sfxName);
        }

        public static void PlayVO(int voID)
        {
            if (audioManager != null)
            {

                audioManager.stopVO("VoiceOver");
                audioManager.playVO("VoiceOver", voID);
            }
        }

        public static void PlayMusic(int audioTrackID)
        {
            if (audioManager != null)
            {

                audioManager.stopMusic("Music");
                audioManager.playMusic("Music", audioTrackID);
            }
        }

        public static void StopMusic()
        {
            audioManager.stopMusic("Music");
        }

        public static void StopVO()
        {
            audioManager.stopVO("VoiceOver");
        }
    }

}