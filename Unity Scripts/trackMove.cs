using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;

//This .cs file is meant to go on every planet object to calculate a track for the planet to move along.


struct Elements {
	//This class holds all the orbital elements of a planet. 
	
	//The following variables must all be read in from somewhere.
	//Mass of the object
	public double mass;
	//Mass of the object it's orbiting
	public double massFocus;
	//The semi-major axis (in meters)
	public double axis;
	//eccentricity
	public double ecc;
	//inclination (in radians)
	public double incl;
	//longitude of ascending node (in radians)
	public double asc;     
	//mean anomaly (in radians)
	public double anom;
	//argument of periapsis (in radians)
	public double arg;       
	
	//The variables P, Q, and n must be calculated using the other orbital elements.
	//If any of the other orbital elements change, P, Q, and n must be recalculated. 
	//(the other orbital elements won't be changing for planets, but they will be for ships).
	public Vector3 P;
	public Vector3 Q;
	public double n;
	
	
	public void calcData() {
		//This function uses the other orbital elements to calculate P, Q, and n, which are al based on other orbital elements.
		//It MUST be called every time any of the orbital elements change.
		
		//Calculating P
		double Px = Math.Cos (arg) * Math.Cos (asc) - Math.Sin (arg) * Math.Cos (incl) * Math.Sin (asc);
		double Py = Math.Cos (arg) * Math.Sin (asc) + Math.Sin (arg) * Math.Cos (incl) * Math.Cos (asc);
		double Pz = Math.Sin (arg) * Math.Sin (incl);
		
		//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
		P = new Vector3 ((float)Px, (float)Py, (float)Pz);
		
		
		//Calculating Q
		double Qx = -Math.Sin (arg) * Math.Cos (asc) - Math.Cos (arg) * Math.Cos (incl) * Math.Sin (asc);
		double Qy = -Math.Sin (arg) * Math.Sin (asc) + Math.Cos (arg) * Math.Cos (incl) * Math.Cos (asc);
		double Qz = Math.Sin (incl) * Math.Cos (arg);
		//*********LOSS OF PRECISION HER	E BY CONVERTING TO FLOATS.
		Q = new Vector3 ((float)Qx, (float)Qy, (float)Qz);
		
		// Calculating n
		n = Math.Sqrt ((6.67384e-11) * (mass + massFocus) / (axis * axis * axis));
	}
	
}

public class trackMove : MonoBehaviour {
	
	
	// m to AU - output.txt coordinates are in meters
	private const double CONVFACTOR = 149597870.700 * 1000;
	//this file holds the orbital elements of all the gameObject's orbits, which are used to make tracks.
	private const string FILENAME = "orbit_info.txt";
	
	//This struct will be used to hold all the orbital elements of the planet
	private Elements orb_elements;
	
	private string line;
	
	private int time = 0;
	
	
	Vector3 findPos(Elements el, int time)
	{
		//This function finds the position of a planet given a bunch of orbital parameters and some other stuff.
		
		double anom = el.anom + el.n * time;
		double E = anom;
		double Enext = E;
		//Normally epsilon should be much smaller than this, but for now the program takes too long with small epsilons. 
		double epsilon = Math.Pow(10, -10);
		
		do {
			E = Enext;
			Enext = E - ( (E - el.ecc*Math.Sin(E) - anom) / (1 - el.ecc*Math.Cos(E)) );
			
		} while (Math.Abs(Enext - E) > epsilon);
		
		Vector3 R;
		//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
		R = (float)(el.axis*(Math.Cos(E) - el.ecc))*el.P + (float)(el.axis*Math.Sqrt(1- el.ecc*el.ecc)*Math.Sin(E))*el.Q;
		
		return R;
	}
	
	private void getElements() {
		
		if (File.Exists (FILENAME)) {
			StreamReader file = null;
			try {
				file = new StreamReader (FILENAME);
				while ((line = file.ReadLine()) != null) {
					if (line.StartsWith (gameObject.transform.name)) {
						// Now split the line into tokens, and grab
						// relavant substrings
						// (i.e. mass of the planet and the thing it's orbiting and the orbital elements)
						string[] split = line.Split (new string[] { " " }, StringSplitOptions.None);
						
						//Mass of the planet.
						orb_elements.mass = double.Parse (split [1], CultureInfo.InvariantCulture);
						//Mass of the object the planet's orbiting.
						orb_elements.massFocus = double.Parse (split [3], CultureInfo.InvariantCulture);
						//The semi-major axis (in meters)
						orb_elements.axis = double.Parse (split [9], CultureInfo.InvariantCulture) * CONVFACTOR;
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
						
						orb_elements.calcData();
					}
				}
			} finally {
				if (file != null)
					file.Close ();
			}
		}
	}
	
	// Read file contents into an array and initialize
	// this planetary body's coordinates
	void Awake() {
		Debug.Log(gameObject.name + ": Awake");
		Debug.Log(CONVFACTOR.ToString("F10"));
		
		getElements();
		Vector3 R;
		//Uncomment the section bellow to write data to a file. 
		/*
		using (System.IO.StreamWriter dataFile = new System.IO.StreamWriter(@"/Users/zeev/Desktop/EMBTrack.txt")) {
						while (time < 60*60*24*365*10) {

								R = findPos (orb_elements, time);
								time += 60 * 60 * 24;


								Debug.Log (time);
														
								string text = R.x + " " + R.y + " " + R.z;

								dataFile.WriteLine (text);

						}
		}*/
	}
	
	// Use this for initialization
	void Start () {
		//Debug.Log(gameObject.name + ": Start");
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//NOTE: this needs to be changed so that it give position relative to the solar barycenter
		//instead of position relative to the object it's orbiting. 
		//We should also change the functionality so that the variable time can be changed.
		gameObject.transform.position = findPos (orb_elements, time);
		
	}
	
	
}
