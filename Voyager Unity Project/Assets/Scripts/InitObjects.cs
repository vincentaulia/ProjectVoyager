/* Created by Jihad El Sheikh
* 
* This script initializes the diameters of all the objects
* 
* Attached to: Bary Center
* 
* Files needed:	basic_info.txt
* 
* May 8 2015 (-Reuben):
* - Modified file to accomodate comets and asteroids.
* - For now, use the older version of basic_info & orbit_info with this updated code,
*   (as in the one with planets+moons only) because some comets are breaking Orbits.cs.
* 
* May 22 2015 (-Reuben):
* - Edited file to dynamically modify asteroid prefab meshes upon awakening.
* - Okay, the current problem is that asteroids in basic_info.txt without radius values will appear invisible.
*   (because NASA/JPL don't actually have values for a lot of them).
* - A possible solution - just assign a custom radius values to these missing radii so that they appear in the game
*   (bearing in mind that these are only placeholder values until NASA has official numbers).
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
	public GameObject moonOrbitPreFab;
	
	public GameObject cometOrbitPreFab;
	public GameObject asteroidOrbitPreFab;

	int count;
	//Count the nmber of orbits for the moons
	int countMoon;
	int countMeteor;
	int countAsteroid;
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

				if ((id.StartsWith("1")) && (id.Length == 7)) { // a comet
					Global.body [i] = (GameObject)Instantiate (GameObject.Find ("Bary Center").GetComponent<Global> ().comet_prefab);
				}
				else if ((id.StartsWith("2")) && (id.Length == 7)) { // an asteroid
					Global.body [i] = (GameObject)Instantiate (GameObject.Find ("Bary Center").GetComponent<Global> ().asteroid_prefab);

					// Ceres and Vesta both get their own spherical textures for now. They're the only ones I can find.
					if (id.EndsWith("1") || id.EndsWith("4")) {
						Renderer rnd = Global.body[i].GetComponent<Renderer>();
						rnd.material.mainTexture = (Texture)Resources.Load("Asteroids_textures/"+id);
					}
					// Ather asteroids use the default Unity asteroid models for now. Again, this is until someone finds/creates better models.
					// That, or either NASA releases better models/textures.
					else {
						MeshFilter mf = Global.body[i].GetComponent<MeshFilter>();
						mf.mesh = Resources.Load<Mesh>("Asteroids_models/Asteroid-001-HighPoly");	
					}

				}
				else {
					//create a moon object
					Global.body [i] = (GameObject)Instantiate (GameObject.Find ("Bary Center").GetComponent<Global> ().moon_prefab);

					//create the texture for the moon
					//access it from the Resources/Moons folder
					//the file should have the same name as the moon's id
					Renderer rend = Global.body [i].GetComponent<Renderer>();
					rend.material.mainTexture = (Texture) Resources.Load("Moons/" + id);
				}
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
			} else if (line[3].Contains("?")) { // some meteors and asteroids don't have radii
				Global.body[i].transform.localScale = new Vector3 ((float)2.0, (float)2.0, (float)2.0);
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
			//if (Global.body [i].name != "10" && (int.Parse (Global.body [i].name) % 100 == 99) && (Global.body [i].name.Length <= 3)) {
            if(Global.body[i].tag == "Planet"){
				count = Global.orbits.Count;
				//create a orbit object
				Global.orbits.Add ((GameObject)Instantiate (orbitPreFab));
				Global.orbits [count].name = "Orbit" + Global.body [i].name;
				Global.orbits [count].transform.parent = GameObject.Find ("OrbitsBody").transform;
                Global.orbits[count].tag = "OrbitPlanet";
				//calculate the points of the orbit
				Global.orbits [count].GetComponent<Orbits> ().makeOrbit (0, Global.body [i].name);
			}

			//Detect if an object is a moon
			//if (Global.body [i].name != "10" && (int.Parse (Global.body [i].name) % 100 != 99) && (Global.body [i].name.Length <= 3)) {
            else if(Global.body[i].tag == "Moon"){
				countMoon = Global.orbitsMoon.Count;

				Global.orbitsMoon.Add ((GameObject)Instantiate (moonOrbitPreFab));
				Global.orbitsMoon [countMoon].name = "Orbit" + Global.body [i].name;
				Global.orbitsMoon [countMoon].transform.parent = GameObject.Find ("OrbitsBody").transform;
                Global.orbitsMoon[countMoon].tag = "OrbitMoon";
				//calculate the points of the orbit
				Global.orbitsMoon [countMoon].GetComponent<MoonOrbit> ().makeMoonOrbit (0, Global.body [i].name, Global.body [i].GetComponent<OrbitalElements> ().orb_elements.IDFocus);
			}

			//if ((Global.body [i].name.StartsWith("1")) && (Global.body [i].name.Length > 3)) { // a comet
            else if (Global.body[i].tag == "Comet"){
				count = Global.orbits.Count;
				//create a orbit object
				Global.orbits.Add ((GameObject)Instantiate (cometOrbitPreFab));
				Global.orbits [count].name = "Orbit" + Global.body [i].name;
				Global.orbits [count].transform.parent = GameObject.Find ("OrbitsBody").transform;
                Global.orbits[count].tag = "OrbitComet";
				//calculate the points of the orbit
				Global.orbits [count].GetComponent<Orbits> ().makeOrbit (0, Global.body [i].name);
			}

			//if ((Global.body [i].name.StartsWith("2")) && (Global.body [i].name.Length > 3)) { // an asteroid
            else if(Global.body[i].tag == "Asteroid"){
				count = Global.orbits.Count;
				//create a orbit object
				Global.orbits.Add ((GameObject)Instantiate (asteroidOrbitPreFab));
				Global.orbits [count].name = "Orbit" + Global.body [i].name;
				Global.orbits [count].transform.parent = GameObject.Find ("OrbitsBody").transform;
                Global.orbits[count].tag = "OrbitAsteroid";
				//calculate the points of the orbit
				Global.orbits [count].GetComponent<Orbits> ().makeOrbit (0, Global.body [i].name);
			}
		}
	}
}