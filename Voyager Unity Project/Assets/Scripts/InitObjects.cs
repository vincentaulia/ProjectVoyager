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
						basicFile = Resources.Load ("basic_info");
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
				//bool isMoon = false;

				//read the id and radii from the file
				//set the scale of the object
				for (int i = 0; i<basic.Length; i++) {
						line = basic [i].Split ();

						id = line [0];

						Global.body.Add (GameObject.Find (id));	//hold the pointer to the planet
						if (Global.body [i] == null) {
								//create a moon object
								Global.body [i] = (GameObject)Instantiate (GameObject.Find ("Bary Center").GetComponent<Global> ().moon_prefab);
								
								//Make the moon a child of the Bary Center
								//Global.body [i].transform.parent = GameObject.Find ("Bary Center").transform;
								
								//name the object
								Global.body [i].name = id;
				
								//set this to true, in order to make it a child later
								//isMoon = true;
						}
						//if it's the earth's moon, set flag to true as well
						/*if (Global.body [i].name == "301") {
								isMoon = true;
						}*/

						//this is commented out because it is already executed below
						//calculate the orbital elements for it
						/*if (Global.body [i].name != "10") {
				Global.body [i].GetComponent<OrbitalElements> ().getElements (line [1], line[3], null);
			}*/

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

						/*
						//calculate the orbital elements for it
						if (Global.body [i].name == "10") {
								//make sure position and rotation of sun is set to zero relative to the Bary Center
								Global.body [i].transform.localPosition = Vector3.zero;
								Global.body [i].transform.localEulerAngles = Vector3.zero;
						} else if(int.Parse(Global.body[i].name) % 100 != 99) {
								//this needs to be done after setting the scales of the bodies
								//otherwise they would be affected by the parent's scale
								Global.body [i].GetComponent<OrbitalElements> ().getElements (line [1], line[3], null);
								orbiting_id = Global.body [i].GetComponent<OrbitalElements> ().orb_elements.IDFocus;
								Global.body [i].transform.parent = GameObject.Find (orbiting_id).transform;
						}
*/

						//calculate the orbital elements for it
						if (Global.body [i].name == "10") {
								//make sure position and rotation of sun is set to zero relative to the Bary Center
								Global.body [i].transform.localPosition = Vector3.zero;
								Global.body [i].transform.localEulerAngles = Vector3.zero;
						} else {
								Global.body [i].GetComponent<OrbitalElements> ().getElements (line [1], line [3], null);
								orbiting_id = Global.body [i].GetComponent<OrbitalElements> ().orb_elements.IDFocus;
								Global.body [i].transform.parent = GameObject.Find (orbiting_id).transform;
								//Debug.Log (Global.body[i].name + ":" + orbiting_id);
						}
						/*
						//This needs to be done after setting the scales of everything, so it doesn't alter it
						//This makes moons the children of the planets they are orbiting
						if (isMoon) {
								orbiting_id = int.Parse (id);
								//get the id of the planet it is orbiting
								orbiting_id = orbiting_id / 100 * 100 + 99;
								//make the moon a child of the planet
								Global.body [i].transform.parent = GameObject.Find (orbiting_id.ToString ()).transform;
								//reset flag
								isMoon = false;
						} else {
								//can be used in the block for orbits
								orbiting_id = 10;
								Global.body [i].transform.parent = GameObject.Find (orbiting_id.ToString ()).transform;
						}
						
*/
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

						//Detect is an object is a moon
						if (Global.body [i].name != "10" && (int.Parse (Global.body [i].name) % 100 != 99)) {
								countMoon = Global.orbitsMoon.Count;
				
								Global.orbitsMoon.Add ((GameObject)Instantiate (orbitPreFab));
								Global.orbitsMoon [countMoon].name = "Orbit" + Global.body [i].name;
								Global.orbitsMoon [countMoon].transform.parent = GameObject.Find ("OrbitsBody").transform;
								//calculate the points of the orbit
								Global.orbitsMoon [countMoon].GetComponent<Orbits> ().makeMoonOrbit (0, Global.body [i].name, Global.body [i].GetComponent<OrbitalElements> ().orb_elements.IDFocus);
						}


						//this is to create orbits for planets and moons
						//if you are using this block, comment the block before
						/*
			if (Global.body [i].name != "10") {
				count = Global.orbits.Count;
				//create a ship object
				Global.orbits.Add ((GameObject)Instantiate (orbitPreFab));
				Global.orbits [count].name = "Orbit" + Global.body [i].name;
				//making the orbit a child of the focus needs to be done before
				//calculating the points for the orbit
				Global.orbits [count].transform.parent = GameObject.Find (orbiting_id.ToString()).transform;
				//calculate the points of the orbit
				Global.orbits [count].GetComponent<Orbits> ().makeOrbit (0, Global.body [i].name);
				
			}*/
				}
				
		}
}