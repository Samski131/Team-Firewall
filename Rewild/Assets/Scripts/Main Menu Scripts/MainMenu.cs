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
    private LineRenderer lineRenderer;
    public GameObject rightController;
    private Camera rightControllerCamera;
    private Vector3 rightControllerPosition;
    private Transform rightControllerTransform;

	public Vector3 originOffset;
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
		originOffset = new Vector3(0.0f, 0.0f, 0.0f);
        //  used to draw the ray in the game 
        //rightController = GameObject.FindGameObjectWithTag("RightHand");
		rightControllerPosition = rightController.transform.position;
        rightControllerCamera = rightController.GetComponent<Camera>();
        lineRenderer= rightController.GetComponent<LineRenderer>();
        rightControllerTransform = rightController.transform;
       // lineRenderer = GetComponent<LineRenderer>(); // nt used any more
        lineRenderer.enabled = true;
        lineRenderer.useWorldSpace = true;
    }


    public void Update()
    {
        //  used for raycasting 
        RaycastHit hit;
        //  create a ray from the camera to where the mouse points to , should be replaced with the controller??
       // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        rightControllerTransform = rightController.transform;
        rightControllerPosition = rightControllerTransform.position;
        Ray ray = rightControllerCamera.ScreenPointToRay(rightControllerPosition);
        //  make the ray visible in debug mode in the scene, need to find something else to make the ray visible in the game 
        Debug.DrawRay(ray.origin, ray.direction * 1000000.0f , Color.red);
        //  make the ray visible in game mode
		lineRenderer.SetPosition(0, ray.origin + new Vector3(0.27f, -1.85f, 0.0f));
		lineRenderer.SetPosition(1, ray.origin + new Vector3(0.27f, -1.85f, 0.0f) + ray.direction * 1000000.0f);


		// 0.27  -1.85
        if(Physics.Raycast(ray, out hit))
        {

           // if (Input.GetButtonDown("Fire1"))   //check if we have collision only if the maouse is clicked for eficiency // should br replaced with the controller button.
           // {
                //  if we have an intersection 
                if (hit.collider.CompareTag("Start"))
                {
				Debug.Log("Hit Start");
                    startTheGameButtonIsPressed();
                }

                if (hit.collider.CompareTag("Options"))
			{
				Debug.Log("Hit Option");
                    optionsButtonIsPressed();
                }

                if (hit.collider.CompareTag("Quit"))
			{
				Debug.Log("Hit Quit");
                    quitTheGameButtonIsPressed();
                }

                if (hit.collider.CompareTag("Back"))
                {
                    backToMainMenuButtonIsPressed();
                }

                if (hit.collider.CompareTag("SoundOptions"))
                {
                    soundVolumeButtonIsPressed();
                }

                if (hit.collider.CompareTag("BackToOptions"))
                {
                    backToOptionsButtonIsPressed();
                }

                if (hit.collider.CompareTag("MusicVolume")) // increase the sound volume
                {
                    scrollbarMusic.value += 0.1f;
                }

                if (hit.collider.CompareTag("SoundEffects")) // increase the sound Effects Volume
                {
                    scrollbarEffects.value += 0.1f;
                }
           //}
            // decrease the sound 
            if (Input.GetButtonDown("Fire1"))
            {
                if (hit.collider.CompareTag("MusicVolume")) // increase the sound volume
                {
                    scrollbarMusic.value -= 0.1f;
                }

                if (hit.collider.CompareTag("SoundEffects")) // increase the sound Effects Volume
                {
                    scrollbarEffects.value -= 0.1f;
                }
            }
        }
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
//Modified by:  Panagiotis Katsiadramis 14/03/18
//Modified by:  Panagiotis Katsiadramis 21/03/18