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

public class Orbits : MonoBehaviour
{
		
		
		private LineRenderer line;
		
		Transform planet;		//store the transform of the planet
		float width;			//store the diameter of the planet
		Vector3 origin;			//the start position of the track
		Vector3 current;		//the current position of the track
		float diff, prev_diff;	//to compare the distance of one point to the previous point
		bool overlap;			//returns true if the track is complete
		bool reached_half;		//returns true if the track is half-complere
		int i = 0;				//counts the number of points in the track

		// Use this for initialization
		void Start ()
		{

				planet = GameObject.Find ("399").transform;
				width = planet.localScale.x;
				line = GetComponent<LineRenderer> ();	
	
		}
	
		// Update is called once per frame
		// This is LateUpdate because we want the planet
		// to move first before getting its current position
		void LateUpdate ()
		{
		// ("Orbit: " + Global.body[83].transform.position);
				//keep adding points until the track is complete
				if (!overlap) {

						//set the origin of the track
						if (i == 0) {
								origin = planet.position;
						}

						//get the current position of the track
						current = planet.position;
						
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
						
						//set track width at the beginnig and at the end
						line.SetWidth (width, width);

						//increase the array of points by 1
						line.SetVertexCount (i + 1);
						//include the new point in the array of points
						line.SetPosition (i++, current);
						//record it for use in the next cycle
						prev_diff = diff;
				}


		}
}
