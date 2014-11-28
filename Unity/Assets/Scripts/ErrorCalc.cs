using UnityEngine;
using System.Collections;

public class ErrorCalc : MonoBehaviour
{

		string body = "";
		string[] bugsBunny = new string[9];
	int localTime = 0;

		void OnGUI ()
		{

			
				
				int x = Screen.width - 300;

				GUI.Box (new Rect (x, 10, 290, 300), "Error Calculations");
				GUI.Label (new Rect (x + 30, 55, 260, 20), "Time: " + localTime);
				GUI.Label (new Rect (x + 30, 70, 260, 20), "Position");
				GUI.Label (new Rect (x + 110, 70, 260, 20), "Distance");
				GUI.Label (new Rect (x + 20, 90, 260, 20), "From sun:");
				GUI.Label (new Rect (x + 30, 110, 260, 20), bugsBunny [0]);
				GUI.Label (new Rect (x + 30, 130, 260, 20), bugsBunny [1] + "     " + bugsBunny [3] + " km");
				GUI.Label (new Rect (x + 30, 150, 260, 20), bugsBunny [2]);

				GUI.Label (new Rect (x + 20, 170, 260, 20), "From " + bugsBunny[4]+ ":");
				GUI.Label (new Rect (x + 30, 190, 260, 20), bugsBunny [5]);
				GUI.Label (new Rect (x + 30, 210, 260, 20), bugsBunny [6] + "     " + bugsBunny [8] + " km");
				GUI.Label (new Rect (x + 30, 230, 260, 20), bugsBunny [7]);


				GUI.SetNextControlName ("target");
				body = GUI.TextField (new Rect (x + 23, 30, 50, 20), body);
		
				//If Enter key is pressed when textfield is focus
				//call the method to change the focus
				if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "target") {
						bugsBunny = fetchData (body);
				}
		
				//if button is pressed
				//call the method to change the focus
				if (GUI.Button (new Rect (x + 80, 30, 50, 25), "Fetch")) {
						bugsBunny = fetchData (body);
				}

		}

		string[] fetchData (string s)
		{
				GameObject bodyObject, orbiting;
				bodyObject = GameObject.Find (s);
				
				string[] output = new string[9];
				
		Vector3 position, pos_orbit;

				if (bodyObject != null && s != "10") {

						localTime = Global.time;
						
						position = bodyObject.transform.position;
						
						output [0] = position [0].ToString ();
						output [1] = position [1].ToString ();
						output [2] = position [2].ToString ();

						output [3] = (position.magnitude * Global.scale).ToString ();
						int n;
						if (int.TryParse (s, out n)) {

								if (n % 100 == 99) {
				
										output [4] = output [5] = output [6] = output [7] = output [8] = "";

								} else {
										
										int orbiting_id;
					
										orbiting_id = (n / 100) * 100 + 99;
										orbiting = GameObject.Find (orbiting_id.ToString());
										pos_orbit = orbiting.transform.position;
					
										output [4] = orbiting.name;
					
										output [5] = (position [0] - pos_orbit[0]).ToString ();
										output [6] = (position [1] - pos_orbit[1]).ToString ();
										output [7] = (position [2] - pos_orbit[2]).ToString ();
					
										output [8] = (Vector3.Distance(pos_orbit, position) * Global.scale).ToString ();
								}
						} else {
								;		//ADD SOMETHING FOR THE SHIP LATER
						}

				}

				return output;
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
		Debug.Log ("ErrorCalc: " + Global.body[83].transform.position);
		}
}
