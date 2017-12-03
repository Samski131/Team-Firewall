using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class inGameMenu : MonoBehaviour {
    public Animator Animator;
    public Canvas Canvas;
    private Vector3 finalCanvasPosition; //the position that my canvas is going to reach in the end of the animation 
    private Vector3 realCanvasPosition; // The real time position of my canvas 
    private Vector3 subtractionResult;

	void Start () {
        Animator.SetBool("isTheMenuCalled", false);
        finalCanvasPosition = new Vector3(0.0f, 0.7f, 0.0f); // the real value of the canvas is 0.635f , but i pass i bigger value to have a negative number as a result
        realCanvasPosition = new Vector3(0.0f, 0.0f, 0.0f);
        subtractionResult = new Vector3(0.0f, 55.0f, 0.0f); // just put  a value bigger than 51 in order not to triger the if statement on line 29
    }
	
	// Update is called once per frame
	void Update ()
    {

            realCanvasPosition = Canvas.transform.position;
                subtractionResult.y=realCanvasPosition.y - finalCanvasPosition.y;
       
        if((subtractionResult.y<52.3f) && Animator.GetBool("isTheMenuCalled"))
        {
           Time.timeScale = 0.0f; // if set to zero pauses everything that uses Delta Time.
        }

       // Debug.Log(subtractionResult.ToString());

        if (!Animator.GetBool("isTheMenuCalled") && (Canvas.transform.localPosition.y > 5.17f))
        {
            //Debug.Log(Canvas.transform.localPosition.y);
            Canvas.gameObject.SetActive(false);
        }

		if (Input.GetButtonDown("Menu"))
        {
            Canvas.gameObject.SetActive(true);

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

        // changed everything to be working with buttons
		if (Input.GetButtonDown("Menu")&& Animator.GetBool("isTheMenuCalled"))// resume the game
        {
            theResumeButtonIsPressed();
        }

        // changed everything to be working with buttons
        if (Input.GetKeyDown(KeyCode.C) && Animator.GetBool("isTheMenuCalled"))// exit the game
        {
            theExitGameIsPressed();
        }

    }

    public void theResumeButtonIsPressed()
    {
        Time.timeScale = 1.0f;
        Animator.SetBool("isTheMenuCalled", false);
    }

    public void theExitGameIsPressed()
    {
        Application.Quit();
        Debug.Log("The game has excited successfully");
    }
}
//Made by Panagiotis Katsiadramis 
//Modified by: 