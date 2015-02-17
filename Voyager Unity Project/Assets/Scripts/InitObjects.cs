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

//needed to catch the exception
//using System.IO;
//using UnityEngine;
//using UnityEditor;

public class InitObjects : MonoBehaviour
{
		string[] basic;
		public GameObject orbitPreFab;
		int count;
		//Count the nmber of orbits for the moons
		int countMoon;
		Object basicFile;

		// Use this for initialization
		void Awake ()
		{

				try {
						basicFile = Resources.Load (Global.BASIC_FILENAME);
						//split the file into lines and store it in an array
						basic = basicFile.ToString ().Split ('\n');
				} catch (System.IO.FileNotFoundException e) {
						Debug.LogError ("Can't loacte the file 'basic_info.txt'.");
						return;
						//NEED TO EXIT HERE

				}


				//make sure the position and rotation of the bary center are set to zero
				this.transform.position = Vector3.zero;
				this.transform.eulerAngles = Vector3.zero;

				float diameter;
				string id;
				string[] line;
				string orbiting_id;

				//read the id and radii from the file
				//set the scale of the object
				for (int i = 0; i<basic.Length; i++) {
						line = basic [i].Split ();

						id = line [0];

						Global.body.Add (GameObject.Find (id));	//hold the pointer to the planet
						if (Global.body [i] == null) {
								//create a moon object
								Global.body [i] = (GameObject)Instantiate (GameObject.Find ("Bary Center").GetComponent<Global> ().moon_prefab);
								
								//name the object
								Global.body [i].name = id;
						}

						//if the radii of the moon vary dpeneding on the axis
						if (line [3].Contains ("x")) {
								int[] j = new int[3];
								float[] diameters = new float[3];
								
								//split them up
								j [0] = line [3].IndexOf ('x');
								j [1] = line [3].IndexOf ('x', j [0] + 1);
								
								//convert them to floats, scale them down and store them up
								diameters [0] = float.Parse (line [3].Substring (0, j [0])) * 2 / Global.scale;
								diameters [1] = float.Parse (line [3].Substring (j [0] + 1, j [1] - j [0] - 1)) * 2 / Global.scale;
								diameters [2] = float.Parse (line [3].Substring (j [1] + 1)) * 2 / Global.scale;
					
								//order of diameters is changed because the axis orientation in Unity is different
								Global.body [i].transform.localScale = new Vector3 (diameters [0], diameters [2], diameters [1]);
						} else {
								//scale down the radius
								diameter = float.Parse (line [3]) / Global.scale;
								//convert to diamter
								diameter *= 2;
								//set the dimentions of the moon
								Global.body [i].transform.localScale = new Vector3 (diameter, diameter, diameter);
						}
						//calculate the orbital elements for it
						if (Global.body [i].name == "10") {
								//make sure position and rotation of sun is set to zero relative to the Bary Center
								Global.body [i].transform.localPosition = Vector3.zero;
								Global.body [i].transform.localEulerAngles = Vector3.zero;
						} else {
								Global.body [i].GetComponent<OrbitalElements> ().getElements (line [1], line [3], null);
								orbiting_id = Global.body [i].GetComponent<OrbitalElements> ().orb_elements.IDFocus;
								Global.body [i].transform.parent = GameObject.Find (orbiting_id).transform;
						}

						//Show the orbits of the planets
						if (Global.body [i].name != "10" && (int.Parse (Global.body [i].name) % 100 == 99)) {
								count = Global.orbits.Count;
								//create a orbit object
								Global.orbits.Add ((GameObject)Instantiate (orbitPreFab));
								Global.orbits [count].name = "Orbit" + Global.body [i].name;
								Global.orbits [count].transform.parent = GameObject.Find ("OrbitsBody").transform;
								//calculate the points of the orbit
								Global.orbits [count].GetComponent<Orbits> ().makeOrbit (0, Global.body [i].name);
						}

						//Detect if an object is a moon
						if (Global.body [i].name != "10" && (int.Parse (Global.body [i].name) % 100 != 99)) {
								countMoon = Global.orbitsMoon.Count;
				
								Global.orbitsMoon.Add ((GameObject)Instantiate (orbitPreFab));
								Global.orbitsMoon [countMoon].name = "Orbit" + Global.body [i].name;
								Global.orbitsMoon [countMoon].transform.parent = GameObject.Find ("OrbitsBody").transform;
								//calculate the points of the orbit
								Global.orbitsMoon [countMoon].GetComponent<Orbits> ().makeMoonOrbit (0, Global.body [i].name, Global.body [i].GetComponent<OrbitalElements> ().orb_elements.IDFocus);
						}
				}
		}
}