/* Created by Jihad El Sheikh
 * 
 * This file holds some global variables that are needed throughout the game
 * 
 * Attached to: 	Bary Center
 * 
 * Files needed:	None
 * 
 */

using UnityEngine;
using System.Collections;
using System;
//This is needed to use List
using System.Collections.Generic;

public class Global : MonoBehaviour {

	//Scales down the measurements by a factor of 1e5. Applies to measurements in km.
	public const float scale = 1e5F;

	//holds a list of the sun, planets and moons in the game
	public static List <GameObject> body = new List<GameObject>();

	//holds a list of the spaceships in the game
	public static List <GameObject> ship = new List<GameObject>();

	// m to AU - output.txt coordinates are in meters
	public const double CONVFACTOR = 149597870.700 * 1000;
	//this file holds the orbital elements of all the gameObject's orbits, which are used to make tracks.
	public const string ORBITAL_FILENAME = "orbit_info.txt";

	//controls the time step of the PCA
	public static int time = 0;

	// Used to scale fast-forwards & rewinds
	/*
	 * IMPORTANT:
	 * - Please make sure that this value greater than 0.
	 * - CSharp is a little silly with unsigned ints.
	 * - For now, if you want to move backwards in time, use the time jump feature
	 * - A value of 1 is normal speed, 2 is twice the normal speed, etc.
	 * - Allowable value for now: 1 to 5
	 */
	public static int time_multiplier = 1;
	
	// A boolean used my TimeJump.cs for pausing
	public static bool time_doPause = false;

	public GameObject planet_prefab;
	public GameObject moon_prefab;

}