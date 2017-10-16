using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{    
    private Image loadingScreen;
    private GameObject myCamera;
    private float transaction;
    private bool startFadeIn;
    private bool startFadeOut;
    private bool executeTheIfStatement;
    private Vector3 cameraPosition;
    private Vector3 requiredPosition; // The position that when the camera will reach , will trigger the fade in function.
    private float zDifference;
    public bool isTheCameraMoving { get; set; } // this bool is set to true in the MainMenu script , in order not to consume cpu resources without reason on math operations.
    private Scene currentScene; // used to check which scene is active
    private string sceneName;
    public float fadeTime;

    // Use this for initialization
    void Start ()
    {
        isTheCameraMoving = false;
        startFadeIn = false;
        requiredPosition = new Vector3(653.0f, 105.0f, 592.0f);  // on this coordinates the Z compoment is the one that interests us
        myCamera = GameObject.Find("Camera");
        cameraPosition = myCamera.transform.position;
        zDifference = 1.0f;
        transaction = 0.0f;
        executeTheIfStatement = false;
        startFadeOut = false;

        loadingScreen = GameObject.Find("Fade/Canvas").GetComponentInChildren<Image>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isTheCameraMoving)  
        {
            cameraPosition = myCamera.transform.position;
            if((cameraPosition.z - requiredPosition.z) >= zDifference)
            {
                startFadeIn = true;
                isTheCameraMoving = false;
            }
        }

      if(startFadeIn)
        {
            transaction += Time.deltaTime / fadeTime;
            fadeIn();
            if (loadingScreen.color == Color.white) // fade in has  been completed , we can load the next scene now 
            {
                startFadeIn = false;
                executeTheIfStatement = true;
                SceneManager.LoadScene("Scene01");
            }
        }

        if (executeTheIfStatement)
        {
            // get the scene which is active right now.
            currentScene = SceneManager.GetActiveScene();
            sceneName = currentScene.name;

            if(sceneName == "Scene01")
            {
                startFadeOut = true;
                transaction = 0.0f;
                executeTheIfStatement = false;  // this statement will run only once
            }
        }

        if(startFadeOut)
        {
            transaction += Time.deltaTime / fadeTime;
            fadeOut();

            if (loadingScreen.color == new Color(1.0f,1.0f,1.0f,0.0f)) // fade out has  been completed
            {
                startFadeOut = false;
                Destroy(loadingScreen);
            }


        }
    }

    private void fadeIn()
    {
        loadingScreen.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 0.0f), Color.white, transaction);
    }

    private void fadeOut()
    {
        loadingScreen.color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), transaction);
    }
}
//Made by Panagiotis Katsiadramis 
//Modified by: 