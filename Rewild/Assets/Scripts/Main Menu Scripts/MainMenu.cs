using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   
   // public AudioSource audioEffects;
    private AudioSource audioMusic;
    public Scrollbar scrollbarMusic;
    public Scrollbar scrollbarEffects;
    public FadeManager fadeManager;
    private LineRenderer lineRenderer;
    private FadeManager FadeManagerScrypt;
    
    private void Start()
    {
        FadeManagerScrypt = GameObject.Find("FadeManager").GetComponent<FadeManager>();
        audioMusic = GameObject.Find("Music/Audio Source").GetComponent<AudioSource>();
        
        //scrollbarEffects.value = 1.0f;
      //  scrollbarMusic.value = 0.3f;  // The range is between 0.0-1.0  , i set up the volume to 0.3 since 1.0 is too loud.
    }


    public void Update()
    {




      /*  
       * if(Input.GetKeyDown(KeyCode.X)) // start the game 
            {
            startTheGameButtonIsPressed();
            }

        if (Input.GetKeyDown(KeyCode.Z))  
        {
            quitTheGameButtonIsPressed();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            scrollbarMusic.value += 0.1f;
            scrollbarMusicVolume();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            scrollbarMusic.value -= 0.1f;
            scrollbarMusicVolume();
        }

	
	*/
	}
    public void startTheGameButtonIsPressed()
    {
		Debug.Log("START");
        FadeManagerScrypt.SetFadeInTrue();
        //Debug.Log("The Scene loaded successfully");
    }

    private void setScrollbarMusicVolume() // this changes the volume of the music 
    {
        audioMusic.volume = scrollbarMusic.value;
    }

    private void scrollbarSoundEffectsVolume()
    {
        //audioEffects.volume = scrollbarEffects.value;
    }

    public void quitTheGameButtonIsPressed()
    {
  
        Application.Quit();
        Debug.Log("The game did exit successfully");
    }
}
//Made by Panagiotis Katsiadramis 
//Modified by:  Panagiotis Katsiadramis 14/03/18
//Modified by:  Panagiotis Katsiadramis 21/03/18