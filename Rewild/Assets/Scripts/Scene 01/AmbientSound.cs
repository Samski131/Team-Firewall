using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSound : MonoBehaviour {
    
    public AudioMixerSnapshot [] soundInsideTheArea;
    public AudioMixerSnapshot [] soundOutsideTheArea;
    public float transitionTime;
    public AudioClip[] SoundsToPlay;
    public bool [] LoopTheSong; // tick if you want to loop the song , first bool pairs with the first song and so on.
    public AudioMixerGroup[] audioMixer; // Place the mixer with the right order as the songs, ex. first mixer pairs with the first song.
    private AudioSource[] audioSource; 
   
    private void Start()
    {
        Debug.Log("start 1");  
        if (SoundsToPlay.Length > 0)
        {
            Debug.Log("start 2");
            audioSource = new AudioSource[SoundsToPlay.Length];
           
            for (int i=0; i < SoundsToPlay.Length; i++ )
            {
                audioSource[i] = gameObject.AddComponent<AudioSource>();

                Debug.Log("start 3 megetos = " + audioSource.Length);
                if (LoopTheSong[i])
                {
                    audioSource[i].loop = true;
                }
                else
                {
                    audioSource[i].loop = false;
                }

                // send the output of the source to the audio mixer
                audioSource[i].outputAudioMixerGroup = audioMixer[i];


                // set the sound to be played by the audio source.
                audioSource[i].clip = SoundsToPlay[i];

                // start playing the sound
                audioSource[i].volume = 1.0f;
                audioSource[i].Play();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
      

        if (soundInsideTheArea.Length !=0.0f  )
        {
            for (int i=0; i < soundInsideTheArea.Length; i++ )
            {
                soundInsideTheArea[i].TransitionTo(transitionTime);
                Debug.Log("malakia!!"); 
            }
        }
        
    }

     void OnTriggerExit(Collider other)
    {

        if (soundInsideTheArea.Length != 0.0f)
        {
            for (int i = 0; i < soundOutsideTheArea.Length; i++)
            {
                soundOutsideTheArea[i].TransitionTo(transitionTime);
            }
        }
    }
}

//created by panos Katsiadramis  31/01/18
// modified : panos Katsiadramis  5/02/18