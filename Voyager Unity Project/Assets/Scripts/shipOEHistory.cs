using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Globalization;
using System.Diagnostics; //For testing purposes only
using Debug = UnityEngine.Debug; //HUH

/* A class for storing the Orbital Elements of a ship and a time at which the Orbital
Elements change. The class also includes functions for calculating new OE based deltaV.*/


//NOTE: In the deltaV functions I added some changes as described below in the comments

public class shipOEHistory : MonoBehaviour
{
		public List <Elements> shipOE = new List<Elements> ();
		public List <long> shipT = new List<long> ();
		public GameObject shipObject;
		private int currentTimePos;
		private Vector3 def;
		public Stopwatch stopwatch = new Stopwatch(); //for testing purposes only
	
		/* Constructor, takes the first OE of a ship, the GamObject ship and the time at which it is created
        NOTE: ship GameObject is only needed for PcaPosition.findPos */
	
		//The first three argumets are for Orbital Elemnts script which is copied down below
	
		public void shipOEHistoryConstructor (Elements el, long time, GameObject shipObj)
		{
				//stopwatch.Start ();
				shipOE.Add (el);
				shipT.Add (time);
				shipObject = shipObj;
				currentTimePos = 0;
				//stopwatch.Stop(); // REMOVE LATER
				//Debug.Log (stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
		}	

		public string currentSphereOfInfluence() // 150 milisecond speed :/
		{
			//stopwatch.Start (); //REMOVE LATER
			Elements current = currentOE (Global.time);   //SHIP ELEMENTS
			Vector3 currentPos = findShipPos (Global.time);    //SHIP POSITION 
			
			GameObject parentPlanet = GameObject.Find (current.IDFocus);  //GAME OBJECT CYRRENT FOCCUS
			string IDF = null;
			IDF = focusDown (currentPos, parentPlanet);
//			Debug.Log ("IDF1");
//			Debug.Log (IDF);
			if (IDF != null)
			{
				//stopwatch.Stop(); // REMOVE LATER
				//Debug.Log (stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
				return IDF;
			}
			IDF = focusUp (currentPos, parentPlanet);
//			Debug.Log ("IDF2");
//			Debug.Log (IDF);
			//stopwatch.Stop(); //REMOVE LATER
			//Debug.Log (stopwatch.Elapsed.TotalMilliseconds); //REMOVE LATER
			return IDF;
		}
		
		public string focusUp(Vector3 shipP, GameObject GO)
		{
			GameObject parentPlanet = GameObject.Find (GO.GetComponent<OrbitalElements> ().orb_elements.IDFocus); //probably good
			string IDF = focusDown (shipP, parentPlanet);
			if (IDF == null)
				return focusUp (shipP - PcaPosition.findPos (parentPlanet.GetComponent<OrbitalElements> ().orb_elements, Global.time, GO), parentPlanet);
			else 
				return IDF;
		}
		
		public string focusDown(Vector3 shipP, GameObject GO)
		{
			int children = GO.transform.childCount;
			int numberRender = 0;
			bool flag = false;
			for (int i = 0; i < children; ++i)
			{	
				string currentTag = GO.transform.GetChild(i).gameObject.tag;
				if (currentTag != "Rendering" && currentTag != "name" && currentTag != "DistantPlanetIcon")
				{
					double radiusChild =  GO.transform.GetChild(i).gameObject.GetComponent<OrbitalElements> ().orb_elements.soi;
					Vector3 childPosition = PcaPosition.findPos (GO.GetComponent<OrbitalElements> ().orb_elements, Global.time, GO.transform.GetChild(i).gameObject);
					Vector3 shipRef = shipP - childPosition;
					double difference = (double)(shipRef).magnitude;
					difference = difference * Global.scale * 1000;
					
					if (difference <= radiusChild)
					{
						flag = true;
						return focusDown(shipRef, GO.transform.GetChild(i).gameObject);
					}
				}
				else
					numberRender++;
			}
			if (!flag)
			{
				double radiusParent = GO.GetComponent<OrbitalElements> ().orb_elements.soi;
				//Vector3 parentPosition = PcaPosition.findPos (GO.GetComponent<OrbitalElements> ().orb_elements, Global.time, GO);
				double difference = (double)(shipP).magnitude;
				difference = difference * Global.scale * 1000;
				if (difference <= radiusParent)
				{
					return GO.GetComponent<OrbitalElements> ().orb_elements.name;
				}
			}
			return null;
		}

		public void deltavChange (long time, Vector3 normal, Vector3 tangent, Vector3 radial)
		{
				Elements Initial = currentOE (Global.time);
				CalcNormalDeltaV (ref Initial, normal);
				CalcTangentialDeltaV (ref Initial, tangent);
				CalcRadialDeltaV (ref Initial, radial);
				AddNewOE (Initial, time);
		}
	
		public Elements currentOE (long time)  //EXECUTION TIME -- 0-1 miliseconds :)
		{
			//stopwatch.Start (); // remove later
				if (currentTimePos >= 1 && (currentTimePos + 1) < shipT.Count) 
				{
						if (time > shipT [currentTimePos - 1] && time < shipT [currentTimePos + 1]) 
								return shipOE [currentTimePos];
						else
						{
							int tpf = timePosFind (time);
							currentTimePos = tpf;
							//stopwatch.Stop(); //remove later
							//Debug.Log (stopwatch.Elapsed.TotalMilliseconds);
							return shipOE [tpf];
						}
				}
				else
				{
					int tpf = timePosFind (time);
					currentTimePos = tpf;
					//stopwatch.Stop(); //remove later
					//Debug.Log (stopwatch.Elapsed.TotalMilliseconds);
					return shipOE [tpf];
				}
				
		}
	
		public void AddNewOE (Elements toAdd, long time)
		{
				shipT.Add (time);
				shipOE.Add (toAdd);
		}
	
		public void removeOEpos (int pos)
		{
				shipT.RemoveAt (pos);
				shipOE.RemoveAt (pos);
		}
	
		public void removeOEtime (long time)
		{
				int pos = timePosFind (time);
				removeOEpos (pos);
		}
	
		public Vector3 findShipPos (long time)
		{
				return PcaPosition.findPos (currentOE (time), time, shipObject);
		}
	
		private int timePosFind (long time)
		{
				int start = 0;
				int end = shipT.Count;
				while (true) {
						int mid = (start + end) / 2;
						if (shipT [mid] == time)
								return mid;
						else if ((end - start) == 1)
								return start;
						else if (shipT [mid] > time)
								end = mid;
						else if (shipT [mid] < time)
								start = mid;
				}
		}
	
	
		//////////////////// Changed a bunch of Cin to Sin and Inital to Initial
	
		/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the normal/anti-normal direction.
		DeltaV varible should be in Perifocal Coordinates.
		*/
		void CalcNormalDeltaV (ref Elements Initial, Vector3 DeltaV)
		{
		
		
				// Approximation of the true anomaly as a sine series of the mean anomaly
				double TrueAnomaly;
				TrueAnomaly = Initial.anom;
				TrueAnomaly += ((2 * Initial.ecc) - (0.25f * System.Math.Pow (Initial.ecc, 3))) * System.Math.Sin (Initial.anom);
				TrueAnomaly += (1.25f * System.Math.Pow (Initial.ecc, 2)) * System.Math.Sin (2 * Initial.anom);
				TrueAnomaly += ((13 / 12) * System.Math.Pow (Initial.ecc, 3)) * System.Math.Sin (3 * Initial.anom);
		
				/* Calculate the final inclination after the maneuver */
		
				// Calculate the difference in inclination
				double DeltaI;
				double Numerator = (double)DeltaV.magnitude * (1 + (Initial.ecc * System.Math.Cos (TrueAnomaly)));
				double Denominator = 2 * System.Math.Sqrt (1 + System.Math.Pow (Initial.ecc, 2)) * Initial.n * Initial.axis * System.Math.Cos (Initial.arg + TrueAnomaly);
				DeltaI = 2 * System.Math.Asin (Numerator / Denominator);
		
				double FinalIncl = DeltaI + Initial.incl;
		
				/* Calculate the final longitude (right ascension) of the ascending node */
		
				// Calculate the magnitude of the initial velocity //
		
				// Calculate the standard gravitational parameter
				double Mu;
				Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
		
				// Calculate the specific relative angular momentum
				double h;
				h = System.Math.Sqrt (System.Math.Sqrt (1 - System.Math.Pow (Initial.ecc, 2)) * Initial.axis * Mu);
		
				// Calculate the initial velocity in Perifocal Coordinates
				Vector3 InitialVelocity;
				double IVp = -1 * Mu * System.Math.Sin (TrueAnomaly) / h;
				double IVq = (Initial.ecc + System.Math.Sin (TrueAnomaly)) * Mu / h; /////////////////////////////////CIN? changed to sin
				double IVw = 0;
				InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
		
				double VelocityMag = InitialVelocity.magnitude;
		
				// Calculate the angle between the initial and the final orbits
				double AngleBtwnOrbits;
				AngleBtwnOrbits = 2 * System.Math.Asin ((double)DeltaV.magnitude / (2 * VelocityMag));
		
				// Calculate the argument of latitude
				double ArgOfLatitude;
				ArgOfLatitude = TrueAnomaly + Initial.arg;
		
				// Calculate the difference between the intial and the final longitudes of ascending node
				double DeltaOmega;
				DeltaOmega = System.Math.Sin (ArgOfLatitude) * System.Math.Sin (AngleBtwnOrbits) / System.Math.Sin (FinalIncl);
				DeltaOmega = System.Math.Asin (DeltaOmega);
		
				double FinalAsc = DeltaOmega + Initial.asc;
		
				// Because the maneuver is only in the normal/anti-normal direction,
				// only the Inclination and Longitude of Ascending Node change
				Initial.incl = FinalIncl;
				Initial.asc = FinalAsc;
		
				// Calculate the final P, Q and n orbital elements after the maneuver
				Initial.calcData ();
		
		}
	
		/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the tangential/anti-tangential direction.
		DeltaV varible should be in Perifocal Coordinates.
	*/
		void CalcTangentialDeltaV (ref Elements Initial, Vector3 DeltaV)
		{
		
				// Approximation of the true anomaly as a sine series of the mean anomaly
				double TrueAnomaly;
				TrueAnomaly = Initial.anom;
				TrueAnomaly += ((2 * Initial.ecc) - (0.25f * System.Math.Pow (Initial.ecc, 3))) * System.Math.Sin (Initial.anom);
				TrueAnomaly += (1.25f * System.Math.Pow (Initial.ecc, 2)) * System.Math.Sin (2 * Initial.anom);
				TrueAnomaly += ((13 / 12) * System.Math.Pow (Initial.ecc, 3)) * System.Math.Sin (3 * Initial.anom);
		
				// Calculate the standard gravitational parameter
				double Mu;
				Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
		
				// Calculate the specific relative angular momentum
				double h;
				h = System.Math.Sqrt (System.Math.Sqrt (1 - System.Math.Pow (Initial.ecc, 2)) * Initial.axis * Mu);
		
				// Calculate the initial velocity in Perifocal Coordinates
				Vector3 InitialVelocity;
				double IVp = -1 * Mu * System.Math.Sin (TrueAnomaly) / h;
				double IVq = (Initial.ecc + System.Math.Sin (TrueAnomaly)) * Mu / h;
				double IVw = 0;
				InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
		
				// Calculate the magnitude of the initial velocity
				double VelocityMag = InitialVelocity.magnitude;
		
				double r;
				r = System.Math.Pow (h, 2) / Mu / (1 + (Initial.ecc * System.Math.Cos (TrueAnomaly)));
		
				// Calculate the new specific orbital energy
				double EnergNew;
				EnergNew = ((System.Math.Pow (VelocityMag, 2) + System.Math.Pow ((double)DeltaV.magnitude, 2)) / 2) - (Mu / r); 
		
				// Calculate the new semi-major axis //
				Initial.axis = -1 * Mu / 2 / EnergNew;
		
				// Calculate the orbital distance from object to primary
		
		
				 ///////////////////////////////////////////////////////////OLD LOCATION
		//double r;
		//r = System.Math.Pow(h, 2) / Mu / (1 + (Initial.ecc * System.Math.Cos(TrueAnomaly)));

		
		
				// Calculate the position of the object in Perifocal Coordinates
				Vector3 rVec;
				double rVp = r * System.Math.Cos (TrueAnomaly);
				double rVq = r * System.Math.Sin (TrueAnomaly);
				double rVw = 0;
				rVec = new Vector3 ((float)rVp, (float)rVq, (float)rVw);
		
				// Calculate the magnitude of the new angular velocity vector
				double hNewMag;
				hNewMag = (double)Vector3.Cross (rVec, InitialVelocity + DeltaV).magnitude;
		
				/////////// Calculate the new eccentricity ///////
				////////////////////////////////////////// hNew -------------->  hNewMagg ?????????????????????????????????????
				Initial.ecc = System.Math.Sqrt (1 + (System.Math.Pow (hNewMag, 2) * EnergNew / System.Math.Pow (Mu, 2)));
		
				////// Calculate the final P, Q, W and n orbital elements after the maneuver ///////
				Initial.calcData ();
		}

		/*	This function takes the initial orbital elements of the 
		ship and returns the final orbital elements based on the
		delta-V applied in the radial/anti-radial direction.
		DeltaV varible should be in Perifocal Coordinates.
		*/
		void CalcRadialDeltaV (ref Elements Initial, Vector3 DeltaV)
		{
		
				// Approximation of the true anomaly as a sine series of the mean anomaly
				double TrueAnomaly;
				TrueAnomaly = Initial.anom;
				TrueAnomaly += ((2 * Initial.ecc) - (0.25f * System.Math.Pow (Initial.ecc, 3))) * System.Math.Sin (Initial.anom);
				TrueAnomaly += (1.25f * System.Math.Pow (Initial.ecc, 2)) * System.Math.Sin (2 * Initial.anom);
				TrueAnomaly += ((13 / 12) * System.Math.Pow (Initial.ecc, 3)) * System.Math.Sin (3 * Initial.anom);
		
				// Calculate the standard gravitational parameter
				double Mu;
				Mu = 6.67384e-11 * (Initial.mass + Initial.massFocus);
		
				// Calculate the specific relative angular momentum
				double h;
				h = System.Math.Sqrt (System.Math.Sqrt (1 - System.Math.Pow (Initial.ecc, 2)) * Initial.axis * Mu);
		
				// Calculate the initial velocity in Perifocal Coordinates
				Vector3 InitialVelocity;
				double IVp = -1 * Mu * System.Math.Sin (TrueAnomaly) / h;
				double IVq = (Initial.ecc + System.Math.Sin (TrueAnomaly)) * Mu / h;
				double IVw = 0;
				InitialVelocity = new Vector3 ((float)IVp, (float)IVq, (float)IVw);
		
				// Calculate the magnitude of the initial velocity
				double VelocityMag = InitialVelocity.magnitude;
		
				// Calculate the orbital distance from object to primary
				double r;
				r = System.Math.Pow (h, 2) / Mu / (1 + (Initial.ecc * System.Math.Cos (TrueAnomaly)));
		
				// Calculate the new specific orbital energy
				double EnergNew;
				EnergNew = ((System.Math.Pow (VelocityMag, 2) + System.Math.Pow ((double)DeltaV.magnitude, 2)) / 2) - (Mu / r); 
		
				/***** Calculate the new semi-major axis *****/
				Initial.axis = -1 * Mu / 2 / EnergNew;
		
				// Calculate the magnitude of the new angular velocity vector
				double hNew;
				hNew = System.Math.Sqrt (Mu * Initial.axis * (1 - System.Math.Pow (Initial.ecc, 2)));
		
				/***** Calculate the new eccentricity *****/
				Initial.ecc = System.Math.Sqrt (1 + (System.Math.Pow (hNew, 2) * EnergNew / System.Math.Pow (Mu, 2)));
		
				/***** Calculate the final P, Q, W and n orbital elements after the maneuver *****/
				Initial.calcData ();
		}

		//return the number of Element structs in the list array
		public int getNumberOfElements ()
		{
				return shipOE.Count;
		}

		//return the start time for the selected Elements struct
		public long startTimeOE (Elements el)
		{
				for (int i=0; i < shipOE.Count; i++) {
						if(shipOE[i] == el){
							return shipT[i];
							break;
						}	
				}
				return -1;
		}

		//this functions adds new orbital elements.. given time, and the new semi-major axis
		public void dummyNode (long time, double newAxis)
		{
			Elements Initial = currentOE (Global.time);
			changeDummyAxis (ref Initial, newAxis);
			AddNewOE (Initial, time);
		}
		
		//changes the semi-major axis
		void changeDummyAxis(ref Elements Initial, double axis){
			Initial.axis = axis;
			Initial.calcData();
		}
}