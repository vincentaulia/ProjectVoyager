﻿/* Created by Jihad El Sheikh
 * 
 * This file holds some global variables that are needed throughout the game
 * 
 * Attached to: 	Bary Center
 * 
 * Files needed:	None
 * 
 * May 8 2015:
 * - Modified file to accomodate comets and asteroids.
 * - For now, use the older version of basic_info & orbit_info with this updated code,
 *   (as in the one with planets+moons only) because some comets are breaking Orbits.cs.
 * 
 */
using UnityEngine;
using System.Collections;
using System;

//This is needed to use List
using System.Collections.Generic;

public class Global : MonoBehaviour
{

		//Scales down the measurements by a factor of 1e5. Applies to measurements in km.
		public const float scale = 1e5F;

		//holds a list of the sun, planets and moons in the game
		public static List <GameObject> body = new List<GameObject> ();

		//holds a list of the spaceships in the game
		public static List <GameObject> ship = new List<GameObject> ();

		//holds a list of the orbits in the game
		public static List <GameObject> orbits = new List<GameObject> ();

		//holds a list of moon orbits in the game
		public static List <GameObject> orbitsMoon = new List<GameObject> ();

		//holds a list of ship orbits in the game
		public static List <GameObject> orbitsShip = new List<GameObject> ();

		// list of asteroid orbits
		public static List <GameObject> orbitsAsteroid = new List<GameObject> ();
	
		// list of meteor orbits
		public static List <GameObject> orbitsMeteor = new List<GameObject> ();

		// m to AU - output.txt coordinates are in meters
		public const double CONVFACTOR = 149597870.700 * 1000;
		//this file holds the names, masses and radii of planets and moons
		public const string BASIC_FILENAME = "basic_info";
		//this file holds the orbital elements of all the gameObject's orbits, which are used to make tracks.
		public const string ORBITAL_FILENAME = "orbit_info";

		//controls the time step of the PCA
		//good for up to 2.9 billion centuries
		public static long time = 0;

		// Used to scale fast-forwards & rewinds
		
		//IMPORTANT:
		//- Please make sure that this value greater than 0.
		//- CSharp is a little silly with unsigned ints.
		//- For now, if you want to move backwards in time, use the time jump feature
		//- A value of 1 is normal speed, 2 is twice the normal speed, etc.
		//- Allowable value for now: 1 to 5
	 
		public static int time_multiplier = 1; //1
	
		// Added on Jan 4 2015
		//it can go from -68 years to + 68 years
		public static int time_stepsize = 60;
	
		// A boolean used my TimeJump.cs for pausing
		public static bool time_doPause = true;
		public GameObject planet_prefab;
		public GameObject moon_prefab;
		public GameObject asteroid_prefab;
		public GameObject comet_prefab;

}