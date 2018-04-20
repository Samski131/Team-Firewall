using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class inGameFadeManager : MonoBehaviour {

    public Image loadingScreen; // in my case its going to be a black
    private float transaction;
    private bool startFadeIn;
    private bool startFadeOut;
    private Scene currentScene; // used to check which scene is active
    private string sceneName;
    public float fadeTime;
    private bool executeTheIfStatement;
    public Animator Animator;

    // Use this for initialization
    void Start()
    {
        startFadeIn = false;
        startFadeOut = false;
        transaction = 0.0f;
        executeTheIfStatement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startFadeIn)
        {
            //Debug.Log("01");
            transaction += Time.deltaTime / fadeTime;
            fadeIn();
            if (loadingScreen.color == new Color(0.0f,0.0f,0.0f,1.0f)) // fade in has  been completed , we can load the next scene now 
            {
               // Debug.Log("02");
                startFadeIn = false;
                executeTheIfStatement = true;
                SceneManager.LoadScene("Main Menu");
            }
        }


        if (executeTheIfStatement)
        {
            // get the scene which is active right now.
            currentScene = SceneManager.GetActiveScene();
            sceneName = currentScene.name;
            //Debug.Log("03");

            if (sceneName == "Main Menu")
            {
              //  Debug.Log("04");

                startFadeOut = true;
                transaction = 0.0f;
                executeTheIfStatement = false;  // this statement will run only once
            }
        }




        if (startFadeOut)
        {
            transaction += Time.deltaTime / fadeTime;
            fadeOut();
           // Debug.Log("05");

            if (loadingScreen.color == new Color(0.0f, 0.0f, 0.0f, 0.0f)) // fade out has  been completed
            {
              //  Debug.Log("06");

                startFadeOut = false;
                Destroy(loadingScreen);
            }
        }


        //used to make the menu work with buttons
        if (Input.GetKeyDown(KeyCode.N) && Animator.GetBool("isTheMenuCalled"))// Load the Main Menu
        {
            Time.timeScale = 1.0f;
            Animator.SetBool("isTheMenuCalled", false);
            StartFadeIn();
            Debug.Log("Button N worked");

        }
    }


    public void StartFadeIn()
    {
        Debug.Log("fade in bool set to true");
        startFadeIn = true;
    }



    private  void fadeIn()
    {
        Debug.Log("fade in function called");
        loadingScreen.color = Color.Lerp(new Color(0.0f, 0.0f, 0.0f, 0.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f), transaction);
    }

    private void fadeOut()
    {
        loadingScreen.color = Color.Lerp(new Color(0.0f, 0.0f, 0.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 0.0f), transaction);
    }

}
//Made by Panagiotis Katsiadramis 
//Modified by: 