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
    bool isMoon = false;	//	This variable indicates if the script is used for a moon
    private List<Vector3> moonPositions = new List<Vector3>(); // List of points on the moon's orbit relative the parent planet

    Vector3[] corners = new Vector3[4];	//stores four corners of the object's track
    float[] distances = new float[4];	//stores the distances of the corners to a target body
    float maxDistance;					//the max distance of the distances array

    //float maxRendering = 2000f;		//max rendering distance for moons
    float normal = 0;				//the distance normal to the orbit

    GameObject cameraObject;			//stores a reference to the object in focus
    GameObject cameraParent;				//stores a reference to the parent of the object in focus

    float distanceFar;          //far distance where tracks become visible
    public int farMultiplier = 300;     //multiplies the standard distance for tracks to become visible
    public int closeMultiplier = 10;     //multiplies the standard distance for tracks to turn invisible

    public bool auto;          //holds the state of visualizing orbits

    /* This function is called in order to construct a planet's orbit */
    public void makeOrbit(long time, string body)
    {
        // Store the ID of the body (required for track accuracy)
        bodyID = body;
        //Store the sun as the parent ID
        parentID = "10";

        // Make sure that the space object exists
        spaceObject = GameObject.Find(body);
        if (spaceObject == null)
        {
            Debug.LogError("Object not found to create path " + body);
            return;
        }

        //get distantIcon of the planet
        if (spaceObject.tag == "Planet")
        {
            distantIcon = spaceObject.transform.GetChild(2);
        }
        //for asteroid or comet
        else
        {
            distantIcon = spaceObject.transform.GetChild(1);
        }
        
        // Get the mass, radius and orbital period (using Kepler's Third Law) of the focus body
        mass = (float)spaceObject.GetComponent<OrbitalElements>().orb_elements.massFocus;
        radius = (float)spaceObject.GetComponent<OrbitalElements>().orb_elements.axis;
        orbitalPeriod = (float)(2 * System.Math.PI * System.Math.Sqrt((System.Math.Pow((double)radius, 3)) / (6.67384E-11 * (double)mass)));
        //Debug.Log ("Orbits.cs: Body " + body + ", m=" + mass + ", r=" + radius + ", orbPeriod=" + orbitalPeriod);

        double major_axis = GameObject.Find(bodyID).GetComponent<OrbitalElements>().orb_elements.axis;
        double ecc = GameObject.Find(bodyID).GetComponent<OrbitalElements>().orb_elements.ecc;
        double minor_axis = System.Math.Sqrt((major_axis * major_axis) + System.Math.Pow(major_axis * ecc, 2));

        //Calculating number of points based on circumference of orbit.
        NUM_ORBIT_POINTS = (int)(0.0002959568 * (major_axis + minor_axis) / 1000000 + 1252);

        // Calculate the timeStep to get about 400 points
        timeStep = (long)(orbitalPeriod / NUM_ORBIT_POINTS);

        //if (body.StartsWith ("1") && body.Length > 3) {
        //Debug.Log ("Orbits.cs: " + body + "~ orbP = " + orbitalPeriod + ", #OrbPts = " + NUM_ORBIT_POINTS + ", tStep = " + timeStep + ", minor-axis = " + minor_axis/1E+12 + ", major-axis = " + major_axis/1E+12);
        //}

        // Initialize the line element
        if (time == 0)
        {
            line = GetComponent<LineRenderer>();
        }
        if (line == null)
        {
            Debug.Log("Line is null for object " + body + " at time " + time);
        }
        // Obtain the width of the object and set it as the width of the line
        width = spaceObject.transform.lossyScale.x;
        line.SetWidth(width, width);

        // The current position of the planet as required by the for loop
        Vector3 current;

        //set the number of points in the array
        line.SetVertexCount(NUM_ORBIT_POINTS + 2);

        // Keep adding points until the track is complete
        for (int i = 0; i <= NUM_ORBIT_POINTS; i++)
        {

            // Get the current position of the track
            current = PcaPosition.findPos(spaceObject.GetComponent<OrbitalElements>().orb_elements, time, spaceObject);

            // Increase the array of points by 1
            //line.SetVertexCount (i + 1);

            // Include the new point in the array of points
            line.SetPosition(i, current);
            // set the last point the same as the first point to close the loop
            if (i == 0)
            {
                line.SetPosition(NUM_ORBIT_POINTS + 1, current);
            }
            // Increment the time STEP
            time += timeStep;
            if (i % (NUM_ORBIT_POINTS / 4 + 1) == 0)
            {
                corners[i / NUM_ORBIT_POINTS] = current;
            }
        }

        // Update the lastTime value to the time when this new orbit completes a round
        lastTime = time;

        //Camera.main.GetComponent<CameraUserControl>().standardDistance * 300
        //if 
        //distanceFar *= Camera.main.GetComponent<CameraUserControl>().standardZoomCalc(spaceObject.transform, Camera.main.GetComponent<CameraUserControl>().standardZoomAngle);
    }

    // Set the width of the orbit
    public void setWidth(float newWidth)
    {
        width = newWidth;
        line = GetComponent<LineRenderer>();
        line.SetWidth(width, width);
        return;
    }

    // Get the width of the orbit
    public float getWidth()
    {
        return width;
    }

    /*	Update is called once per frame
    This is LateUpdate because we want the planet
    to move first before getting its current position
*/
    void LateUpdate()
    {
        if (this.tag == "OrbitPlanet")
        {

            //if it is set to auto
            if(VisualizeOrbits.auto && !VisualizeOrbits.a_planetOrbits) {
                //turn tracks off and exit
                line.GetComponent<Renderer>().enabled = false;
                distantIcon.GetComponent<Renderer>().enabled = false;
                return;
            }
            //if it is set to manual
            else if(!VisualizeOrbits.auto) {
                if (VisualizeOrbits.m_planetOrbits)
                {
                    setWidth(0.0015f * Camera.main.GetComponent<CameraUserControl>().distance);
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
            cameraObject = GameObject.Find(Camera.main.GetComponent<CameraUserControl>().target.name);
            //get the parent of object of focus
            cameraParent = GameObject.Find(cameraObject.transform.parent.name);

            //the ship is not a child of the planet
            //so need to adjust this manually
            if (cameraObject.tag == "Ship")
            {
                cameraParent = GameObject.Find(cameraObject.GetComponent<OrbitalElements>().orb_elements.IDFocus);
            }

            //if the object is not in focus
            //and the camera is close to it
            //turn other tracks off
            if (cameraObject != spaceObject)
            {
                //if the object in focus is a moon or a ship, base the cutoff distance on the planet it is orbiting
                if (cameraObject.tag == "Moon" || cameraObject.tag == "Ship")
                {
                    distanceFar = Camera.main.GetComponent<CameraUserControl>().getStandardDistance(cameraParent.transform) * farMultiplier;
                }
                else
                {
                    distanceFar = Camera.main.GetComponent<CameraUserControl>().getStandardDistance(cameraObject.transform) * farMultiplier;
                }
                if (Camera.main.GetComponent<CameraUserControl>().distance < distanceFar)
                {
                    line.GetComponent<Renderer>().enabled = false;
                    distantIcon.GetComponent<Renderer>().enabled = false;
                    return;
                }
                //if the camera is far, turn the tracks back on
                else
                {
                    line.GetComponent<Renderer>().enabled = true;
                }
            }
            //turn the track on for the object in focus
            else
            {
                //if you're close to the planet
                //turn off its tracks
                if (Camera.main.GetComponent<CameraUserControl>().distance < Camera.main.GetComponent<CameraUserControl>().standardDistance * closeMultiplier)
                {
                    line.GetComponent<Renderer>().enabled = false;
                    return;
                }
                line.GetComponent<Renderer>().enabled = true;
            }

            Vector3 cameraPosition = Camera.main.transform.position;

            //if it's the focus body or if it's the parent of the moon in focus
            if (Camera.main.GetComponent<CameraUserControl>().target.name == bodyID || Camera.main.GetComponent<CameraUserControl>().target.parent.name == bodyID)
            {
                setWidth(0.0015f * Camera.main.GetComponent<CameraUserControl>().distance);
            }
            //otherwise, calculate distances from four corners and compute the max distance
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    distances[i] = Vector3.Distance(cameraPosition, corners[i]);
                }
                maxDistance = distances.Max();

                //if the camera is not very close to the planet
                if (maxDistance > Vector3.Distance(spaceObject.transform.position, cameraPosition) * 4)
                {
                    setWidth(0.0015f * Vector3.Distance(spaceObject.transform.position, cameraPosition));
                }
                else
                {
                    setWidth(0.001f * maxDistance);
                }

            }
        }
        //if it is an asteroid or a comet
        else
        {
            if (this.tag == "OrbitComet")
            {
                //if it is set to auto
                if (VisualizeOrbits.auto && !VisualizeOrbits.a_cometOrbits)
                {
                    //turn tracks off and exit
                    line.GetComponent<Renderer>().enabled = false;
                    distantIcon.GetComponent<Renderer>().enabled = false;
                    return;
                }
                //if it is set to manual
                else if (!VisualizeOrbits.auto)
                {
                    if (VisualizeOrbits.m_cometOrbits)
                    {
                        setWidth(0.0015f * Camera.main.GetComponent<CameraUserControl>().distance);
                        line.GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        line.GetComponent<Renderer>().enabled = false;
                        distantIcon.GetComponent<Renderer>().enabled = false;
                    }
                    return;
                }
            }else if(this.tag == "OrbitAsteroid"){
                //if it is set to auto
                if (VisualizeOrbits.auto && !VisualizeOrbits.a_asteroidOrbits)
                {
                    //turn tracks off and exit
                    line.GetComponent<Renderer>().enabled = false;
                    distantIcon.GetComponent<Renderer>().enabled = false;
                    return;
                }
                //if it is set to manual
                else if (!VisualizeOrbits.auto)
                {
                    if (VisualizeOrbits.m_asteroidOrbits)
                    {
                        setWidth(0.0015f * Camera.main.GetComponent<CameraUserControl>().distance);
                        line.GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        line.GetComponent<Renderer>().enabled = false;
                        distantIcon.GetComponent<Renderer>().enabled = false;
                    }
                    return;
                }
            }

            //get the object of focus
            cameraObject = GameObject.Find(Camera.main.GetComponent<CameraUserControl>().target.name);
            //get the parent of object of focus
            cameraParent = GameObject.Find(cameraObject.transform.parent.name);

            //the ship is not a child of the planet
            //so need to adjust this manually
            if (cameraObject.tag == "Ship")
            {
                cameraParent = GameObject.Find(cameraObject.GetComponent<OrbitalElements>().orb_elements.IDFocus);
            }

            //if the object is not in focus
            //and the camera is close to it
            //turn the tracks off
            if (cameraObject != spaceObject)
            {
                //if the object in focus is a moon or a ship, base the cutoff distance on the planet it is orbiting
                if (cameraObject.tag == "Moon" || cameraObject.tag == "Ship")
                {
                    distanceFar = Camera.main.GetComponent<CameraUserControl>().getStandardDistance(cameraParent.transform) * farMultiplier;
                }
                else {
                    distanceFar = Camera.main.GetComponent<CameraUserControl>().getStandardDistance(cameraObject.transform) * farMultiplier;
                }

                if (Camera.main.GetComponent<CameraUserControl>().distance < distanceFar)
                {
                    line.GetComponent<Renderer>().enabled = false;
                    distantIcon.GetComponent<Renderer>().enabled = false;
                    return;
                }
                //if the camera is far, turn tracks back on
                else
                {
                    line.GetComponent<Renderer>().enabled = true;
                }
            }
            //turn the ran on for the objec in focus
            else
            {
                //if you're close to the body
                //turn off its tracks
                if (Camera.main.GetComponent<CameraUserControl>().distance < Camera.main.GetComponent<CameraUserControl>().standardDistance * closeMultiplier)
                {
                    line.GetComponent<Renderer>().enabled = false;
                    return;
                }
                line.GetComponent<Renderer>().enabled = true;
            }

            Vector3 cameraPosition = Camera.main.transform.position;

            //if it's the focus body
            if (Camera.main.GetComponent<CameraUserControl>().target.name == bodyID)
            {
                setWidth(0.0015f * Camera.main.GetComponent<CameraUserControl>().distance);
            }
            //otherwise, calculate distances from four corners and compute the max distance
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    distances[i] = Vector3.Distance(cameraPosition, corners[i]);
                }
                maxDistance = distances.Max();

                //if the camera is not very close to the body
                if (maxDistance > Vector3.Distance(spaceObject.transform.position, cameraPosition) * 4)
                {
                    setWidth(0.0015f * Vector3.Distance(spaceObject.transform.position, cameraPosition));
                }
                else
                {
                    setWidth(0.001f * maxDistance);
                }

            }


        }
        //	If the game is playing, then unflag localPause
        if (!Global.time_doPause)
        {
            localPause = false;
        }

        // If the planet has finished one round of its current orbit, then create a new orbit
        // in order to improve track accuracy
        if (!localPause)
        {

            // If the planet has finished the current orbit, recalculate the orbit at the current time
            // in order to improve track accuracy
            if (currentTime == lastTime)
            {
                makeOrbit(currentTime, bodyID);
            }

            if (Global.time_doPause)
            {
                localPause = true;
            }

            // Update the current time of the object's new position
            currentTime += timeStep;
        }

    }
}
