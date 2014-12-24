/* Created by Jihad El Sheikh
 * 
 * This script initializes the diameters of all the objects
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	basic_info.txt
 * 
 */

using UnityEngine;
using System.Collections;

public class InitObjects : MonoBehaviour
{
	
		System.IO.StreamReader basic = new System.IO.StreamReader ("basic_info.txt");

		// Use this for initialization
		void Awake ()
		{
				Debug.Log ("initialize");		

				//make sure the position and rotation of the bary center are set to zero
				this.transform.position = Vector3.zero;
				this.transform.eulerAngles = Vector3.zero;


				float diameter;
				string id;
				string readFile;
				string[] line;
				int i = 0;

				//read the id and radii from the file
				//set the scale of the object
				while ((readFile = basic.ReadLine ()) != null) {
						line = readFile.Split ();

						id = line [0];

						Global.body.Add (GameObject.Find (id));	//hold the pointer to the planet
						if (Global.body [i] == null) {

						
								//create a moon object
								Global.body [i] = (GameObject)Instantiate (GameObject.Find ("Bary Center").GetComponent<Global> ().moon_prefab);
								//name the object
								Global.body [i].name = id;
								//calculate the orbital elements for it
								Global.body [i].GetComponent<OrbitalElements> ().getElements ();
						}

						//if the radii of the moon vary dpeneding on the axis
						if (line [3].Contains ("x")) {
								int[] j = new int[3];
								float[] diameters = new float[3];
								
								//split them up
								j [0] = line [3].IndexOf ('x');
								j [1] = line [3].IndexOf ('x', j [0] + 1);
								
								//convert them to floats and store them up
								diameters [0] = float.Parse (line [3].Substring (0, j [0])) * 2 / Global.scale;
								diameters [1] = float.Parse (line [3].Substring (j [0] + 1, j [1] - j [0] - 1)) * 2 / Global.scale;
								diameters [2] = float.Parse (line [3].Substring (j [1] + 1)) * 2 / Global.scale;
					
								//order of diameters is changed because the axis orientation in Unity is different
								Global.body [i++].transform.localScale = new Vector3 (diameters [0], diameters [2], diameters [1]);
						} else {
								//scale down the radius
								diameter = float.Parse (line [3]) / Global.scale;
								//convert to diamter
								diameter *= 2;
								//set the dimentions of the moon
								Global.body [i++].transform.localScale = new Vector3 (diameter, diameter, diameter);
						}

				}

				basic.Close ();
		}
}