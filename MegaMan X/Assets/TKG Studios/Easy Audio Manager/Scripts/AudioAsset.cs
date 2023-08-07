using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace EasyAudioManager
{

    [System.Serializable]
    public class AudioAsset
    {
        public string name;
        [HideInInspector] public float volume = 1f;
        public AudioClip[] clip;
        public AudioMixerGroup mixerGroup;

    }
}
