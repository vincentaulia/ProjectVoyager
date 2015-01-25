/* Created by Jihad El Sheikh and Rashad Ajward
 * 
 * This file asks the user to input features of the ship,
 * then creates a ship from a prefab.
 * 
 * 
 * Attached to: ForShip
 * 
 * Files needed:	basic_info.txt
 * 
 */
using UnityEngine;
using System.Collections;

public class InsertShip : MonoBehaviour
{
		public GameObject spaceShip;
		public Rect popUp;
		//controls the appearance of the ship customizer
		bool showEdit = false;
		string name;
		//holds the data from the text fields
		string[] data = new string[12];
		public Rect windowRect;
		Rect warning;
		bool clickedAdd, clickedCancel;
		bool showWarning;
		string warningMsg;

		void OnGUI ()
		{

				if (!showEdit) {
						windowRect = GUI.Window (0, windowRect, DoMyWindow, "Ships");
				} else if (!showWarning) {
						popUp = GUI.Window (1, popUp, popUpFunc, "Ship Features");
					
				}
				if (showWarning) {
						warning = GUILayout.Window (2, warning, warningFunc, "Warning");
				}


				//These two blocks just to test the functionality of changing the width of the orbits
				//TO BE REMOVED LATER
				if (GUI.Button (new Rect (10, 250, 70, 20), "/\\ orbit")) {
						int c = Global.orbits.Count;
						for (int i=0; i<c; i++) {
								float w = Global.orbits [i].GetComponent<Orbits> ().getWidth ();
								Global.orbits [i].GetComponent<Orbits> ().setWidth (w + 5);

						}
			c = Global.orbitsMoon.Count;
			for (int i=0; i<c; i++) {
				float w = Global.orbitsMoon [i].GetComponent<Orbits> ().getWidth ();
				Global.orbitsMoon [i].GetComponent<Orbits> ().setWidth (w + 5);
				
			}
				}
				if (GUI.Button (new Rect (10, 280, 70, 20), "\\/ orbit")) {
						int c = Global.orbits.Count;
						for (int i=0; i<c; i++) {
								float w = Global.orbits [i].GetComponent<Orbits> ().getWidth ();
								//if width is smaller than 6, then decrease the width by a fraction instead
								//of decreasing it by a fixed number
								if (w < 6) {
										Global.orbits [i].GetComponent<Orbits> ().setWidth (w * 0.6f);
								} else {
										Global.orbits [i].GetComponent<Orbits> ().setWidth (w - 5);
								}
				
						}
			c = Global.orbitsMoon.Count;
			for (int i=0; i<c; i++) {
				float w = Global.orbitsMoon [i].GetComponent<Orbits> ().getWidth ();
				//if width is smaller than 6, then decrease the width by a fraction instead
				//of decreasing it by a fixed number
				if (w < 6) {
					Global.orbitsMoon [i].GetComponent<Orbits> ().setWidth (w * 0.6f);
				} else {
					Global.orbitsMoon [i].GetComponent<Orbits> ().setWidth (w - 5);
				}
				
			}
				}

		}

		void createShip (string[] parameters)
		{

				int i = Global.ship.Count;
				string mass, orbitingID;
				string[] line, inputParameter;
				//create a ship object
				Global.ship.Add ((GameObject)Instantiate (spaceShip));
				//name the object
				Global.ship [i].name = "Ship" + (i + 1).ToString ();

				//read the object's id to get its mass
				orbitingID = parameters [2];

		string[] basic;
		Object basicFile;
		basicFile = Resources.Load ("basic_info");
		basic = basicFile.ToString().Split('\n');
		bool found = false;

		//Add exception handler here
		//Searches the file to get the mass of the object it is orbiting
		
		for(int j = 0; j<basic.Length; j++){
			//while ((mass = basic_file.ReadLine ()) != null) {
			
			if (basic[j].StartsWith (orbitingID)) {
				line = basic[j].Split ();
				//get the mass
				parameters [3] = line [2];
				found = true;
				break;
			}
			
		}
				//output an error message if the id was not found in the file
		if (!found) {
						Debug.LogError ("ERROR [InsertShip]: Cannot find object in textfile.");
				}

				//calculate the orbital elements for it
				Global.ship [i].GetComponent<OrbitalElements> ().getElements (name, "0.1", string.Join (" ", parameters));
				//float size = 0.005f;
				float size = 0.0005f; //added this for the Voyager 1 probe. Should actually modify this to take the size/scale given on the prefab object
				Global.ship [i].transform.localScale = new Vector3 (size, size, size);
				//place the ship in orbit around the planet
				Global.ship [i].transform.position = PcaPosition.findPos (Global.ship [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.ship [i]);
				Global.ship [i].transform.parent = GameObject.Find ("ForShip").transform;

		}

		void DoMyWindow (int windowID)
		{
				if (GUILayout.Button ("Add Ships")) {
						showEdit = true;

						//Default values
						data [0] = "Ship" + (Global.ship.Count + 1).ToString ();
						data [1] = "1.00E+5";
						data [2] = "399";
						data [3] = "5.97219E+24";
						data [4] = "0";
						data [5] = "0";
						data [6] = "20";
						data [7] = "0";
						data [8] = "0";
						data [9] = "3E+4";
						data [10] = "0";
						data [11] = "1";
						name = "Space Ship";
				}
				GUI.DragWindow ();
		
		}
	
		void popUpFunc (int windowID)
		{
				//This makes only the top bar draggable
				//GUI.DragWindow (new Rect(0, 0, 10000, 20));
				GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ();

				GUILayout.Label ("Name of the ship");
				name = GUILayout.TextField (name, 20);	
				GUILayout.Label ("Orbiting Focus");
				data [2] = GUILayout.TextField (data [2], 20);
				GUILayout.Label ("Eccentricity");
				data [4] = GUILayout.TextField (data [4], 20);
				GUILayout.Label ("Inclination (deg)");
				data [5] = GUILayout.TextField (data [5], 20);
				GUILayout.Label ("Mean Anomaly (deg)");
				data [8] = GUILayout.TextField (data [8], 20);

				GUILayout.EndVertical ();

				GUILayout.BeginVertical ();

				GUILayout.Label ("Mass (kg)");
				data [1] = GUILayout.TextField (data [1], 20);
				GUILayout.Label ("Semi-major axis (km)");
				data [9] = GUILayout.TextField (data [9], 20);
				GUILayout.Label ("Ascending Node (deg)");
				data [6] = GUILayout.TextField (data [6], 20);
				GUILayout.Label ("Perifocus (deg)");
				data [7] = GUILayout.TextField (data [7], 20);
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();

				clickedAdd = GUILayout.Button ("Add");
				clickedCancel = GUILayout.Button ("Cancel");

				GUILayout.EndHorizontal ();

				GUI.DragWindow ();

				if (clickedAdd) {
						//set the warning message parameters in case it was called
						warning.position = new Vector2 (popUp.x + 40, popUp.y + 80);
						float f;

						//remove white spaces before and after all arguments
						name = name.Trim ();
						for (int i= 0; i<data.Length; i++) {
								data [i] = data [i].Trim ();
						}

						//handles all the warnings that can be violated

						if (name.Length == 0) {
								establishWarning ("a name", "ship");
								return;
						}
						if (!float.TryParse (data [1], out f) || f <= 0) {
								establishWarning ("positive number", "mass");
								return;
						}

						//need the object it is orbiting around for other warnings
						GameObject focus;
						focus = GameObject.Find (data [2]);
						
						if (focus == null) {
								establishWarning ("a valid ID", "orbiting focus");
								return;
						}
						if (float.TryParse (data [9], out f) && f > 0) {								
								float radius;
								radius = focus.transform.lossyScale.x * Global.scale / 2;
								if (f <= radius) {
										establishWarning ("a number greater than the orbiting focus's radius", "semi-major axis");
										return;
								}
						} else {
								establishWarning ("positive number", "semi-major axis");
								return;
						}
						if (!(float.TryParse (data [4], out f) && f >= 0 && f <= 1)) {
								establishWarning ("a number between 0 and 1 inclusive", "eccentricity");
								return;
						}
						if (!(float.TryParse (data [6], out f) && f >= 0 && f <= 360)) {
								establishWarning ("a number between 0 and 360 inclusive", "ascending Node");
								return;
						}
						if (!(float.TryParse (data [5], out f) && f >= 0 && f <= 360)) {
								establishWarning ("a number between 0 and 360 inclusive", "inclination");
								return;
						}
						if (!(float.TryParse (data [7], out f) && f >= 0 && f <= 360)) {
								establishWarning ("a number between 0 and 360 inclusive", "perifocus");
								return;
						}
						if (!(float.TryParse (data [8], out f) && f >= 0 && f <= 360)) {
								establishWarning ("a number between 0 and 360 inclusive", "mean anomaly");
								return;
						}

						createShip (data);
						showEdit = false;
				} else if (clickedCancel) {
						showEdit = false;
				}
		}

		//sets the message for the warning
		void establishWarning (string msg, string field)
		{
				warningMsg = "Please enter " + msg + " for the " + field;
				showWarning = true;
		}

		//includes the button to close the warning message
		void warningFunc (int windowID)
		{
				GUILayout.Label (warningMsg);
				if (GUILayout.Button ("OK")) {
						showWarning = false;
				}
		}

		// Use this for initialization
		void Start ()
		{
				//initialize both windows here
				windowRect = new Rect (10, 105, 120, 50);
				popUp = new Rect (10, 135, 280, 300);
				warning = new Rect (10, 135, 200, 80);
		}

		// Update is called once per frame
		void Update ()
		{
	
		}




}
