/*
 * Created by Jihad El Sheikh
 * 
 * This script handles the display of the information windows.
 * It reads the information from a file and stores them in an array.
 * It pops up the info window when the button is pressed.
 * It also closes it if the button is pressed, if the camera is focusing
 * on the same object.
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	printedInfo.txt
 * 
 */


using UnityEngine;
using System.Collections;

//This is needed to use List with <Rect>
using System.Collections.Generic;


public class InfoWindows : MonoBehaviour {
	
	List<Rect> myList = new List<Rect>();	//holds the windows
	List<string>names = new List<string>();	//holds the ids of the planets
	string focused;							//holds the focus of the main camera
	GameObject camera;
	string[] InfoButton = {"Show Info", "Hide Info"};	//Text on the button alternate
	string[] printedInfo = new string[10];	//holds the information of all planets
	int InfoPresent;						//boolean. whether a window is displayed for this object
	public jumpingCam target;				//to read a variable from another script

	void OnGUI() {

		//read the id of the object the camera is viewing
		focused = GameObject.Find("Main Camera").GetComponent<jumpingCam> ().camTarget;

		//check if the window is displayed (if the id is in 'names')
		if (names.Contains (focused)) {
			InfoPresent = 1;
		} else {
			InfoPresent = 0;
		}

		//Create the button to display/hide the window
		if (GUI.Button (new Rect (10, 100, 80, 25), InfoButton[InfoPresent])) {
			CreateWindow();
			
		}

		//display all the windows that have been poped up
		for (int i=0; i<myList.Count; i++) {
			myList [i] = GUI.Window (int.Parse(names[i]), myList [i], DoMyWindow, getInfo(int.Parse ( names[i] )));
				}
	}

	//sets some properties for the pop up windows
	void DoMyWindow(int windowID) {
		//GUI.DragWindow(new Rect(0, 0, 10000, 20)); //Only the top bar is dragable
		GUI.DragWindow(); //This makes the whole window dragable
	}

	//adds a window to the list if it doesn't exist
	//or removes it if it exists
	void CreateWindow(){
		camera = GameObject.Find ("Main Camera");	//hold the camera object
		focused = camera.GetComponent<jumpingCam> ().camTarget; //determine the object in view

		//add the object if it is not in the list
		//otherwise, remove it
		if (!names.Contains (focused)) {
			myList.Add (new Rect (100, 100, 200, 150));
			names.Add (focused);
		} else {
			int i;
			i = names.IndexOf(focused);
			names.RemoveAt(i);
			myList.RemoveAt(i);
		}
	}

	//match the id of the planet to the index in the lists
	//return the infomation to be displayed
	string getInfo(int planet){
		int index;
		index = planet / 100;

		if (planet > 350) {
			index++;
		}
		return printedInfo [index];
	}

	// Use this for initialization
	void Start () {
		//read the information upon start
		System.IO.StreamReader info = new System.IO.StreamReader ("printedInfo.txt");
		string line;
		string[] split = null;
		
		for(int i=0; i<10; i++){
			line = info.ReadLine ();
			line = line.Replace("$", "\n");	//splits the lines at the $ sign
			printedInfo[i] = line;			
		}

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}