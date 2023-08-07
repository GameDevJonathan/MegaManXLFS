using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAudioManager
{
    public class AudioManager : MonoBehaviour
    {


        [Header("Music Tracks")]
        public AudioAsset[] MusicList;
        [Header("Voice Over Clips")]
        public AudioAsset[] VOList;
        [Header("In Game SFX")]
        public AudioAsset[] InGameSFXList;
        [Header("UI SFX")]
        public AudioAsset[] uiSFXList;

        //Audio Source Object
        private AudioSource source;


        //Global Variables
        [HideInInspector] public AudioSource ASM;
        [HideInInspector] public AudioSource ASV;
        [HideInInspector] public GameObject music;
        [HideInInspector] public GameObject VO;

        private void Start()
        {
            GameObject audioSources = new GameObject();
            audioSources.name = "Audio Sources";

            //Create Game Objects for Music and VO
            if (music == null)
            {
                music = new GameObject();
                music.name = "Music";
                music.transform.parent = audioSources.transform;

                ASM = music.AddComponent<AudioSource>();
            }
            if (VO == null)
            {
                VO = new GameObject();
                VO.name = "VoiceOver";
                VO.transform.parent = audioSources.transform;

                ASV = VO.AddComponent<AudioSource>();
            }






        }

        void Awake()
        {
            UniversalAudioPlayer.audioManager = this;
            source = GetComponent<AudioSource>();

        }

        public void playInGameSFX(string name)
        {
            foreach (AudioAsset s in InGameSFXList)
            {
                if (s.name == name)
                {
                    // this is used for if you have multiple sound effects for a specific in game action. Think punch / kick / hurt sound effects!
                    int rand = Random.Range(0, s.clip.Length);
                    source.PlayOneShot(s.clip[rand]);
                    source.outputAudioMixerGroup = s.mixerGroup;
                }
            }
        }

        public void playUISFX(string name)
        {
            foreach (AudioAsset s in uiSFXList)
            {
                if (s.name == name)
                {
                    source.PlayOneShot(s.clip[0]);
                    source.outputAudioMixerGroup = s.mixerGroup;
                }
            }
        }

        public void playMusic(string name, int audioID)
        {
            foreach (AudioAsset s in MusicList)
            {
                if (s.name == name)
                {

                    ASM.clip = s.clip[audioID];
                    ASM.loop = true;
                    ASM.outputAudioMixerGroup = s.mixerGroup;
                    ASM.Play();
                }
            }
        }

        public void stopMusic(string name)
        {
            foreach (AudioAsset s in MusicList)
            {
                if (s.name == name)
                {
                    if (ASM.isPlaying)
                    {
                        ASM.Stop();
                    }



                }
            }
        }


        public void playVO(string name, int audioID)
        {
            foreach (AudioAsset s in VOList)
            {
                if (s.name == name)
                {
                    ASV.clip = s.clip[audioID];
                    ASV.loop = false;
                    ASV.outputAudioMixerGroup = s.mixerGroup;
                    ASV.Play();
                }
            }
        }

        public void stopVO(string name)
        {
            foreach (AudioAsset s in VOList)
            {
                if (s.name == name)
                {

                    if (ASV.isPlaying)
                    {
                        ASV.Stop();
                    }

                }
            }
        }
    }

}