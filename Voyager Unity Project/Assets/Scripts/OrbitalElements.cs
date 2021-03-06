﻿/* 
 * 
 * This file calculates the orbital elements for each planet and moon and stores them.
 * 
 * Attached to: 	Planet prefab
 * 					Moon prefab
 * 
 * Files needed:	orbit_info.txt
 * 
 * May 8 2015:
 * - Modified file to accomodate comets and asteroids.
 * - For now, use the older version of basic_info & orbit_info with this updated code,
 *   (as in the one with planets+moons only) because some comets are breaking Orbits.cs.
 */
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;

public class OrbitalElements : MonoBehaviour
{

		public Elements orb_elements;
		string line;

		public void getElements (string name, string radius, string parameters)
		{
				bool lineFound = false;

				string[] orbit;
				object orbitFile;
				orbitFile = Resources.Load ("orbit_info");

				//decide whether to read from file or from the inputted string
				if (parameters != null) {
						line = parameters;
						lineFound = true;
				} 
				else if (orbitFile != null) {
						orbit = orbitFile.ToString ().Split ('\n');
						for (int i = 0; i<orbit.Length; i++) {
								line = orbit [i];
								if (line.StartsWith (gameObject.transform.name)) {
										lineFound = true;
										break;
								}
						}
				}

				if (lineFound) {
						// Now split the line into tokens, and grab
						// relavant substrings
						// (i.e. mass of the planet and the thing it's orbiting and the orbital elements)
						string[] split = line.Split (new string[] {" "}, StringSplitOptions.None);

						//Name of the body
						orb_elements.name = name;
						//Mass of the planet.
						if (split[1].Contains("?")) {
							orb_elements.mass = (double)0;
						} else {
							orb_elements.mass = double.Parse (split [1], CultureInfo.InvariantCulture);
						}
						//Mass of the object the planet's orbiting.
						orb_elements.massFocus = double.Parse (split [3], CultureInfo.InvariantCulture);
						//The semi-major axis (in meters)
						orb_elements.axis = double.Parse (split [9], CultureInfo.InvariantCulture) * 1000;
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
						orb_elements.dir = int.Parse (split [11]);
						//the id of the body it is orbiting
						orb_elements.IDFocus = split [2];
						//sphere of influence
						orb_elements.P_Vector = new double[3];
						orb_elements.Q_Vector = new double[3];
						orb_elements.soi = orb_elements.axis * Math.Pow ((orb_elements.mass / orb_elements.massFocus), (2.0/5));

						//for the Ships
						if (split.Length > 12) {
							orb_elements.dryMass = double.Parse (split [13], CultureInfo.InvariantCulture);
							orb_elements.fuelMass = double.Parse (split [14], CultureInfo.InvariantCulture);
							orb_elements.Isp = double.Parse (split [12], CultureInfo.InvariantCulture);
							orb_elements.deltaVbudget = orb_elements.Isp*9.822*Math.Log((orb_elements.dryMass + orb_elements.fuelMass)/orb_elements.dryMass);
						} else {
							orb_elements.dryMass = 0;
							orb_elements.fuelMass = 0;
							orb_elements.Isp = 0;
							orb_elements.deltaVbudget = 0;
						}

						//the radius of the body
						if (radius.Contains ("x")) {
								int[] j = new int[3];
				
								//split them up
								j [0] = radius.IndexOf ('x');
								j [1] = radius.IndexOf ('x', j [0] + 1);
				
								//convert them to floats, and convert to m
								orb_elements.radiusx = float.Parse (radius.Substring (0, j [0])) * 1000;
								orb_elements.radiusy = float.Parse (radius.Substring (j [0] + 1, j [1] - j [0] - 1)) * 1000;
								orb_elements.radiusz = float.Parse (radius.Substring (j [1] + 1)) * 1000;
						} else if (radius.Contains("?")) {
								orb_elements.radiusx = orb_elements.radiusy = orb_elements.radiusz = (float)0;
						} else {
								//convert from km to m
								orb_elements.radiusx = orb_elements.radiusy = orb_elements.radiusz = float.Parse (radius) * 1000;
						}

						orb_elements.calcData ();
				} else {
						Debug.LogError ("ERROR [OrbitalElements]: Cannot assign orbital values");
				}
		}

		void Awake ()
		{

		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			
		}
}
