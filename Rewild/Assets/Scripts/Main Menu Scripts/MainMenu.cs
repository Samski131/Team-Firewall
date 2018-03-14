using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioEffects;
    private AudioSource audioMusic;
    public Scrollbar scrollbarMusic;
    public Scrollbar scrollbarEffects;
    public FadeManager fadeManager;
    


    private void Start()
    {
        

        audioMusic = GameObject.Find("Music/Audio Source").GetComponent<AudioSource>();
        
        animator.SetBool("startTheGame", false);
        animator.SetBool("fromMenuToOptions", false);
        animator.SetBool("fromOptionsToMenu", false);
        animator.SetBool("fromOptionsToSoundVolume", false);
        animator.SetBool("fromSoundVolumeToOptions", false);
        scrollbarEffects.value = 1.0f;
        scrollbarMusic.value = 0.3f;  // The range is between 0.0-1.0  , i set up the volume to 0.3 since 1.0 is too loud.
    }

    public void Update()
    {
        scrollbarSoundEffectsVolume();
        scrollbarMusicVolume();
    }




    public void startTheGameButtonIsPressed()
    {
        fadeManager.isTheCameraMoving = true;
        audioEffects.Play();
        animator.SetBool("startTheGame", true);
        //Debug.Log("The Scene loaded successfully");
    }


    private void scrollbarMusicVolume() // this changes the volume of the music 
    {
        audioMusic.volume = scrollbarMusic.value;
    }

    private void scrollbarSoundEffectsVolume()
    {
        audioEffects.volume = scrollbarEffects.value;
    }





    public void optionsButtonIsPressed()
    {
        animator.SetBool("startTheGame", false); // not necessary, just to be sure.


        animator.SetBool("fromMenuToOptions", true);
        animator.SetBool("fromOptionsToMenu", false);
        animator.SetBool("fromOptionsToSoundVolume", false);
        animator.SetBool("fromSoundVolumeToOptions", false);
        audioEffects.Play();
    }

    public void backToMainMenuButtonIsPressed()
    {
        animator.SetBool("startTheGame", false); // not necessary, just to be sure.
        animator.SetBool("fromMenuToOptions", false);
        animator.SetBool("fromOptionsToMenu", true);
        animator.SetBool("fromOptionsToSoundVolume", false);
        animator.SetBool("fromSoundVolumeToOptions", false);
        audioEffects.Play();
    }

    public void soundVolumeButtonIsPressed()
    {
        animator.SetBool("startTheGame", false); // not necessary, just to be sure.
        animator.SetBool("fromMenuToOptions", false);
        animator.SetBool("fromOptionsToMenu", false);
        animator.SetBool("fromOptionsToSoundVolume", true);
        animator.SetBool("fromSoundVolumeToOptions", false);
        audioEffects.Play();
    }

    public void backToOptionsButtonIsPressed() // from the Volume Options
    {
        animator.SetBool("startTheGame", false); // not necessary, just to be sure.
        animator.SetBool("fromMenuToOptions", false);
        animator.SetBool("fromOptionsToMenu", false);
        animator.SetBool("fromOptionsToSoundVolume", false);
        animator.SetBool("fromSoundVolumeToOptions", true);
        audioEffects.Play();
    }

    public void quitTheGameButtonIsPressed()
    {
        audioEffects.Play();
        Application.Quit();
        Debug.Log("The game did exit successfully");
    }
}
//Made by Panagiotis Katsiadramis 
//Modified by: 