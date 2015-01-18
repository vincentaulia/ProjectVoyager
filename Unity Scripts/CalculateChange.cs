﻿//	Project Voyager | Icarus Interstellar | Sep 24, 2014

using UnityEngine;
using System.Collections;
using System;


// Component W of the Perifocal Coordinate system has been added to this struct
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
	
	//The variables P, Q, W and n must be calculated using the other orbital elements.
	//If any of the other orbital elements change, P, Q, W and n must be recalculated. 
	//(the other orbital elements won't be changing for planets, but they will be for ships).
	public Vector3 P;
	public Vector3 Q;
	public Vector3 W;
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
		//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
		Q = new Vector3 ((float)Qx, (float)Qy, (float)Qz);
		
		//Calculating W
		double Wx = Math.Sin(incl) * Math.Sin(asc);
		double Wy = -1 * Math.Sin(incl) * Math.Cos(asc);
		double Wz = Math.Cos(incl);
		//*********LOSS OF PRECISION HERE BY CONVERTING TO FLOATS.
		W = new Vector3 ((float)Wx, (float)Wy, (float)Wz);
		
		
		
		// Calculating n
		n = Math.Sqrt ((6.67384e-11) * (mass + massFocus) / (axis * axis * axis));
	}
	
}

public class test_calculate_changes : MonoBehaviour {
	
	void Start() {
		
		/***************************************
		 *HEY PHYSICS TEAM, PAY ATTENTION OVER HERE 
		 ****************************************/
		Elements testOrbElements;
		//Initialize this set of orbital elements to whatever you want it. 
		
		//call the CalcNormalDeltaV, CalcTangentialDeltaV, CalcRadialDeltaV functions here with testOrbElements as a parameter.
		
		//Outputting data
		//Use Debug.Log(testOrbElements.___); to output whatever you want. 
		
		
	}
	
	void Update() {
		
	}
	
	/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the normal/anti-normal direction.
		DeltaV varible should be in Perifocal Coordinates.
	*/
	void CalcNormalDeltaV(Elements Initial, Vector3 DeltaV) {
		
		
		// Approximation of the true anomaly as a sine series of the mean anomaly
		double TrueAnomaly;
		TrueAnomaly = Initial.anom;
		TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Math.Pow(Initial.ecc, 3))) * Math.Sin(Initial.anom);
		TrueAnomaly += (1.25 * Math.Pow(Initial.ecc, 2)) * Math.Sin(2 * Initial.anom);
		TrueAnomaly += ((13/12) * Math.Pow(Initial.ecc, 3)) * Math.Sin(3 * Initial.anom);
		
		/* Calculate the final inclination after the maneuver */
		
		// Calculate the difference in inclination
		double DeltaI;
		double Numerator = (double)DeltaV.magnitude * (1 + (Initial.ecc * Math.Cos(TrueAnomaly)));
		double Denominator = 2 * Math.Sqrt(1 + Math.Pow(Initial.ecc, 2)) * Initial.n * Initial.axis * Math.Cos(Initial.arg + TrueAnomaly);
		DeltaI = 2 * Math.Asin(Numerator/Denominator);
		
		double FinalIncl = DeltaI + Initial.incl;
		
		/* Calculate the final longitude (right ascension) of the ascending node */
		
		// Calculate the magnitude of the initial velocity //
		
		// Calculate the standard gravitational parameter
		double Mu;
		Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
		
		// Calculate the specific relative angular momentum
		double h;
		h = Math.Sqrt(Math.Sqrt(1 - Math.Pow(Initial.ecc, 2)) * Initial.axis * Mu);
		
		// Calculate the initial velocity in Perifocal Coordinates
		Vector3 InitialVelocity;
		double IVp = -1 * Mu * Math.Sin(TrueAnomaly) / h;
		double IVq = (Initial.ecc + Math.Sin(TrueAnomaly)) * Mu / h;
		double IVw = 0;
		InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
		
		double VelocityMag = InitialVelocity.magnitude;
		
		// Calculate the angle between the initial and the final orbits
		double AngleBtwnOrbits;
		AngleBtwnOrbits = 2 * Math.Asin((double)DeltaV.magnitude / (2 * VelocityMag));
		
		// Calculate the argument of latitude
		double ArgOfLatitude;
		ArgOfLatitude = TrueAnomaly + Initial.arg;
		
		// Calculate the difference between the intial and the final longitudes of ascending node
		double DeltaOmega;
		DeltaOmega = Math.Sin(ArgOfLatitude) * Math.Sin(AngleBtwnOrbits) / Math.Sin(FinalIncl);
		DeltaOmega = Math.Asin(DeltaOmega);
		
		double FinalAsc = DeltaOmega + Initial.asc;
		
		// Because the maneuver is only in the normal/anti-normal direction,
		// only the Inclination and Longitude of Ascending Node change
		Initial.incl = FinalIncl;
		Initial.asc = FinalAsc;
		
		// Calculate the final P, Q and n orbital elements after the maneuver
		Initial.calcData();
		
	}
	
	/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the tangential/anti-tangential direction.
		DeltaV varible should be in Perifocal Coordinates.
	*/
	void CalcTangentialDeltaV(Elements Initial, Vector3 DeltaV) {
		
		// Approximation of the true anomaly as a sine series of the mean anomaly
		double TrueAnomaly;
		TrueAnomaly = Initial.anom;
		TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Math.Pow(Initial.ecc, 3))) * Math.Sin(Initial.anom);
		TrueAnomaly += (1.25 * Math.Pow(Initial.ecc, 2)) * Math.Sin(2 * Initial.anom);
		TrueAnomaly += ((13/12) * Math.Pow(Initial.ecc, 3)) * Math.Sin(3 * Initial.anom);
		
		// Calculate the standard gravitational parameter
		double Mu;
		Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
		
		// Calculate the specific relative angular momentum
		double h;
		h = Math.Sqrt(Math.Sqrt(1 - Math.Pow(Initial.ecc, 2)) * Initial.axis * Mu);
		
		// Calculate the initial velocity in Perifocal Coordinates
		Vector3 InitialVelocity;
		double IVp = -1 * Mu * Math.Sin(TrueAnomaly) / h;
		double IVq = (Initial.ecc + Math.Sin(TrueAnomaly)) * Mu / h;
		double IVw = 0;
		InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
		
		// Calculate the magnitude of the initial velocity
		double VelocityMag = InitialVelocity.magnitude;
		
		// Calculate the orbital distance from object to primary
		double r;
		r = Math.Pow(h, 2) / Mu / (1 + (Initial.ecc * Math.Cos(TrueAnomaly)));


		// Calculate the new specific orbital energy
		double EnergNew;
		EnergNew = ((Math.Pow(VelocityMag, 2) + Math.Pow((double)DeltaV.magnitude, 2)) / 2) - (Mu / r); 
		
		/* Calculate the new semi-major axis */
		Initial.axis = -1 * Mu / 2 / EnergNew;
		

		// Calculate the position of the object in Perifocal Coordinates
		Vector3 rVec;
		double rVp = r * Math.Cos(TrueAnomaly);
		double rVq = r * Math.Sin(TrueAnomaly);
		double rVw = 0;
		rVec = new Vector3 ((float)rVp, (float)rVq, (float)rVw);
		
		// Calculate the magnitude of the new angular velocity vector
		double hNewMag;
		hNewMag = (double)Vector3.Cross(rVec, InitialVelocity + DeltaV).magnitude;
		
		/***** Calculate the new eccentricity *****/
		Initial.ecc = Math.Sqrt(1 + (Math.Pow(hNewMag, 2) * EnergNew / Math.Pow(Mu, 2)));
		
		/***** Calculate the final P, Q, W and n orbital elements after the maneuver *****/
		Initial.calcData();
	}
	
	/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the radial/anti-radial direction.
		DeltaV varible should be in Perifocal Coordinates.
	*/
	void CalcRadialDeltaV(Elements Initial, Vector3 DeltaV) {
		
		// Approximation of the true anomaly as a sine series of the mean anomaly
		double TrueAnomaly;
		TrueAnomaly = Initial.anom;
		TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Math.Pow(Initial.ecc, 3))) * Math.Sin(Initial.anom);
		TrueAnomaly += (1.25 * Math.Pow(Initial.ecc, 2)) * Math.Sin(2 * Initial.anom);
		TrueAnomaly += ((13/12) * Math.Pow(Initial.ecc, 3)) * Math.Sin(3 * Initial.anom);
		
		// Calculate the standard gravitational parameter
		double Mu;
		Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
		
		// Calculate the specific relative angular momentum
		double h;
		h = Math.Sqrt(Math.Sqrt(1 - Math.Pow(Initial.ecc, 2)) * Initial.axis * Mu);
		
		// Calculate the initial velocity in Perifocal Coordinates
		Vector3 InitialVelocity;
		double IVp = -1 * Mu * Math.Sin(TrueAnomaly) / h;
		double IVq = (Initial.ecc + Math.Sin(TrueAnomaly)) * Mu / h;
		double IVw = 0;
		InitialVelocity = new Vector3((float)IVp, (float)IVq, (float)IVw);
		
		// Calculate the magnitude of the initial velocity
		double VelocityMag = InitialVelocity.magnitude;
		
		// Calculate the orbital distance from object to primary
		double r;
		r = Math.Pow(h, 2) / Mu / (1 + (Initial.ecc * Math.Cos(TrueAnomaly)));
		
		// Calculate the new specific orbital energy
		double EnergNew;
		EnergNew = ((Math.Pow(VelocityMag, 2) + Math.Pow((double)DeltaV.magnitude, 2)) / 2) - (Mu / r); 
		
		/***** Calculate the new semi-major axis *****/
		Initial.axis = -1 * Mu / 2 / EnergNew;
		
		// Calculate the magnitude of the new angular velocity vector
		double hNew;
		hNew = Math.Sqrt(Mu * Initial.axis * (1 - Math.Pow(Initial.ecc, 2)));
		
		/***** Calculate the new eccentricity *****/
		Initial.ecc = Math.Sqrt(1 + (Math.Pow(hNew, 2) * EnergNew / Math.Pow(Mu, 2)));
		
		/***** Calculate the final P, Q, W and n orbital elements after the maneuver *****/
		Initial.calcData();
	}
}