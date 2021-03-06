﻿/* Created by Jihad El Sheikh and Rashad Ajward
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
using System.Diagnostics; //For testing purposes only
using Debug = UnityEngine.Debug; //HUH


public class InsertShip : MonoBehaviour
{
		public GameObject shipVoyager1, shipISS, shipOrbiter, shipSkyLab, shipHST;
		public Rect popUp;
		//controls the appearance of the ship customizer
		bool showEdit = false;
		string name;
		//holds the data from the text fields
		string[] data = new string[16];
		public Rect windowRect;
		Rect warning;
		bool clickedAdd, clickedCancel;
		bool showWarning;
		string warningMsg;

        GUIContent[] comboBoxList;
        ComboBox comboBoxControl = new ComboBox();
        GUIStyle listStyle = new GUIStyle();

		public GameObject orbitPrefab;
		public Stopwatch stopwatch = new Stopwatch(); //for testing purposes only

        public GameObject vector;

		void OnGUI ()
		{

				if (!showEdit) {
						windowRect = GUI.Window (0, windowRect, DoMyWindow, "Ships");
				} else if (!showWarning) {
                      //  int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();
                      //  selectedItemIndex = comboBoxControl.List(new Rect(popUp.xMax, popUp.y + 21, 100, 20), comboBoxList[selectedItemIndex].text, comboBoxList, listStyle);
                       // GUI.Label(new Rect(popUp.xMax, popUp.y, 400, 21), "Ship Model Selection");   
						
                        popUp = GUI.Window (1, popUp, popUpFunc, "Ship Features");
					
				}
				if (showWarning) {
						warning = GUILayout.Window (2, warning, warningFunc, "Warning");
				}

		//this is to test the output after adding a NODE
		//SHOULD BE REMOVED LATER
				if(GUI.Button(new Rect(10,250,70,20), "Add node")){
					GameObject ship = GameObject.Find ("Ship1");
			int number = ship.GetComponent<shipOEHistory>().getNumberOfElements();
			if(number == 1){
			ship.GetComponent<shipOEHistory>().dummyNode(900000, 50000000);
			}else if (number == 2){
				ship.GetComponent<shipOEHistory>().dummyNode(2200000, 10000000);
			}else if (number == 3){
				ship.GetComponent<shipOEHistory>().dummyNode(3500000, 70000000);
			}
			//ship.GetComponent<shipOEHistory>().deltavChange(10, new Vector3(0,0,0), new Vector3(0,500,0), new Vector3(0,0,0));
			//Debug.Log ("number: " + ship.GetComponent<shipOEHistory>().getNumberOfElements());
			//Elements one = ship.GetComponent<shipOEHistory>().shipOE[0];
			//Elements two = ship.GetComponent<shipOEHistory>().shipOE[1];
			//Debug.Log ("shipOE count: " + ship.GetComponent<shipOEHistory>().shipOE.Count);
			//Debug.Log ("one axis: " + one.axis);
			//Debug.Log ("two axis: " + two.axis);

			//Debug.Log ("two ecc: " + two.ecc);
			//Debug.Log ("two incl: " + two.incl);
			//Debug.Log ("two asc: " + two.asc);
			//Debug.Log ("two anom: " + two.anom);
			//Debug.Log ("two arg: " + two.arg);


			//long oneTime = ship.GetComponent<shipOEHistory>().startTimeOE(one);
			//long twoTime = ship.GetComponent<shipOEHistory>().startTimeOE(two);
			//Debug.Log ("one time: " + oneTime);
			//Debug.Log ("two time: " + twoTime);
					//Debug.Log (ship.transform.position.magnitude * Global.scale);
				}
               
		}

		void createShip (string[] parameters)
		{
				Debug.Log ("Ship Created");
				//stopwatch.Start(); REMOVE LATER
				int i = Global.ship.Count;
				string mass, orbitingID;
				string[] line, inputParameter;
				//create a ship object. Choice of ship model based on user selectino from drop down list.
                float size = 0.0005f; //transform scaling factor. Needs to be adjusted for ISS, Orbiter, Skylab and HST

                if(parameters[15] == "0"){
                    Global.ship.Add((GameObject)Instantiate(shipVoyager1));
                    size = 0.0005f;
                }
                else if(parameters[15] == "1"){
                    Global.ship.Add((GameObject)Instantiate(shipISS));
                    size = 0.00007f;
                }     
                else if (parameters[15] == "2"){
                    Global.ship.Add((GameObject)Instantiate(shipOrbiter));
                    size = 0.00007f;
                }
                else if (parameters[15] == "3"){
                    Global.ship.Add((GameObject)Instantiate(shipSkyLab));
                    size = 0.00007f;
                }
                else if (parameters[15] == "4"){
                    Global.ship.Add((GameObject)Instantiate(shipHST));
                    size = 0.0001f;
                }
                    
            
				//name the object
				Global.ship [i].name = "Ship" + (i + 1).ToString ();

				//read the object's id to get its mass
				orbitingID = parameters [2];



				string[] basic;
				Object basicFile;
				basicFile = Resources.Load (Global.BASIC_FILENAME);
				basic = basicFile.ToString ().Split ('\n');
				bool found = false;

				//Add exception handler here
				//Searches the file to get the mass of the object it is orbiting
		
				for (int j = 0; j<basic.Length; j++) {
			
						if (basic [j].StartsWith (orbitingID)) {
								line = basic [j].Split ();
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
				Elements el = Global.ship [i].GetComponent<OrbitalElements> ().orb_elements;
				Global.ship [i].GetComponent<shipOEHistory> ().shipOEHistoryConstructor (el, Global.time, Global.ship [i]);  

			//	float size = 0.0005f; //added this for the Voyager 1 probe. Should actually modify this to take the size/scale given on the prefab object
				Global.ship [i].transform.localScale = new Vector3 (size, size, size);
				//place the ship in orbit around the planet
				Global.ship [i].transform.position = Global.ship [i].GetComponent<shipOEHistory> ().findShipPos (Global.time);
				Global.ship [i].transform.position += GameObject.Find (orbitingID).transform.position;
				Global.ship [i].transform.parent = GameObject.Find ("ForShip").transform;
				
				int count;
				count = Global.orbitsShip.Count;
				//create a orbit object
				Global.orbitsShip.Add ((GameObject)Instantiate (orbitPrefab));
				Global.orbitsShip [count].name = "Orbit" + Global.ship [i].name;
				Global.orbitsShip [count].transform.parent = GameObject.Find ("OrbitsShip").transform;
				//calculate the points of the orbit
				Global.orbitsShip [count].GetComponent<ShipOrbit> ().createOrbit (Global.time, Global.ship [i].name);
                //link the track to the ship
                Global.ship[i].GetComponent<shipOEHistory>().linkTrack(Global.ship[i].name);

				//stopwatch.Stop(); // REMOVE LATER
				//Debug.Log ("TIME ELAPSED");
				//Debug.Log(stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
		
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
						data [6] = "0";
						data [7] = "0";
						data [8] = "0";
						data [9] = "3E+4";
						data [10] = "0";
						data [11] = "1";
						data [12] = "1";
						data [13] = "1";
						data [14] = "0";
                        data[14] = "0";
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
				GUILayout.Label ("Dry Mass (kg)");
				data [13] = GUILayout.TextField (data [13], 20);

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
				GUILayout.Label ("Isp");
				data [12] = GUILayout.TextField (data [12], 20);
				GUILayout.Label ("Fuel Mass (kg)");
				data [14] = GUILayout.TextField (data [14], 20);

				GUILayout.EndVertical ();
                GUILayout.BeginVertical();
                GUILayout.Label("Ship Model Selection");
                int selectedItemIndex = comboBoxControl.GetSelectedItemIndex();
                selectedItemIndex = comboBoxControl.List(new Rect(popUp.xMax-120, popUp.yMin -90 , 100, 20), comboBoxList[selectedItemIndex].text, comboBoxList, listStyle);
                data[15] = selectedItemIndex.ToString();
               // Debug.Log("selected ship index:" + selectedItemIndex);

                GUILayout.EndVertical();
                
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
						if (!(float.TryParse (data [5], out f) && f >= 0 && f <= 180)) {
								establishWarning ("a number between 0 and 180 inclusive", "inclination");
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
				popUp = new Rect (10, 135, 420, 350);
				warning = new Rect (10, 135, 200, 80);


                comboBoxList = new GUIContent[5];
            	comboBoxList[0] = new GUIContent("Voyager 1");
            	comboBoxList[1] = new GUIContent("ISS");
            	comboBoxList[2] = new GUIContent("Orbiter");
           	    comboBoxList[3] = new GUIContent("Sky Lab");
            	comboBoxList[4] = new GUIContent("HST");
            
            	listStyle.normal.textColor = Color.white; 
            	listStyle.onHover.background =
            	listStyle.hover.background = new Texture2D(2, 2);
            	listStyle.padding.left =
            	listStyle.padding.right =
            	listStyle.padding.top =
            	listStyle.padding.bottom = 4;

             
        }

        
      
		// Update is called once per frame
		void Update ()
		{
	
		}
        public void drawVector(Vector3 start, Vector3 dir, float length, Color color, string name = "vector")
        {
            GameObject vect = (GameObject)Instantiate(vector);

            //this is the direction vector after being scaled
            Vector3 scaled;
            scaled = dir * length / dir.magnitude;

            vect.name = name;

            //place the vector halfway between the origin and the destinatino
            vect.transform.position = start + scaled / 2;

            //rotate it accordingly
            vect.transform.LookAt(start + scaled);

            //scale the vector along the z-axis only
            vect.transform.localScale = new Vector3(vect.transform.localScale.x, vect.transform.localScale.y, length);

            //colour the vector
            vect.GetComponent<MeshRenderer>().material.color = color;
        }
}
