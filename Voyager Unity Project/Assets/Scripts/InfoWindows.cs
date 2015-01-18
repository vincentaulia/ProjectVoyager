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
	List<string>data = new List<string> (); //holds the information on planets
	string focused = "";							//holds the focus of the main camera
	GameObject camera;
	string[] InfoButton = {"Show", "Hide"};	//Text on the button alternate
	string[] printedInfo = new string[10];	//holds the information of all planets
	int InfoPresent;						//boolean. whether a window is displayed for this object
	public jumpingCam target;				//to read a variable from another script
	public Rect windowRect;					//to hold the coordinates of the window

	void OnGUI() {

		windowRect = GUI.Window(6, windowRect, windowFunc, "Information");



		//display all the windows that have been poped up
		for (int i=0; i<myList.Count; i++) {
			myList [i] = GUI.Window (int.Parse(names[i]), myList [i], DoMyWindow, data[i]);
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
		//camera = GameObject.Find ("Main Camera");	//hold the camera object
		//focused = camera.GetComponent<jumpingCam> ().camTarget; //determine the object in view
		Debug.Log ("focused is: " + focused);

		//add the object if it is not in the list
		//otherwise, remove it
		if (!names.Contains (focused)) {
			//if the information exist in file
			string info = getInfo(focused);
			if(info != null){
			myList.Add (new Rect (100, 100, 200, 150));
				names.Add (focused);
				data.Add(info);
			}
		} else {
			int i;
			i = names.IndexOf(focused);
			names.RemoveAt(i);
			myList.RemoveAt(i);
			data.RemoveAt(i);
		}
	}

	//match the id of the planet to the information in file
	//return the infomation to be displayed
	string getInfo(string planet){
		System.IO.StreamReader info = new System.IO.StreamReader ("Assets\\Textfiles\\English\\printedInfo.txt");
		string line;
		line = info.ReadLine ();

		//read until the EOF
		while (line != null) {
			//check if information about the planet is present
			if (line.StartsWith(planet)){
				//break down the information to separate lines
				line = line.Replace("$", "\n");	//splits the lines at the $ sign
				break;
			}
			line = info.ReadLine();
				}
		info.Close();
		return line;
		/*
		int index;
		index = planet / 100;

		if (planet > 350) {
			index++;
		}
		return printedInfo [index];
		*/
	}

	void windowFunc(int windowID){

		//read the id of the object the camera is viewing
		//focused = GameObject.Find("Main Camera").GetComponent<jumpingCam> ().camTarget;
		GUI.SetNextControlName ("InfoBody");
		focused = GUI.TextField (new Rect(10, 22, 50, 20), focused);
		
		//If Enter key is pressed when textfield is focus
		//call the method to create a window
		if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "InfoBody") {
			if(GameObject.Find(focused)){
				CreateWindow();
			}
		}
		
		//check if the window is displayed (if the id is in 'names')
		if (names.Contains (focused)) {
			InfoPresent = 1;
		} else {
			InfoPresent = 0;
		}
		
		//Create the button to display/hide the window
		if (GUI.Button (new Rect (70, 20, 70, 25), InfoButton[InfoPresent])) {
			//check if this object is there
			if(GameObject.Find(focused)){
				CreateWindow();
			}
			
		}

		GUI.DragWindow ();
		}

	// Use this for initialization
	void Start () {

		windowRect = new Rect (10, 40, 150, 55);

		//read the information upon start
		/*System.IO.StreamReader info = new System.IO.StreamReader ("Assets\\Textfiles\\English\\printedInfo.txt");
		string line;
		string[] split = null;

		line = info.ReadLine ();
		for(int i=0; line != null; i++){
			line = line.Replace("$", "\n");	//splits the lines at the $ sign
			printedInfo[i] = line;
			line = info.ReadLine ();
		}*/

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}