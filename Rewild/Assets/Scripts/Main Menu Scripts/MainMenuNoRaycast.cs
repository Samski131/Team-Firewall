using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuNoRaycast : MonoBehaviour {

    // public Animator animator;
    public AudioSource audioEffects;
    private AudioSource audioMusic;
    public Scrollbar scrollbarMusic;
    public Scrollbar scrollbarEffects;
    public FadeManager fadeManager;


    private void Start()
    {
        audioMusic = GameObject.Find("Music/Audio Source").GetComponent<AudioSource>();

        scrollbarEffects.value = 1.0f;
        scrollbarMusic.value = 0.3f;  // The range is between 0.0-1.0  , i set up the volume to 0.3 since 1.0 is too loud.

        //  used to draw the ray in the game 

    }


    public void Update()
    {

        scrollbarMusic.value += 0.1f;



        scrollbarEffects.value += 0.1f;


        if (Input.GetButtonDown("Fire1"))
        {

            scrollbarMusic.value -= 0.1f;



            scrollbarEffects.value -= 0.1f;
        }



    }

        public void startTheGameButtonIsPressed()
        {
            //  fadeManager.
            //  audioEffects.Play();


        }

        private void scrollbarMusicVolume() // this changes the volume of the music 
        {
            audioMusic.volume = scrollbarMusic.value;
        }

        private void scrollbarSoundEffectsVolume()
        {
            audioEffects.volume = scrollbarEffects.value;
        }

        public void quitTheGameButtonIsPressed()
        {
            audioEffects.Play();
            Application.Quit();
            Debug.Log("The game did exit successfully");
        }
    
}