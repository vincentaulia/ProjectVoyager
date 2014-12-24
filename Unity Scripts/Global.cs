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

	public static List <GameObject> body = new List<GameObject>();

	// m to AU - output.txt coordinates are in meters
	public const double CONVFACTOR = 149597870.700 * 1000;
	//this file holds the orbital elements of all the gameObject's orbits, which are used to make tracks.
	public const string ORBITAL_FILENAME = "orbit_info.txt";

	//controls the time step of the PCA
	public static int time = 0;

	public GameObject planet_prefab;
	public GameObject moon_prefab;

}