using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;

//This .cs file is meant to go on every planet object to calculate a track for the planet to move along.
public class trackMove : MonoBehaviour {
	

	// m to AU - output.txt coordinates are in meters
	private const double CONVFACTOR = 149597870.700 * 1000;
	//this file holds the orbital elements of all the gameObject's orbits, which are used to make tracks.
	private const string FILENAME = "orbit_info.txt";

	//This variable exists because the Update() function needs to know the array size.
	//A value is assigned to this variable in the makeTrack() function.
	private int ARRAY_SIZE;
	//This array of position vectors will hold the track.
	private Vector3[] track;
	// Timestamp, for logging purposes
	private double[] timestamp;
	
	private string line;
	
	private int step = 0;
	
	
	Vector3 findPos(double anom, double axis, double ecc, Vector3 Q, Vector3 P)
	{
		//This function finds the position of a planet given a bunch of orbital parameters and some other stuff. 
		double E = anom;
		double Enext = E;
		//Normally epsilon should be much smaller than this, but for now the program takes too long with small epsilons. 
		double epsilon = Math.Pow(10, -5);
		
		do {
			E = Enext;
			Enext = E - ( (E - ecc*Math.Sin(E) - anom) / (1 - ecc*Math.Cos(E)) );
			
		} while (Math.Abs(Enext - E) > epsilon);
		
		Vector3 R;
		//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
		R = (float)(axis*(Math.Cos(E) - ecc))*P + (float)(axis*Math.Sqrt(1- ecc*ecc)*Math.Sin(E))*Q;
		
		return R;
	}
	
	
	private void makeTrack() {

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
												double mass = double.Parse (split [1], CultureInfo.InvariantCulture);
												//Mass of the object the planet's orbiting.
												double massFocus = double.Parse (split [3], CultureInfo.InvariantCulture);
												//The semi-major axis (in meters)
												double axis = double.Parse (split [9], CultureInfo.InvariantCulture) * CONVFACTOR;
												//eccentricity
												double ecc = double.Parse (split [4], CultureInfo.InvariantCulture);
												//inclination (in radians)
												double incl = double.Parse (split [5], CultureInfo.InvariantCulture) * Math.PI / 180;
												//longitude of ascending node (in radians)
												double asc = double.Parse (split [6], CultureInfo.InvariantCulture) * Math.PI / 180;     
												//mean anomaly (in radians)
												double anom = double.Parse (split [8], CultureInfo.InvariantCulture) * Math.PI / 180;
												//argument of periapsis (in radians)
												double arg = double.Parse (split [7], CultureInfo.InvariantCulture) * Math.PI / 180;       

												//The period of the rotation, in seconds.
												double t_final = double.Parse (split [10], CultureInfo.InvariantCulture) * 60 * 60 * 24;
												double stepSize = 24 * 60 * 60;
												
												//Determine the array size based on the final time and the stepsize. 
												ARRAY_SIZE = (int)Mathf.Ceil ((float)(t_final / stepSize));
												//assign memory to the track and the timestamp array.
												track = new Vector3[ARRAY_SIZE];
												timestamp = new double[ARRAY_SIZE];
												Debug.Log (ARRAY_SIZE);

												//Complicated math calculations here
												double Px = Math.Cos (arg) * Math.Cos (asc) - Math.Sin (arg) * Math.Cos (incl) * Math.Sin (asc);
												double Py = Math.Cos (arg) * Math.Sin (asc) + Math.Sin (arg) * Math.Cos (incl) * Math.Cos (asc);
												double Pz = Math.Sin (arg) * Math.Sin (incl);
												
												//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
												Vector3 P = new Vector3 ((float)Px, (float)Py, (float)Pz);
						
												double Qx = -Math.Sin (arg) * Math.Cos (asc) - Math.Cos (arg) * Math.Cos (incl) * Math.Sin (asc);
												double Qy = -Math.Sin (arg) * Math.Sin (asc) + Math.Cos (arg) * Math.Cos (incl) * Math.Cos (asc);
												double Qz = Math.Sin (incl) * Math.Cos (arg);
												//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
												Vector3 Q = new Vector3 ((float)Qx, (float)Qy, (float)Qz);
						
												double n = Math.Sqrt ((6.67384e-11) * (mass + massFocus) / (axis * axis * axis));
						
												//Read in the orbital period. 
												int counter = 0;
												//Used to log positions in text file:
												//string text, Rx, Ry, Rz;

												Vector3 R;
												Debug.Log("stepsize: " + stepSize);
						//Uncomment the line bellow to write data to a file. 
						//using (System.IO.StreamWriter dataFile = new System.IO.StreamWriter(@"/Users/zeev/Desktop/track.txt")) {

												for (double t = 0; t < t_final; t += stepSize) {
														

														R = findPos (anom, axis, ecc, Q, P);
														//Debug.Log ("Found positionof : " + gameObject.transform.name + R + "at time " + t);
														
														/**CODE TO write X Y Z POSITION OF PLANET to a file.
														if (t % (60*60*24) == 0) {
															Rx =  (R.x/1000).ToString("F6");
															Ry = (R.y/1000).ToString("F6");
															Rz = (R.z/1000).ToString("F6");
														
															text = Rx + " " + Ry + " " + Rz;

															dataFile.WriteLine(text);
														}*/
														track [counter] = R;
														timestamp[counter] = t;

														anom += n * stepSize;
														counter++;
												}
						//} Uncomment this line to write data to a file. 
						
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

		makeTrack();

		//Vector3 newposition = new Vector3((track[step]).x / Math.Pow(10, 8), (track[step]).y / Math.Pow(10, 8), (track[step]).z / Math.Pow(10, 8));
		gameObject.transform.position = track[step];
		Debug.Log(gameObject.transform.name + track[step]);
		step++;
		
	}
	
	// Use this for initialization
	void Start () {
		//Debug.Log(gameObject.name + ": Start");
		
	}
	
	// Update is called once per frame
	void Update () {

		//Vector3 newposition = new Vector3((track[step]).x / Math.Pow(10, 8), (track[step]).y / Math.Pow(10, 8), (track[step]).z / Math.Pow(10, 8));
		gameObject.transform.position = track[step];
		//Debug.Log(gameObject.transform.name + track[step]);
		
		step++;
		
		if (step >= ARRAY_SIZE) {
			step = 0;
		}
		
		
	}
	
	
}
