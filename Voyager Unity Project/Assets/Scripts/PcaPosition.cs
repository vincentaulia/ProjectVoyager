/*  
 * 
 * This file contains the definitions of findPos and the struct Elements
 * 
 * FindPos:		Returns a vector with the position of a body at a given time and given orbital elements
 * Elements:	Holds the orbital elements of a body
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	None
 * 
 */
using UnityEngine;
using System.Collections;
using System;

public class PcaPosition : MonoBehaviour
{

		public static Vector3 findPos (Elements el, long time, GameObject body)
		{
				//This function finds the position of a planet given a bunch of orbital parameters and some other stuff.
		
				double anom = el.anom + el.n * time;
				double E = anom;
				double Enext = E;
				//Normally epsilon should be much smaller than this, but for now the program takes too long with small epsilons. 
				double epsilon = Math.Pow (10, -10);
				int count = 0;
		
				do {
						count ++;
						E = Enext;
						Enext = E - ((E - el.ecc * Math.Sin (E) - anom) / (1 - el.ecc * Math.Cos (E)));
			
				} while (Math.Abs(Enext - E) > epsilon && count < 15);
		
				if (count == 50) {
						Debug.Log ("Epsilon Crash: " + body.name);
				}
		
				Vector3 R;
				//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
				R = (float)(el.axis * (Math.Cos (E) - el.ecc)) * el.P + (float)(el.axis * Math.Sqrt (1 - el.ecc * el.ecc) * Math.Sin (E)) * el.Q;
		
				//interchange the y and z components to move the planets in the plane of the game
				float y = R.y;
				R.y = R.z;
				R.z = y;
		
				//scales down R to fit the program. The extra 1000 is for converting from m to km
				R = R / (Global.scale * 1000f);

				GameObject orbiting;
				
				//if it's a ship, get the id of it's orbit focus
				//if (body.name.Contains ("Ship")) {

				//orbiting = GameObject.Find (el.IDFocus);
				//R += orbiting.transform.position; 

				//}
				//Don't need this anymore. Will make moons children of planets
				/*else {



						//this is to add the vector to the object it is orbiting
						int objectID = int.Parse (body.name);
							
						//if the id ends with 99, then it orbits the sun (10)
						//if the id is something else, then it orbits a planet
						if (objectID == 10)
								;
						else if (objectID % 100 == 99) {
			
								orbiting = GameObject.Find ("10");
								//Debug.Log (body.name + ": " + orbiting.name);
								R += orbiting.transform.position;
						} else {
								int orbiting_id;
			
								orbiting_id = (objectID / 100) * 100 + 99;
								orbiting = GameObject.Find (orbiting_id.ToString ());
								//Debug.Log (body.name + ": " + orbiting.name);
								R += orbiting.transform.position;
			
						}


				}*/
		
				return R;
		}


}

public struct Elements
{
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
		//1 for Prograde and -1 for Retrograde
		public int dir;
		//ID of the body it is orbiting around
		public string IDFocus;
		//name of the body
		public string name;
		//sphere of influence
		public double soi;
		//radius of the body (in meters)
		public double radiusx;
		public double radiusy;
		public double radiusz;

		//needed for the ships
		public double dryMass;
		public double fuelMass;
		public double Isp;
		public double deltaVbudget;
	
		//The variables P, Q, and n must be calculated using the other orbital elements.
		//If any of the other orbital elements change, P, Q, and n must be recalculated. 
		//(the other orbital elements won't be changing for planets, but they will be for ships).
		public Vector3 P;
		public Vector3 Q;
		public Vector3 W;
		public double n;
	
		public void calcData ()
		{
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
				//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
				Q = new Vector3 ((float)Qx, (float)Qy, (float)Qz);

				//Calculating W
				double Wx = Math.Sin(incl) * Math.Sin(asc);
				double Wy = -1 * Math.Sin(incl) * Math.Cos(asc);
				double Wz = Math.Cos(incl);
				//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
				W = new Vector3 ((float)Wx, (float)Wy, (float)Wz);
		
				// Calculating n
				n = Math.Sqrt ((6.67384e-11) * (mass + massFocus) / (axis * axis * axis)) * dir;
				//Debug.Log (dir);
		}

		public static bool operator == (Elements x, Elements y)
		{
				Debug.Log ("x-axis: " + x.axis);
				Debug.Log ("y-axis: " + y.axis);
				if (x.axis != y.axis) {
						Debug.Log ("x-ecc: " + x.ecc);
						Debug.Log ("y-ecc: " + y.ecc);
						if (x.ecc == y.ecc) {
								Debug.Log ("x-incl: " + x.incl);
								Debug.Log ("y-incl: " + y.incl);
								if (x.incl == y.incl) {
										Debug.Log ("x-asc: " + x.asc);
										Debug.Log ("y-asc: " + y.asc);
										if (x.asc == y.asc) {
												Debug.Log ("x-anom: " + x.anom);
												Debug.Log ("y-anom: " + y.anom);
												if (x.anom == y.anom) {
														Debug.Log ("x-arg: " + x.arg);
														Debug.Log ("y-arg: " + y.arg);
														if (x.arg == y.arg) {
																Debug.Log ("x-dir: " + x.dir);
																Debug.Log ("y-adir " + y.dir);
																if (x.dir == y.dir) {
																		Debug.Log ("x-IDFocus: " + x.IDFocus);
																		Debug.Log ("y-IDFocus: " + y.IDFocus);
																		if (x.IDFocus == y.IDFocus) {
																				Debug.Log ("it is equal");
																				return true;
																		}
																}
														}
												}
										}
								}
						}
				}
				return false;
		}

		public static bool operator != (Elements x, Elements y)
		{
				return !(x == y);
		}
}