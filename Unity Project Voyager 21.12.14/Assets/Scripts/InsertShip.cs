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

		void OnGUI ()
		{
				if (!showEdit) {
						windowRect = GUI.Window (0, windowRect, DoMyWindow, "Ships");
				} else {
						popUp = GUI.Window (1, popUp, popUpFunc, "Ship Features");
					
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

				//Add exception handler here
				//Searches the file to get the mass of the object it is orbiting
				System.IO.StreamReader basic_file = new System.IO.StreamReader ("basic_info.txt");
				while ((mass = basic_file.ReadLine ()) != null) {
						if (mass.StartsWith (orbitingID)) {
								line = mass.Split ();
								parameters [3] = line [2];
								break;
						}
						
				}
				//output an error message if the id was not found in the file
				if (mass == null) {
						Debug.LogError ("ERROR [InsertShip]: Cannot find object in textfile.");
				}

				//calculate the orbital elements for it
				Global.ship [i].GetComponent<OrbitalElements> ().getElements (name,string.Join (" ", parameters));
				float size = 0.005f;
				Global.ship [i].transform.localScale = new Vector3 (size, size, size);
				//place the ship in orbit around the planet
				Global.ship[i].transform.position = PcaPosition.findPos (Global.ship[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.ship[i]);

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
				GUILayout.Label ("Planet");
				data [2] = GUILayout.TextField (data [2], 20);
				GUILayout.Label ("Eccentricity");
				data [4] = GUILayout.TextField (data [4], 20);
				GUILayout.Label ("Inclination");
				data [5] = GUILayout.TextField (data [5], 20);
				GUILayout.Label ("Anomaly");
				data [8] = GUILayout.TextField (data [8], 20);

				GUILayout.EndVertical ();

				GUILayout.BeginVertical ();

				GUILayout.Label ("Mass");
				data [1] = GUILayout.TextField (data [1], 20);
				GUILayout.Label ("Semi-major axis");
				data [9] = GUILayout.TextField (data [9], 20);
				GUILayout.Label ("Ascending Node");
				data [6] = GUILayout.TextField (data [6], 20);
				GUILayout.Label ("Perifocus");
				data [7] = GUILayout.TextField (data [7], 20);
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();


				if (GUILayout.Button ("Add")) {
						createShip (data);
						showEdit = false;
				} else if (GUILayout.Button ("Cancel")) {
						showEdit = false;
				}

				GUILayout.EndHorizontal ();

				GUI.DragWindow ();
		}

		// Use this for initialization
		void Start ()
		{
				//initialize both windows here
				windowRect = new Rect (10, 70, 120, 50);
				popUp = new Rect (10, 70, 280, 300);
		}

		// Update is called once per frame
		void Update ()
		{
	
		}




}
