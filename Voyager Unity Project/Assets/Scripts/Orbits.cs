/*
 * Created by Jihad El Sheikh
 * 
 * This program creates a path that follows a body
 * 
 * Attached to: 	Orbit
 * 
 * Files needed:	None
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Orbits : MonoBehaviour
{
	private LineRenderer line;
	GameObject planet;		//store the gameobject of the planet
	float width;			//store the diameter of the planet
	Vector3 origin;			//the start position of the track
	Vector3 current;		//the current position of the track
	float diff, prev_diff;	//to compare the distance of one point to the previous point
	bool overlap;			//returns true if the track is complete
	bool reached_half;		//returns true if the track is half-complere
	int i;					//stores number of points in the track
	long time_step = 24 * 60 * 60;	//stepsize is a day
	float orbital_period;	//stores the time it takes for one orbit
	float mass;				//mass of focus body
	float radius;			//semi-major axis of orbit
	bool localPause;		//make sure the game track has moved before the game is paused

	bool isMoon = false;	//this variable indicates if the script is used for a moon
	string parentID;	//store the ID of the parent object
	int moonI = 0;	//calculates the number of points on the moon's orbit
	public List<Vector3> moonPositions = new List<Vector3>(); //list of points on the moon's orbit relative the parent planet
	
	
	public void makeMoonOrbit (long time, string body, string parent) {
		
		isMoon = true;
		
		//get the position of the parent
		parentID = parent;
		GameObject parentPlanet = GameObject.Find(parentID);
		Vector3 parentPosition = PcaPosition.findPos (parentPlanet.GetComponent<OrbitalElements> ().orb_elements, time, parentPlanet);
		
		planet = GameObject.Find (body);
		if (planet == null) {
			Debug.LogError ("Object not found to create path");
			return;
		}
		
		//set approximately 400 points for an orbit
		
		//get the mass of the focus body
		mass = (float)planet.GetComponent<OrbitalElements> ().orb_elements.massFocus;
		//get the semi-major axis
		radius = (float)planet.GetComponent<OrbitalElements> ().orb_elements.axis;
		//calculate the orbital period using Kepler's third law
		orbital_period = Mathf.Sqrt ((4 * Mathf.PI * Mathf.PI * Mathf.Pow (radius, 3)) / (6.67384e-11f * mass));
		//calculate the time_step to get about 400 points
		time_step = (long)(orbital_period / 400);
		
		i = 0;
		
		width = planet.transform.lossyScale.x;
		line = GetComponent<LineRenderer> ();
		//set track width at the beginnig and at the end
		line.SetWidth (width, width);
		
		//moon's position is parents position + moon's position relative to the parent
		origin = parentPosition + PcaPosition.findPos (planet.GetComponent<OrbitalElements> ().orb_elements, time, planet);
		//add the point to the list and increment the index
		moonPositions.Add(PcaPosition.findPos (planet.GetComponent<OrbitalElements> ().orb_elements, time, planet));
		moonI++;
		
		//increase the array of points by 1
		line.SetVertexCount (i + 1);
		//include the new point in the array of points
		line.SetPosition (i++, origin);
		//record it for use in the next cycle
		time += time_step;
		
		//keep adding points until the track is complete
		while (moonI != 401) {
			
			//get the current position of the track
			//moon's position is parents position + moon's position relative to the parent
			current = parentPosition + PcaPosition.findPos (planet.GetComponent<OrbitalElements> ().orb_elements, time, planet);
			//add the point to the list and increment the index
			moonPositions.Add(PcaPosition.findPos (planet.GetComponent<OrbitalElements> ().orb_elements, time, planet));
			moonI++;
			
			//the distance of the point from the origin
			diff = (current - origin).magnitude;

			//increase the array of points by 1
			line.SetVertexCount (i + 1);
			//include the new point in the array of points
			line.SetPosition (i++, current);
			//increase the time step
			time += time_step;
		}
		//Debug.Log ("body: " + body);
		//Debug.Log ("total time: " + time);
	}


		public void makeOrbit (long time, string body)
		{


				planet = GameObject.Find (body);
				if (planet == null) {
						Debug.LogError ("Object not found to create path");
						return;
				}

				//set approximately 400 points for an orbit

				//get the mass of the focus body
				mass = (float)planet.GetComponent<OrbitalElements> ().orb_elements.massFocus;
				//get the semi-major axis
				radius = (float)planet.GetComponent<OrbitalElements> ().orb_elements.axis;
				//calculate the orbital period using Kepler's third law
				orbital_period = Mathf.Sqrt ((4 * Mathf.PI * Mathf.PI * Mathf.Pow (radius, 3)) / (6.67384e-11f * mass));
				//calculate the time_step to get about 400 points
				time_step = (long)(orbital_period / 400);

				i = 0;
				diff = 0;
				overlap = false;
				reached_half = false;

				width = planet.transform.lossyScale.x;
				line = GetComponent<LineRenderer> ();
				//set track width at the beginnig and at the end
				line.SetWidth (width, width);
				origin = PcaPosition.findPos (planet.GetComponent<OrbitalElements> ().orb_elements, time, planet);
				//increase the array of points by 1
				line.SetVertexCount (i + 1);
				//include the new point in the array of points
				line.SetPosition (i++, origin);
				//record it for use in the next cycle
				prev_diff = diff;
				time += time_step;
		
				//keep adding points until the track is complete
				while (!overlap) {
			
						//get the current position of the track
						current = PcaPosition.findPos (planet.GetComponent<OrbitalElements> ().orb_elements, time, planet);
			
						//the distance of the point from the origin
						diff = (current - origin).magnitude;
			
						//if the distance is getting smaller, then half the track has been reached
						if (prev_diff > diff) {
								reached_half = true;
						}
			
						//if it's the second half of the track, and the different is increasing
						//then the track is complete
						if (prev_diff < diff && reached_half) {
								overlap = true;
				
						}
			
						//increase the array of points by 1
						line.SetVertexCount (i + 1);
						//include the new point in the array of points
						line.SetPosition (i++, current);
						//record it for use in the next cycle
						prev_diff = diff;
						time += time_step;
				}
				//Debug.Log ("body: " + body);
				//Debug.Log ("total time: " + time);
		}

		//set the width of the orbit
		public void setWidth(float newWidth){
			width = newWidth;
			line = GetComponent<LineRenderer> ();
			line.SetWidth (width, width);
			return;
		}

		//get the edith of the orbit
		public float getWidth(){
			return width;
		}

		// Use this for initialization
		void Start ()
		{

		}
	
		// Update is called once per frame
		// This is LateUpdate because we want the planet
		// to move first before getting its current position
	void LateUpdate ()
	{
		setWidth (0.002f*Camera.main.GetComponent<CameraUserControl> ().distance);

		//if the game is playing, then unflag localPause
		if (!Global.time_doPause) {
			localPause = false;
				}

		//if the object is a moon, need to update the orbit every frame
		//and the game is not on Pause
		if (isMoon && !localPause) {
			
			//find's the parent's current position
			Transform parent = GameObject.Find(parentID).transform;
			Vector3 parentPosition = parent.position;
			
			//update all points on the moon's orbit relative to parent's new position
			int index = 0;
			while (index < moonI) {
				//Debug.Log (index + " " + moonPositions[index]);
				line.SetPosition(index, parentPosition + moonPositions[index]);
				index++;
			}
			//this allows the Update method to run once after the game is paused to
			//ensure that the moons are at the same time_step as the planets
			if (Global.time_doPause){
				localPause = true;
			}
		}
	}
}
