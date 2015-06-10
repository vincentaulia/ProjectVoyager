/*
 * Created by Jihad El Sheikh, Joohyun, Zachary Fejes
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
 * April 5, 2015
 * Updates:
 * - X button closes the info window for each planets
 * - Right click on a planet object triggers the info window
 * 
 */

using UnityEngine;
using System.Collections;

//This is needed to use List with <Rect>
using System.Collections.Generic;


public class InfoWindows : MonoBehaviour
{
		
		public List<Rect> myList = new List<Rect> ();	//holds the windows
		public List<string>names = new List<string> ();	//holds the ids of the planets
		public List<string>data = new List<string> (); //holds the information on planets
		public GameObject camera;
		//string[] InfoButton = {"Show", "Hide"};	//Text on the button alternate, no longer used after X button implementation
		public string[] printedInfo = new string[10];	//holds the information of all planets
		//int InfoPresent;						//boolean. whether a window is displayed for this object
		//public Rect windowRect;					//to hold the coordinates of the window

		// This boolean is toggled from CameraUserControl.cs
		// It brings up the camera options menu when right clicking a planet.
		public bool popUpMoreCamOptions = false; 

		//sets some properties for the pop up windows
		public void DoMyWindow (int windowID)
		{
				//The top bar's dimentsions are 200 wide and 20 tall. The borders are 2 wide all around.
				CameraUserControl cameraScript = Camera.main.GetComponent<CameraUserControl> ();	//cameraUserControl script attached to the main camera

				GUI.DragWindow(new Rect(0, 0, 178, 20)); //Only the top bar is dragable

				// Closes the window and removes the planet from myList upon clicking on X button
				if(GUI.Button (new Rect (178, 2, 20, 16), "X"))
				{	
					Debug.Log("Clicked on X, windowID is: " + windowID);
					int i;
					i = names.IndexOf (windowID.ToString());
					names.RemoveAt (i);
					myList.RemoveAt (i);
					data.RemoveAt (i);
				}
				// Creates a button to activate the "radial camera angle" script
				if (GUI.Button (new Rect (30, 110, 45, 16), "rad")) {	//the 
					cameraScript.cameraAngleSwitch(0);
				}
				// Creates a button to activate the "radial camera angle" script
				if (GUI.Button (new Rect (30, 130, 45, 16), "arad")) {	//the 
					cameraScript.cameraAngleSwitch(1);
				}
				// Creates a button to activate the "radial camera angle" script
				if (GUI.Button (new Rect (80, 110, 45, 16), "nor")) {	//the 
					cameraScript.cameraAngleSwitch(2);
				}
				// Creates a button to activate the "radial camera angle" script
				if (GUI.Button (new Rect (80, 130, 45, 16), "anor")) {	//the 
					cameraScript.cameraAngleSwitch(3);
				}
				// Creates a button to activate the "radial camera angle" script
				if (GUI.Button (new Rect (130, 110, 45, 16), "tan")) {	//the 
					cameraScript.cameraAngleSwitch(4);
				}
				// Creates a button to activate the "radial camera angle" script
				if (GUI.Button (new Rect (130, 130, 45, 16), "atan")) {	//the 
					cameraScript.cameraAngleSwitch(5);
				}

		}

		//adds a window to the list if it doesn't exist
		//or removes it if it exists
		public void CreateWindow (string focused)
		{
				//Debug.Log ("focused is: " + focused);
                int dummy;
            
				//add the object if it is not in the list
				//otherwise, do nothing
                //and if it's a planet
				if (int.TryParse(focused, out dummy) && !names.Contains (focused)) {
						//if the information exist in file
						string info = getInfo (focused);
						if (info != null) {
								myList.Add (new Rect (Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200, 150));
								names.Add (focused);
								data.Add (info);
						}
				}
				else
				{

				}

				// Removing window functionality is moved to the button implementation
				/* else {
						int i;
						i = names.IndexOf (focused);
						names.RemoveAt (i);
						myList.RemoveAt (i);
						data.RemoveAt (i);
				}*/
		}

		//match the id of the planet to the information in file
		//return the infomation to be displayed
		public string getInfo (string planet)
		{
				string[] info;
				object infoFile;
				infoFile = Resources.Load ("Textfiles/English/printedInfo");

				info = infoFile.ToString ().Split ('\n');
				string line = null;

				for (int i=0; i< info.Length; i++) {
						//Debug.Log (line);
						line = info [i];
						//check if information about the planet is present
						if (line.StartsWith (planet)) {
								//break down the information to separate lines
								line = line.Replace ("$", "\n");	//splits the lines at the $ sign
								return line;
						}
				}
				return line;
		}

		// No longer needed after the close button
		/*void windowFunc (int windowID)
		{

				//read the id of the object the camera is viewing
				GUI.SetNextControlName ("InfoBody");
				focused = GUI.TextField (new Rect (10, 22, 50, 20), focused);
		
				//If Enter key is pressed when textfield is focus
				//call the method to create a window
				if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "InfoBody") {
						if (GameObject.Find (focused)) {
								CreateWindow ();
						}
				}
		
				//check if the window is displayed (if the id is in 'names')
				if (names.Contains (focused)) {
						InfoPresent = 1;
				} else {
						InfoPresent = 0;
				}
		
				//Create the button to display/hide the window
				if (GUI.Button (new Rect (70, 20, 70, 25), InfoButton [InfoPresent])) {
						//check if this object is there
						if (GameObject.Find (focused)) {
								CreateWindow ();
						}
			
				}

				GUI.DragWindow ();
		}*/

		void OnGUI ()
		{
			//windowRect = GUI.Window (6, windowRect, windowFunc, "Information");

			// This boolean is toggled from CameraUserControl.cs
			// It brings up the camera options menu when right clicking a planet.
			if (popUpMoreCamOptions) {
				//display all the windows that have been poped up
				for (int i=0; i<myList.Count; i++) {
					myList [i] = GUI.Window (int.Parse (names [i]), myList [i], DoMyWindow, data [i]);
				}
			}
		
		}

		// Use this for initialization
		void Start ()
		{
				//windowRect = new Rect (10, 40, 150, 55);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}