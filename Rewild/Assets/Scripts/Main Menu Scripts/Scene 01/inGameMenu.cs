using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class inGameMenu : MonoBehaviour {
    public Animator Animator;
    public Canvas Canvas;
    private Vector3 finalCanvasPosition; //the position that my canvas is going to reach in the end of the animation 
    private Vector3 realCanvasPosition; // The real time position of my canvas 
    private Vector3 subtractionResult;
    private LineRenderer lineRenderer;
    // used to load the menu back 
    private Image loadingScreen;
    private float transaction;
    private bool startFadeIn;
    private bool startFadeOut;
    public float fadeTime;
    private bool executeTheIfStatement;
    private Scene currentScene; // used to check which scene is active
    private string sceneName;
    private bool loadTheMenu;
    private bool startFadeInRunnedOnce;

    void Start () {
        Animator.SetBool("isTheMenuCalled", false);
        finalCanvasPosition = new Vector3(0.0f, 0.7f, 0.0f); // the real value of the canvas is 0.635f , but i pass i bigger value to have a negative number as a result
        realCanvasPosition = new Vector3(0.0f, 0.0f, 0.0f);
        subtractionResult = new Vector3(0.0f, 55.0f, 0.0f); // just put  a value bigger than 51 in order not to triger the if statement on line 29
        // used to load the menu back 
        startFadeInRunnedOnce = false;
           startFadeIn = false;
        startFadeOut = false;
        executeTheIfStatement = false;
        loadingScreen = GameObject.Find("Fade/Canvas").GetComponentInChildren<Image>();
        loadTheMenu = false;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update ()
    {
        //  used for raycasting 
        RaycastHit hit;
        //  create a ray from the camera to where the mouse points to , should be replaced with the controller??
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //  make the ray visible in debug mode in the scene, need to find something else to make the ray visible in the game 
        Debug.DrawRay(ray.origin, ray.direction * 1000000.0f, Color.red);
        //  make the ray visible in game mode
        lineRenderer.SetPosition(0, ray.origin + new Vector3(1.0f, 0.0f, 0.0f));
        lineRenderer.SetPosition(1, ray.origin + ray.direction * 1000000.0f);


        realCanvasPosition = Canvas.transform.position;
                subtractionResult.y=realCanvasPosition.y - finalCanvasPosition.y;
       
        if((subtractionResult.y<52.3f) && Animator.GetBool("isTheMenuCalled"))
        {
           Time.timeScale = 0.0f; // if set to zero pauses everything that uses Delta Time.
        }

       // Debug.Log(subtractionResult.ToString());
		if (Canvas.gameObject.activeSelf)
		{
	        if (!Animator.GetBool("isTheMenuCalled") && (Canvas.transform.localPosition.y > 5.17f))
	        {
	            //Debug.Log(Canvas.transform.localPosition.y);
	            Canvas.gameObject.SetActive(false);
	        }
		}

		if (Input.GetButtonDown("Menu"))
        {
            Canvas.gameObject.SetActive(true);
			Debug.Log("Menu button pressed");
            if(Animator.GetBool("isTheMenuCalled"))
            {
                Time.timeScale = 1.0f;
                Animator.SetBool("isTheMenuCalled", false);
            }
            else
            {
                Animator.SetBool("isTheMenuCalled", true);
            }
        }

        if (Physics.Raycast(ray, out hit))
        {

            // changed everything to be working with buttons
            if (Input.GetButtonDown("Resume") && Animator.GetBool("isTheMenuCalled")&&hit.collider.CompareTag("InGameMenuResume"))// resume the game
            {
                theResumeButtonIsPressed();
            }

            // changed everything to be working with buttons
            if (Input.GetKeyDown(KeyCode.C) && Animator.GetBool("isTheMenuCalled") && hit.collider.CompareTag("InGameMenuBackToMainMenu"))// load the main menu
            {
                loadTheMenu = true;
                if((!startFadeIn)&&(!startFadeInRunnedOnce))
                {
                    startFadeIn = true;
                }

            }

            // changed everything to be working with buttons
            if (Input.GetKeyDown(KeyCode.C) && Animator.GetBool("isTheMenuCalled") && hit.collider.CompareTag("InGameMenuExit"))// exit the game
            {
                theExitGameIsPressed();
            }
        }

        if (loadTheMenu)
        {
            loadTheMainMenu();
        }
    }

    public void theResumeButtonIsPressed()
    {
		Debug.Log("START");
        Time.timeScale = 1.0f;
        Animator.SetBool("isTheMenuCalled", false);
    }

    public void theExitGameIsPressed()
    {
        Application.Quit();
        Debug.Log("The game has excited successfully");
    }

    public void loadTheMainMenu()
    {
        if (startFadeIn)
        {
            Debug.Log("04");
            transaction += Time.deltaTime / fadeTime;
            fadeIn();
            if (loadingScreen.color == Color.white) // fade in has  been completed , we can load the next scene now 
            {
                Debug.Log("05");
                startFadeIn = false;
                startFadeInRunnedOnce = true;
               executeTheIfStatement = true;
                SceneManager.LoadScene("Main Menu");
            }
        }

        if (executeTheIfStatement)
        {
            Debug.Log("06");
            // get the scene which is active right now.
            currentScene = SceneManager.GetActiveScene();
            Debug.Log(currentScene.name);
            sceneName = currentScene.name;

            if (sceneName == "Main Menu")
            {
                Debug.Log("07");
                startFadeOut = true;
                transaction = 0.0f;
                executeTheIfStatement = false;  // this statement will run only once

            }
        }

        if (startFadeOut)
        {
            Debug.Log("08");
            transaction += Time.deltaTime / fadeTime;
            fadeOut();

            if (loadingScreen.color == new Color(1.0f, 1.0f, 1.0f, 0.0f)) // fade out has  been completed
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

}
//Made by Panagiotis Katsiadramis 
//Modified by:  Panagiotis Katsiadramis 21/03/18