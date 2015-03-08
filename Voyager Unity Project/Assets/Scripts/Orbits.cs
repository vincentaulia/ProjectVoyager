/*
 * Created by Jihad El Sheikh and Tirthak Patel
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

//used to get max number in an array
using System.Linq;

public class Orbits : MonoBehaviour
{
		/* GLOBALLY REQUIRED VARIABLES */

		/* Constant which stores the number of points used for the construction of the orbit */
		private static int NUM_ORBIT_POINTS = 800;

		/* Variables needed to draw the line */
		LineRenderer line;		// This stores the line of the orbit
		float width;			// The width of the line that draws the orbit

		/* Variables that store information related to the target body */
		GameObject spaceObject;		// Store the gameobject of the object
		GameObject parentObject;		//Store the object reference to the parent
		string bodyID;			// Store the ID of the target body
		string parentID;		// Store the ID of the parent object
		float orbitalPeriod;	// Stores the time it takes for one orbit
		float mass;				// Mass of focus body
		float radius;			// Semi-major axis of orbit

		/* Variables required for pause/unpause functionality */
		bool localPause;		// Used to make sure the game track has moved before the game is paused

		/* Variables that are required for time-related operations */
		long currentTime = 0;	// The current time (the time at which the body exists at the current position)
		long lastTime = 0;		// The time at which the last new orbit was constructed (used for track accuracy)
		long timeStep;			// Stepsize (required for the loop the constructs the orbit)

		/* Variables needed if the target body is a moon */
		bool isMoon = false;	//	This variable indicates if the script is used for a moon
		private List<Vector3> moonPositions = new List<Vector3> (); // List of points on the moon's orbit relative the parent planet

		Vector3[] corners = new Vector3[4];	//stores four corners of the object's track
		float[] distances = new float[4];	//stores the distances of the corners to a target body
		float maxDistance;					//the max distance of the distances array

		float maxRendering = 2000f;		//max rendering distance for moons
	

		/* This function is called in order to construct a moon's orbit */
		public void makeMoonOrbit (long time, string body, string parent)
		{

				// Tell the script that the object is a moon
				isMoon = true;

				// Store the ID of the body and the parent in a global variable (required for track accuracy)
				bodyID = body;
				parentID = parent;

				// Get the position of the parent
				parentObject = GameObject.Find (parentID);
				Vector3 parentPosition = PcaPosition.findPos (parentObject.GetComponent<OrbitalElements> ().orb_elements, time, parentObject);


				// Make sure that the space object exists
				spaceObject = GameObject.Find (body);
				if (spaceObject == null) {
						Debug.LogError ("Object not found to create path" + body);
						return;
				}

				// Get the mass, radius and orbital period (using Kepler's Third Law) of the focus body
				mass = (float)spaceObject.GetComponent<OrbitalElements> ().orb_elements.massFocus;
				radius = (float)spaceObject.GetComponent<OrbitalElements> ().orb_elements.axis;
				orbitalPeriod = Mathf.Sqrt ((4 * Mathf.PI * Mathf.PI * Mathf.Pow (radius, 3)) / (6.67384e-11f * mass));

				// Calculate the timeStep to get about 400 points
				timeStep = (long)(orbitalPeriod / NUM_ORBIT_POINTS);

				// Initialize the line element
				if (time == 0)
						line = GetComponent<LineRenderer> ();
		
				// Obtain the width of the object and set it as the width of the line
				width = spaceObject.transform.lossyScale.x;
				line.SetWidth (width, width);

				// The current position of the planet as required by the for loop
				Vector3 current;

				// Keep adding points until the track is complete
				for (int i = 0; i <= NUM_ORBIT_POINTS; i++) {
	
						// Get the current position of the track
						// Moon's position is parents position + moon's position relative to the parent
						current = parentPosition + PcaPosition.findPos (spaceObject.GetComponent<OrbitalElements> ().orb_elements, time, spaceObject);
			
						// Add the point to the list and increment the index
						moonPositions.Add (PcaPosition.findPos (spaceObject.GetComponent<OrbitalElements> ().orb_elements, time, spaceObject));

						// Increase the array of points by 1
						line.SetVertexCount (i + 1);
						//	Include the new point in the array of points
						line.SetPosition (i, current);
						// Increase the time step
						time += timeStep;
				}

				// Update the lastTime value to the time when this new orbit completes a round
				lastTime = time;
		}

		/* This function is called in order to construct a planet's orbit */
		public void makeOrbit (long time, string body)
		{
				// Store the ID of the body (required for track accuracy)
				bodyID = body;
				//Store the sun as the parent ID
				parentID = "10";

				// Make sure that the space object exists
				spaceObject = GameObject.Find (body);
				if (spaceObject == null) {
						Debug.LogError ("Object not found to create path1" + body);
						return;
				}

				// Get the mass, radius and orbital period (using Kepler's Third Law) of the focus body
				mass = (float)spaceObject.GetComponent<OrbitalElements> ().orb_elements.massFocus;
				radius = (float)spaceObject.GetComponent<OrbitalElements> ().orb_elements.axis;
				orbitalPeriod = Mathf.Sqrt ((4 * Mathf.PI * Mathf.PI * Mathf.Pow (radius, 3)) / (6.67384e-11f * mass));

				// Calculate the timeStep to get about 400 points
				timeStep = (long)(orbitalPeriod / NUM_ORBIT_POINTS);

				// Initialize the line element
				if (time == 0)
						line = GetComponent<LineRenderer> ();
		
				// Obtain the width of the object and set it as the width of the line
				width = spaceObject.transform.lossyScale.x;
				line.SetWidth (width, width);

				// The current position of the planet as required by the for loop
				Vector3 current;

				// Keep adding points until the track is complete
				for (int i = 0; i <= NUM_ORBIT_POINTS; i++) {
	
						// Get the current position of the track
						current = PcaPosition.findPos (spaceObject.GetComponent<OrbitalElements> ().orb_elements, time, spaceObject);

						// Increase the array of points by 1
						line.SetVertexCount (i + 1);
						// Include the new point in the array of points
						line.SetPosition (i, current);
						// Increment the time STEP
						time += timeStep;
						if (i % (NUM_ORBIT_POINTS / 4 + 1) == 0) {
								corners [i / NUM_ORBIT_POINTS] = current;
						}
				}

				// Update the lastTime value to the time when this new orbit completes a round
				lastTime = time;
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

		/*	Update is called once per frame
		This is LateUpdate because we want the planet
		to move first before getting its current position
	*/
		void LateUpdate ()
		{

				//if the object is a planet
				if (!isMoon) {
						//if it's the focus body or if's the parent of the moon in focus
						if (Camera.main.GetComponent<CameraUserControl> ().target.name == bodyID || Camera.main.GetComponent<CameraUserControl> ().target.parent.name == bodyID) {
								setWidth (0.0015f * Camera.main.GetComponent<CameraUserControl> ().distance);
						}
			//otherwise, calculate distances from four corners and compute the max distance
				else {
								for (int i=0; i<4; i++) {
										distances [i] = Vector3.Distance (Camera.main.transform.position, corners [i]);
								}
								maxDistance = distances.Max ();
								setWidth (0.001f * maxDistance);

						}
				}
		//if the object is a moon
		else {
						if (Vector3.Distance (parentObject.transform.position, Camera.main.transform.position) > maxRendering) {
								line.GetComponent<Renderer> ().enabled = false;
						} else {
								line.GetComponent<Renderer> ().enabled = true;
								setWidth (0.001f * Camera.main.GetComponent<CameraUserControl> ().distance);
						}
				}

				//	If the game is playing, then unflag localPause
				if (!Global.time_doPause) {
						localPause = false;
				}

				//	If the object is a moon and the game is unpaused, need to update the orbit every frame
				if (isMoon && !localPause) {

						// If the moon has finished the current orbit, recalculate the orbit at the current time
						// in order to improve track accuracy
						if (currentTime == lastTime) {
								moonPositions.Clear ();
								makeMoonOrbit (currentTime, bodyID, parentID);
						}

						//	Find's the parent's current position
						Transform parent = GameObject.Find (parentID).transform;
						Vector3 parentPosition = parent.position;

						//	Update all points on the moon's orbit relative to parent's new position
						for (int i = 0; i <= NUM_ORBIT_POINTS; i++) {
								line.SetPosition (i, parentPosition + moonPositions [i]);
						}

						//	This allows the Update method to run once after the game is paused in order to
						//	ensure that the moons are at the same timeStep as the planets
						if (Global.time_doPause) {
								localPause = true;
						}

						// Update the current time of the object's new position
						currentTime += timeStep;
				}

				// If the planet has finished one round of its current orbit, then create a new orbit
				// in order to improve track accuracy
				if (!isMoon && !localPause) {

						// If the planet has finished the current orbit, recalculate the orbit at the current time
						// in order to improve track accuracy
						if (currentTime == lastTime) {
								makeOrbit (currentTime, bodyID);
						}

						if (Global.time_doPause) {
								localPause = true;
						}

						// Update the current time of the object's new position
						currentTime += timeStep;
				}
		}
}
