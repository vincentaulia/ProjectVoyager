/* 
 * 
 * This file calculates the orbital elements for each planet and moon and stores them.
 * 
 * Attached to: 	Planet prefab
 * 					Moon prefab
 * 
 * Files needed:	orbit_info.txt
 * 
 */

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;

public class OrbitalElements : MonoBehaviour {

	public Elements orb_elements;
	string line;

	public void getElements ()
	{
		
		if (File.Exists (Global.ORBITAL_FILENAME)) {
			StreamReader file = null;
			try {

				file = new StreamReader (Global.ORBITAL_FILENAME);
				while ((line = file.ReadLine()) != null) {
					if (line.StartsWith (gameObject.transform.name)) {
						Debug.Log ("I have started " + this.name);
						// Now split the line into tokens, and grab
						// relavant substrings
						// (i.e. mass of the planet and the thing it's orbiting and the orbital elements)
						string[] split = line.Split (new string[] {" "}, StringSplitOptions.None);
						
						//Mass of the planet.
						orb_elements.mass = double.Parse (split [1], CultureInfo.InvariantCulture);
						//Mass of the object the planet's orbiting.
						orb_elements.massFocus = double.Parse (split [3], CultureInfo.InvariantCulture);
						//The semi-major axis (in meters)
						orb_elements.axis = double.Parse (split [9], CultureInfo.InvariantCulture) * Global.CONVFACTOR;
						//eccentricity
						orb_elements.ecc = double.Parse (split [4], CultureInfo.InvariantCulture);
						//inclination (in radians)
						orb_elements.incl = double.Parse (split [5], CultureInfo.InvariantCulture) * Math.PI / 180;
						//longitude of ascending node (in radians)
						orb_elements.asc = double.Parse (split [6], CultureInfo.InvariantCulture) * Math.PI / 180;     
						//mean anomaly (in radians)
						orb_elements.anom = double.Parse (split [8], CultureInfo.InvariantCulture) * Math.PI / 180;
						//argument of periapsis (in radians)
						orb_elements.arg = double.Parse (split [7], CultureInfo.InvariantCulture) * Math.PI / 180;
						//direction (Prograde or Retrograde)
						orb_elements.dir = int.Parse (split[11]);
						
						orb_elements.calcData ();
					}
				}
			} finally {
				if (file != null)
					file.Close ();
			}
		}
	}

	void Awake(){
		getElements();
		Debug.Log ("this is orbital");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
