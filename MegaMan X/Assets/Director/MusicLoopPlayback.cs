using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Add the unity namespace
using UnityEngine.Playables;

public class MusicLoopPlayback : MonoBehaviour
{
    //create a serialized field so i can access the director
    [SerializeField] private PlayableDirector _playableDirector;
    //float to loop back to the playback time.
    [SerializeField] private float _playBackTime;    

    //public function to be called within the emitter
    public void Playback()
    {
        //set the time of the director
        _playableDirector.time = _playBackTime;
        //start to play from this point.
        _playableDirector.Play();
    }
}
