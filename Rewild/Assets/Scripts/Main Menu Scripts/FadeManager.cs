using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class FadeManager : MonoBehaviour
{    
    private Image loadingScreen;
   
    private float transaction;
    private bool startFadeIn;
    private bool startFadeOut;
    private bool executeTheIfStatement;
   
    private Scene currentScene; // used to check which scene is active
    private string sceneName;
    public float fadeTime;
    public string sceneToLoad;

    // Use this for initialization
    void Start ()
    {
        startFadeIn = false;
        transaction = 0.0f;
        executeTheIfStatement = false;
        startFadeOut = false;
        loadingScreen = GameObject.Find("Fade/Canvas").GetComponentInChildren<Image>();
        Debug.Log("01");
    }
	
	// Update is called once per frame
	void Update ()
    {
      
      if(startFadeIn)
        {
            Debug.Log("04");
            transaction += Time.deltaTime / fadeTime;
            fadeIn();
            if (loadingScreen.color == Color.white) // fade in has  been completed , we can load the next scene now 
            {
                Debug.Log("05");
                startFadeIn = false;
				executeTheIfStatement = true;
				//SceneManager.LoadScene(sceneToLoad);
				SteamVR_LoadLevel.Begin(sceneToLoad, false, 0.5f, 0,0,0,1);
            }
        }

        if (executeTheIfStatement)
        {
            Debug.Log("06");
            // get the scene which is active right now.
            currentScene = SceneManager.GetActiveScene();
			Debug.Log (currentScene.name);
            sceneName = currentScene.name;

            if(sceneName == sceneToLoad)
            {
                Debug.Log("07");
                startFadeOut = true;
                transaction = 0.0f;
                executeTheIfStatement = false;  // this statement will run only once
             
            }
        }

        if(startFadeOut)
        {
            Debug.Log("08");
            transaction += Time.deltaTime / fadeTime;
            fadeOut();

            if (loadingScreen.color == new Color(1.0f,1.0f,1.0f,0.0f)) // fade out has  been completed
            {
                Debug.Log("09");
                startFadeOut = false;
                //Destroy(loadingScreen.transform.parent);
                Destroy(gameObject);
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

    public void SetFadeInTrue()
    {
        startFadeIn = true;
    }
}
//Made by Panagiotis Katsiadramis 
//Modified by: