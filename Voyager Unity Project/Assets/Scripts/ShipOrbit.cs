using UnityEngine;
using System.Collections;

//This is needed to use List
using System.Collections.Generic;

public class ShipOrbit : MonoBehaviour
{


		private LineRenderer line;
		float width = 0.005f;
		public List<Vector3> points = new List<Vector3> ();
		long time_step;
		long localTime;
		float orbital_period;	//stores the time it takes for one orbit
		float mass;				//mass of focus body
		float radius;			//semi-major axis of orbit
		int i;					//stores number of points in the track

		Vector3 origin;			//the start position of the track
		Vector3 current;		//the current position of the track

		bool localPause;		//make sure the game track has moved before the game is paused
		GameObject ship;
		GameObject parentPlanet;	//store the reference to the planet object
		int orbits = 1;			//holds the number of orbits the ship has

		Vector3 parentPosition; //holds the position of the parent planet

		//float maxRendering = 2000f;		//max rendering distance for ships

		public void createOrbit (long time, string shipName)
		{
				ship = GameObject.Find (shipName);
				Elements el = ship.GetComponent<shipOEHistory> ().shipOE [0];
				parentPlanet = GameObject.Find (el.IDFocus);

				line = GetComponent<LineRenderer> ();
				//set track width at the beginnig and at the end
				line.SetWidth (width, width);

				//get the mass of the focus body
				mass = (float)el.massFocus;
				//get the semi-major axis
				radius = (float)el.axis;
				//calculate the orbital period using Kepler's third law
				orbital_period = Mathf.Sqrt ((4 * Mathf.PI * Mathf.PI * Mathf.Pow (radius, 3)) / (6.67384e-11f * mass));
				//get position of the body it is orbiting
				parentPosition = PcaPosition.findPos (parentPlanet.GetComponent<OrbitalElements> ().orb_elements, time, parentPlanet);
				//Vector3 parentPosition = ship.GetComponent<shipOEHistory>().findShipPos (time);

				//calculate the time_step to get about 400 points
				time_step = (long)(orbital_period / 400);
				i = 0;
		
				//add the point to the list and increment the index
				points.Add (ship.GetComponent<shipOEHistory> ().findShipPos (time));
				//ship's position is parents position + ship's position relative to the parent
				origin = parentPosition + points [0];
		
				//increase the array of points by 1
				line.SetVertexCount (i + 1);
				//include the new point in the array of points
				line.SetPosition (i++, origin);
				//record it for use in the next cycle
				localTime = time + time_step;

				//keep adding points until the track is complete
				while (i != 401) {

						//add the point to the list and increment the index
						points.Add (ship.GetComponent<shipOEHistory> ().findShipPos (localTime));
						//ship's position is parents position + ship's position relative to the parent
						current = parentPosition + points [i];

						//increase the array of points by 1
						line.SetVertexCount (i + 1);
						//include the new point in the array of points
						line.SetPosition (i++, current);
						//increase the time step
						localTime += time_step;
				}

		}

		public void makeAnotherOrbit (long time)
		{
				Elements el = ship.GetComponent<shipOEHistory> ().shipOE [orbits];

				//get the semi-major axis
				radius = (float)el.axis;
				//calculate the orbital period using Kepler's third law
				orbital_period = Mathf.Sqrt ((4 * Mathf.PI * Mathf.PI * Mathf.Pow (radius, 3)) / (6.67384e-11f * mass));
				//get position of the body it is orbiting
				//Vector3 parentPosition = PcaPosition.findPos (parentPlanet.GetComponent<OrbitalElements> ().orb_elements, time, parentPlanet);

				//calculate the time_step to get about 400 points
				time_step = (long)(orbital_period / 400);
				
				localTime = time;

				int j = i + 401;
				//keep adding points until the track is complete
				while (i != j) {
			
						//add the point to the list and increment the index
						points.Add (ship.GetComponent<shipOEHistory> ().findShipPos (localTime));
						//ship's position is parents position + ship's position relative to the parent
						current = parentPosition + points [i];
			
						//increase the array of points by 1
						line.SetVertexCount (i + 1);
						//include the new point in the array of points
						line.SetPosition (i++, current);
						//increase the time step
						localTime += time_step;
				}

		}

		// Set the width of the orbit
		public void setWidth (float newWidth)
		{
				width = newWidth;
				line = GetComponent<LineRenderer> ();
				line.SetWidth (width, width);
				return;
		}
	
		// Get the width of the orbit
		public float getWidth ()
		{
				return width;
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void LateUpdate ()
		{

				bool auto = VisualizeOrbits.auto;

				//if the the user is taking control and toggle is off
				if (!auto && !VisualizeOrbits.shipOrbits) {
					//turn tracks off
					line.GetComponent<Renderer> ().enabled = false;
					return;
				}

				//if the game is playing, then unflag localPause
				if (!Global.time_doPause) {
						localPause = false;
				}

				//if the focus is the ship, use the camera's distance
				if (Camera.main.GetComponent<CameraUserControl> ().target.name == ship.name) {
						setWidth (0.0015f * Camera.main.GetComponent<CameraUserControl> ().distance);
						line.GetComponent<Renderer> ().enabled = true;

				} else {
						float normal;
						//get the normal of the orbit at an angle of 60 degrees from the edge
						normal = (float)ship.GetComponent<OrbitalElements> ().orb_elements.axis * Mathf.Tan (Mathf.PI / 3);
						//scale it to Unity scale
						normal /= (Global.scale * 1000);
						//value determiend by trial and error
						normal *= 25;

						if (auto && Vector3.Distance (parentPlanet.transform.position, Camera.main.transform.position) > normal) {
								line.GetComponent<Renderer> ().enabled = false;
				
						} else {
								line.GetComponent<Renderer> ().enabled = true;
								setWidth (0.001f * Vector3.Distance (Camera.main.transform.position, parentPlanet.transform.position));
						}

				}

				//Need to update the orbit every frame
				//if the game is not on Pause
				if (!localPause) {
			
						//find's the parent's current position
						Transform parent = GameObject.Find (ship.GetComponent<shipOEHistory> ().currentOE (Global.time).IDFocus).transform;
						//Vector3 parentPosition = parent.position;
						parentPosition = parent.position;
			
						//update all points on the ship's orbit relative to parent's new position
						int index = 0;
						while (index < i) {
								//Debug.Log (index + " " + moonPositions[index]);
								line.SetPosition (index, parentPosition + points [index]);
								index++;
						}
						//this allows the Update method to run once after the game is paused to
						//ensure that the tracks are at the same time_step as the planets
						if (Global.time_doPause) {
								localPause = true;
						}
				}

				//if new orbital elements are added
				if (ship.GetComponent<shipOEHistory> ().getNumberOfElements () > orbits) {
						//make another orbit for them
						makeAnotherOrbit (ship.GetComponent<shipOEHistory> ().shipT [orbits]);
						orbits++;
				}
		}
}
