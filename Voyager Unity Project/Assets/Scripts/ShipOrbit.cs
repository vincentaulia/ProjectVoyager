using UnityEngine;
using System.Collections;
//This is needed to use List
using System.Collections.Generic;

public class ShipOrbit : MonoBehaviour {


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

	public void createOrbit(long time, string shipName){
		ship = GameObject.Find (shipName);
		Elements el = ship.GetComponent<shipOEHistory> ().currentOE (time);
		GameObject parentPlanet = GameObject.Find (el.IDFocus);

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
		Vector3 parentPosition = PcaPosition.findPos (parentPlanet.GetComponent<OrbitalElements> ().orb_elements, time, parentPlanet);
		//Vector3 parentPosition = ship.GetComponent<shipOEHistory>().findShipPos (time);

		//calculate the time_step to get about 400 points
		time_step = (long)(orbital_period / 400);
		i = 0;
		
		//add the point to the list and increment the index
		points.Add (ship.GetComponent<shipOEHistory> ().findShipPos (time));
		//ship's position is parents position + ship's position relative to the parent
		origin = parentPosition + points[0];
		
		//increase the array of points by 1
		line.SetVertexCount (i + 1);
		//include the new point in the array of points
		line.SetPosition (i++, origin);
		//record it for use in the next cycle
		localTime = time + time_step;

		//keep adding points until the track is complete
		while (i != 402) {
			
			//get the current position of the track

			//add the point to the list and increment the index
			points.Add (ship.GetComponent<shipOEHistory> ().findShipPos (localTime));
			//ship's position is parents position + ship's position relative to the parent
			current = parentPosition + points[i];

			//increase the array of points by 1
			line.SetVertexCount (i + 1);
			//include the new point in the array of points
			line.SetPosition (i++, current);
			//increase the time step
			localTime += time_step;
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//if the game is playing, then unflag localPause
		if (!Global.time_doPause) {
			localPause = false;
		}
		
		//Need to update the orbit every frame
		//if the game is not on Pause
		if (!localPause) {
			
			//find's the parent's current position
			Transform parent = GameObject.Find (ship.GetComponent<shipOEHistory>().currentOE(Global.time).IDFocus).transform;
			Vector3 parentPosition = parent.position;
			
			//update all points on the moon's orbit relative to parent's new position
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
	}
}
