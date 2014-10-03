//	Project Voyager | Icarus Interstellar | Sep 24, 2014

using UnityEngine;
using System.Collections;

// From trackMove.cs
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

public class UpdateOrbitalElements : MonoBehaviour {

	void Start() {

	}

	void Update() {

	}

	/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the normal/anti-normal direction.
	*/
	void CalcNormalDeltaV(Elements &Initial, float DeltaV) {


		// Approximation of the true anomaly as a sine series of the mean anomaly
		double TrueAnomaly;
		TrueAnomaly = Inital.anom;
		TrueAnomaly += ((2 * Initial.ecc) - (0.25 * Mathf.Pow(Initial.ecc, 3))) * Mathf.Sin(Initial.anom);
		TrueAnomaly += (1.25 * Mathf.Pow(Initial.ecc, 2)) * Mathf.Sin(2 * Initial.anom);
		TrueAnomaly += ((13/12) * Mathf.Pow(Initial.ecc, 3)) * Mathf.Sin(3 * Initial.anom);

		/* Calculate the final inclination after the manoeuver */

		// Calculate the difference in inclination
		double DeltaI;
		double Numerator = DeltaV * (1 + (Initial.ecc * Mathf.Cos(TrueAnomaly)));
		double Denominator = 2 * Mathf.Sqrt(1 + Mathf.Pow(Initial.ecc, 2)) * Initial.n * Initial.axis * Mathf.Cos(Initial.arg + TrueAnomaly);
		DeltaI = 2 * Mathf.Asin(Numerator/Denominator);

		double FinalIncl = DeltaI + Initial.Incl;

		/* Calculate the final longitude (right ascension) of the ascending node */

		// Calculate the magnitude of the initial velocity
		double VelocityMag;
		Initial.calcData();
		VelocityMag = Initial.P.magnitude + Initial.Q.magnitude + Initial.W.magnitude;

		// Calculate the angle between the initial and the final orbits
		double AngleBtwnOrbits;
		AngleBtwnOrbits = 2 * Mathf.Asin(DeltaV / (2 * VelocityMag));

		// Calculate the argument of latitude
		double ArgOfLatitude;
		ArgOfLatitude = TrueAnomaly + Initial.arg;

		// Calculate the difference between the intial and the final longitudes of ascending node
		double DeltaOmega;
		DeltaOmega = Mathf.Sin(ArgOfLatitude) * Mathf.Sin(AngleBtwnOrbits) / Mathf.Sin(FinalIncl);
		DeltaOmega = Mathf.Asin(DeltaOmega);

		double FinalAsc = DeltaOmega + Inital.asc;

		// Because the manoeuver is only in the normal/anti-normal direction,
		// only the Inclination and Longitude of Ascending Node change
		Initial.incl = FinalIncl;
		Initial.asc = FinalAsc;

		// Calculate the final P, Q and n orbital elements after the manoeuver
		Initial.calcData();

	}
}