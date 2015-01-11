/*
 * Created by Jihad El Sheikh
 * 
 * This script displays a small window with a button. If the user clicks the button,
 * Another window pops up that shows coordinates of any requred body.It can be used to
 * assess the accuracy of the positions over time.
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	None
 * 
 */
using UnityEngine;
using System.Collections;

public class ErrorCalc : MonoBehaviour
{

		string body = "";
		//holds the data to be shown
		//0,1,2 - x,y and z coordinates of the body's position
		//3 - distance from the sun in km
		//4 - focus body (the body it is orbiting)
		//5,6,7 - x,y and z coordinates of the focus body
		//8 - distance from the focus body in km
		string[] bugsBunny = new string[9];
		string localTime = "0";
		int x = Screen.width - 300;
		public Rect errorRect;
		public Rect errorButton;
		bool showError = false;

		void OnGUI ()
		{
				//show the big window if the user chooses
				if (showError) {
						errorRect = GUI.Window (2, errorRect, errorFunc, "Error Calculations");
				} else {
						errorButton = GUI.Window (3, errorButton, errorButtonFunc, "Error Calculations");
				}
		}
		
		void errorFunc (int windowID)
		{

				//shows all the information for error calculation
				x = 0;				
				GUI.Label (new Rect (x + 30, 55, 260, 20), "Time: " + localTime);
				GUI.Label (new Rect (x + 30, 75, 260, 20), "Position");
				GUI.Label (new Rect (x + 160, 75, 260, 20), "Distance");
				GUI.Label (new Rect (x + 20, 90, 260, 20), "From sun:");
				GUI.Label (new Rect (x + 30, 110, 260, 20), "x: " + bugsBunny [0]);
				GUI.Label (new Rect (x + 30, 130, 260, 20), "y: " + bugsBunny [1]);
				GUI.Label (new Rect (x + 30, 150, 260, 20), "z: " + bugsBunny [2]);
				GUI.Label (new Rect (x + 160, 130, 260, 20), bugsBunny [3]);
		
				GUI.Label (new Rect (x + 20, 170, 260, 20), "From " + bugsBunny [4] + ":");
				GUI.Label (new Rect (x + 30, 190, 260, 20), "x: " + bugsBunny [5]);
				GUI.Label (new Rect (x + 30, 210, 260, 20), "y: " + bugsBunny [6]);
				GUI.Label (new Rect (x + 30, 230, 260, 20), "z: " + bugsBunny [7]);
				GUI.Label (new Rect (x + 160, 210, 260, 20), bugsBunny [8]);

				GUI.Label (new Rect (x + 20, 250, 260, 20), "All numbers are in km.");
				GUI.Label (new Rect (x + 20, 265, 260, 20), "The x-y plane is the horizontal plane.");
				GUI.Label (new Rect (x + 20, 280, 260, 25), "Hold down \"Enter\" to get real-time updates.");
		
		
				GUI.SetNextControlName ("target");
				body = GUI.TextField (new Rect (x + 100, 30, 50, 20), body);
		
				//If Enter key is pressed when textfield is in focus
				//call the method to update the data
				if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "target") {
						bugsBunny = fetchData (body);
				}
		
				//if button is pressed
				//call the method to update the data
				if (GUI.Button (new Rect (x + 155, 28, 50, 25), "Fetch")) {
						bugsBunny = fetchData (body);
				}

				//Hide the error calc window
				if (GUI.Button (new Rect (x + 120, 305, 80, 25), "Hide")) {
						showError = false;
				}
				GUI.DragWindow ();
		}

		void errorButtonFunc (int windowID)
		{
				//show the error calc window
				if (GUI.Button (new Rect (10, 20, 100, 25), "Show")) {
						showError = true;
				}
				GUI.DragWindow ();
		}
		//input is the target body
		//returns an array with the required information
		string[] fetchData (string s)
		{
				GameObject bodyObject, orbiting;
				//retrieve the specified object
				bodyObject = GameObject.Find (s);
				
				string[] output = new string[9];
				string[] units = {"s", "min", "hr", "d", "yr"};
				int[] divisor = {60, 60, 24, 365};
				
				Vector3 position, pos_orbit;
				int time;
				int remainder;
				string breakDown = "";
				
				//check if the object is there and that it is not the sun
				if (bodyObject != null && s != "10") {
						

						//get the current time
						time = Global.time;
						localTime = time.ToString () + "s: ";

						//divide it into seconds, minutes, hours, days and years
						for (int i = 0; i<4 && time != 0; i++) {
			
								remainder = time % divisor [i];
								breakDown += remainder.ToString () + units [i] + " ";
								time -= remainder;
								time /= divisor [i];
						}
						//if there's still time left, then put it in years
						if (time != 0) {
								breakDown += time.ToString () + units [4];
						}

						localTime += breakDown;
						
						//get the position of the target
						position = bodyObject.transform.position;

						//split the position to the three coordinates
						//switching back from Unity's coorodinates to standard ones (z-axis is vertical)
						output [0] = (position [0] * Global.scale).ToString ();
						output [1] = (position [2] * Global.scale).ToString ();
						output [2] = (position [1] * Global.scale).ToString ();
					
						//get the distance from the center (sun)
						output [3] = (position.magnitude * Global.scale).ToString ();
					
						int n;
						//if it's a planet, reset the second part of the screen
						//if the parse fails, then it's a ship, but it has the same procedure
						if (int.TryParse (s, out n) && n % 100 == 99) {
				
								output [4] = output [5] = output [6] = output [7] = output [8] = "";

						} else {

								//get the orbtial elements
								Elements el;
								el = bodyObject.GetComponent<OrbitalElements> ().orb_elements;

								string orbiting_id;
								
								//use the orbital elements to ge the id of the focus body
								orbiting_id = el.IDFocus;
								Debug.Log ("orbiting id: " + orbiting_id);
								orbiting = GameObject.Find (orbiting_id);
								pos_orbit = orbiting.transform.position;
								el = orbiting.GetComponent<OrbitalElements> ().orb_elements;

								//store name of the planet it's orbiting
								output [4] = el.name;

								//get the position of the target body with respect to the focux one
								//switching back from Unity's coorodinates to standard ones (z-axis is vertical)
								output [5] = ((position [0] - pos_orbit [0]) * Global.scale).ToString ();
								output [6] = ((position [2] - pos_orbit [2]) * Global.scale).ToString ();
								output [7] = ((position [1] - pos_orbit [1]) * Global.scale).ToString ();
								//get the distance from the target body
								output [8] = (Vector3.Distance (pos_orbit, position) * Global.scale).ToString ();
						}

				}

				return output;
		}

		// Use this for initialization
		void Start ()
		{
				errorRect = new Rect (Screen.width - 300, 100, 290, 340);
				errorButton = new Rect (Screen.width - 140, 100, 130, 50);
		}
	
		// Update is called once per frame
		void Update ()
		{
				//Debug.Log ("ErrorCalc: " + Global.body[83].transform.position);
		}
}
