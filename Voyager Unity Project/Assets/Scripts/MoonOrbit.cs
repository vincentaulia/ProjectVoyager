/*
 * Created by Jihad El Sheikh and Tirthak Patel
 * 
 * This program creates a path that follows a body
 * 
 * Attached to: 	MoonOrbit
 * 
 * Files needed:	None
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//used to get max number in an array
using System.Linq;

public class MoonOrbit : MonoBehaviour
{
		/* GLOBALLY REQUIRED VARIABLES */
	
		/* Constant which stores the number of points used for the construction of the orbit */
		private int NUM_ORBIT_POINTS;
	
		/* Variables needed to draw the line */
		LineRenderer line;		// This stores the line of the orbit
		float width;			// The width of the line that draws the orbit
	
		/* Variables that store information related to the target body */
		GameObject spaceObject;		// Store the gameobject of the object
		GameObject parentObject;	//Store the object reference to the parent
		Transform distantIcon;	//Store the planet's distantIcon.
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
		private List<Vector3> moonPositions = new List<Vector3> (); // List of points on the moon's orbit relative the parent planet
	
		//float maxRendering = 2000f;		//max rendering distance for moons
		float normal = 0;				//the distance normal to the orbit
	
		GameObject cameraObject;			//stores a reference to the object in focus
		GameObject cameraParent;				//stores a reference to the parent of the object in focus

        public int closeMultiplier = 5;     //multiplies the standard distance for tracks to turn invisible
	
		/* This function is called in order to construct a moon's orbit */
		public void makeMoonOrbit (long time, string body, string parent)
		{
		
				// Store the ID of the body and the parent in a global variable (required for track accuracy)
				bodyID = body;
				parentID = parent;
		
				//The semi major axis of the body, used to determine the number of points drawn. 
				double major_axis = GameObject.Find (bodyID).GetComponent<OrbitalElements> ().orb_elements.axis;
				double ecc = GameObject.Find (bodyID).GetComponent<OrbitalElements> ().orb_elements.ecc;
				double minor_axis = System.Math.Sqrt ((major_axis * major_axis) + System.Math.Pow (major_axis * ecc, 2));
				
				NUM_ORBIT_POINTS = (int)(90.483 * System.Math.Log((major_axis + minor_axis)/1000000) - 191.67);
				if (NUM_ORBIT_POINTS < 100) {
					NUM_ORBIT_POINTS = 100;
				}
		
				// Get the position of the parent
				parentObject = GameObject.Find (parentID);
				Vector3 parentPosition = PcaPosition.findPos (parentObject.GetComponent<OrbitalElements> ().orb_elements, time, parentObject);
		
		
				// Make sure that the space object exists
				spaceObject = GameObject.Find (body);
				if (spaceObject == null) {
						Debug.LogError ("Object not found to create path" + body);
						return;
				}
				
				//This will stop working if moons get more child objects before the distant icon. I think it's less robust, so it's commented out.
				//distantIcon = spaceObject.transform.GetChild (1); 
				//This will stop working if moon distant Icon object name is changed
				distantIcon = spaceObject.transform.Find ("Planet Distance Icon");

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
		
				//set the number of points in the array
				line.SetVertexCount (NUM_ORBIT_POINTS + 2);
		
				// Keep adding points until the track is complete
				for (int i = 0; i <= NUM_ORBIT_POINTS; i++) {
			
						// Get the current position of the track
						// Moon's position is parents position + moon's position relative to the parent
						current = parentPosition + PcaPosition.findPos (spaceObject.GetComponent<OrbitalElements> ().orb_elements, time, spaceObject);
			
						// Add the point to the list and increment the index
						moonPositions.Add (PcaPosition.findPos (spaceObject.GetComponent<OrbitalElements> ().orb_elements, time, spaceObject));
			
						// Increase the array of points by 1
						//line.SetVertexCount (i + 1);
			
						//	Include the new point in the array of points
						line.SetPosition (i, current);
			
						// set the last point the same as the first point to close the loop
						if (i == 0) {
								line.SetPosition (NUM_ORBIT_POINTS + 1, current);
						}
			
						// Increase the time step
						time += timeStep;
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

				//if it is set to auto
                if (VisualizeOrbits.auto && !VisualizeOrbits.a_moonOrbits)
                {
                    //turn tracks off and exit
                    line.GetComponent<Renderer>().enabled = false;
                    distantIcon.GetComponent<Renderer>().enabled = false;
                    return;
                }
                //if it is set to manual
                else if (!VisualizeOrbits.auto)
                {
                    if (VisualizeOrbits.m_moonOrbits)
                    {
                        setWidth(0.001f * Camera.main.GetComponent<CameraUserControl>().distance);
                        line.GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        line.GetComponent<Renderer>().enabled = false;
                        distantIcon.GetComponent<Renderer>().enabled = false;
                    }
                    return;
                }

				//get the object of focus
				cameraObject = GameObject.Find (Camera.main.GetComponent<CameraUserControl> ().target.name);
				//get the parent of object of focus
				cameraParent = GameObject.Find (cameraObject.transform.parent.name);

                //the ship is not a child of the planet
                //so need to adjust this manually
                if (cameraObject.tag == "Ship")
                {
                    cameraParent = GameObject.Find(cameraObject.GetComponent<OrbitalElements>().orb_elements.IDFocus);
                }

				//reset the appearance of the tracks
				line.GetComponent<Renderer> ().enabled = true;
		
				Vector3 cameraPosition = Camera.main.transform.position;
				//if the object is a planet

				//if the orbits are that of the moon of another planet, turn them off
				if (spaceObject == cameraObject || parentObject == cameraObject || parentObject == cameraParent) {
                    //if the camera is too close to the moon
                    //turn tracks off
                    if (Camera.main.GetComponent<CameraUserControl>().distance < Camera.main.GetComponent<CameraUserControl>().standardDistance * closeMultiplier)
                    {
                        line.GetComponent<Renderer>().enabled = false;
                        distantIcon.GetComponent<Renderer>().enabled = false;
                        return;
                    }
						//get the normal of the orbit at an angle of 60 degrees from the edge
						normal = (float)spaceObject.GetComponent<OrbitalElements> ().orb_elements.axis * Mathf.Tan (Mathf.PI / 3);
						//scale it to Unity scale
						normal /= (Global.scale * 1000);
						//value determiend by trial and error
						normal *= 25;
				
						if (Vector3.Distance (parentObject.transform.position, cameraPosition) > normal) {
								line.GetComponent<Renderer> ().enabled = false;
								distantIcon.GetComponent<Renderer> ().enabled = false;
					
						} else {
								line.GetComponent<Renderer> ().enabled = true;
								setWidth (0.001f * Camera.main.GetComponent<CameraUserControl> ().distance);
						}
				} else {
						line.GetComponent<Renderer> ().enabled = false;
						distantIcon.GetComponent<Renderer> ().enabled = false;
				}

				//	If the game is playing, then unflag localPause
				if (!Global.time_doPause) {
						localPause = false;
				}

				//	If the game is unpaused, need to update the orbit every frame
				if (!localPause) {
			
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
						for (int i = 0; i < NUM_ORBIT_POINTS+1; i++) {
								line.SetPosition (i, parentPosition + moonPositions [i]);
						}
                        //manually fix the last point to equal the first point in order to close the track
                        line.SetPosition(NUM_ORBIT_POINTS+1, parentPosition + moonPositions[0]);
			
						//	This allows the Update method to run once after the game is paused in order to
						//	ensure that the moons are at the same timeStep as the planets
						if (Global.time_doPause) {
								localPause = true;
						}
			
						// Update the current time of the object's new position
						currentTime += timeStep;
				}
		}
}
