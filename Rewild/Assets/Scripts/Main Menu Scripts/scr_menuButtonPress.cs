using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_menuButtonPress : MonoBehaviour {

	[Range(0.0f,1.5f)]
	public float musicVolume = 0.0f;
	[Range(0.0f,1.5f)]
	public float effectVolume = 0.0f;
	private MainMenu mainMenuScript;
	private GameObject musicScrollKnob;
	private GameObject effectScrollKnob;
	// Use this for initialization
	void Start () 
	{
		musicScrollKnob = GameObject.FindGameObjectWithTag("Music Scrollbar");
		effectScrollKnob = GameObject.FindGameObjectWithTag("Effects Scrollbar");
		mainMenuScript = GameObject.FindGameObjectWithTag("Main Menu").GetComponent<MainMenu>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log("This is Colliding");
		if (other.tag == "Left Hand")
		{
			
			if (this.tag == "Start")
			{
				Debug.Log("This is START");
				mainMenuScript.startTheGameButtonIsPressed();
			}
			else if(this.tag == "Quit")
			{
				mainMenuScript.quitTheGameButtonIsPressed();
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Left Hand")
		{
		if (this.tag == "Music Scrollbar" && Input.GetButton("Sprint"))
		{
			Debug.Log("Music");

			if (other.transform.position.z < 3.0f)
			{
				musicScrollKnob.transform.position = new Vector3(transform.position.x, transform.position.y, 3.0f);

			}
			else if (other.transform.position.z > 4.5f)
			{
				musicScrollKnob.transform.position = new Vector3(transform.position.x, transform.position.y, 4.5f);
			}
			else
			{
				musicScrollKnob.transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.position.z);

			}
			musicVolume = musicScrollKnob.transform.position.z -3.0f;
				mainMenuScript.scrollbarMusic.value = musicVolume / 1.5f;
		}

		if (this.tag == "Effects Scrollbar" && Input.GetButton("Sprint"))
		{
			Debug.Log("Effects");

			if (other.transform.position.z < 3.0f)
			{
				effectScrollKnob.transform.position = new Vector3(transform.position.x, transform.position.y, 3.0f);

			}
			else if (other.transform.position.z > 4.5f)
			{
				effectScrollKnob.transform.position = new Vector3(transform.position.x, transform.position.y, 4.5f);
			}
			else
			{
				effectScrollKnob.transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.position.z);

			}
			effectVolume = effectScrollKnob.transform.position.z -3.0f;
				mainMenuScript.scrollbarEffects.value = effectVolume / 1.5f;
			Debug.Log(effectVolume);
		}
		}
	}



}
