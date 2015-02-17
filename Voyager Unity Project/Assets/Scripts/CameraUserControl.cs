/*
 * Created by Zachary Fejes and Jihad El Sheikh
 * 
 * Focuses the camera around a target object, and uses the mouse to change the view.
 * 
 * Controls:
 * Free rotate by holding down the right mouse button and moving the mouse.
 * Zoom in and out using the scroll wheel.
 * Single-click with left mouse button on an object to make it the target.
 * 
 * Note: the camera's max clip distance should be set slightly higher than the zoomMax to avoid clipping.
 * 
 * Attached to: Main Camera
 * 
 * Files needed:	None
 */

/* Update Notes:
 * Jan 7, 2015
 * Updates:
 * - The camera is jumping between planets now to a pre-set distance by using the GUI entry field
 * - The camera is jumping between planets to pre-set distance by using single left click
 * - Camera rotation is a bit shakey
 * - Camera zooming is linear
 * - Jumping between targets is instant
 * - Preset distance is constant
 * 
 * In progress
 * - Need to damp camera rotation to get rid of shake and make more comfortable
 * - Need to damp zooming (Lerp ?) for smooth zoom motion.
 * - Deed to interpolate fast path for smooth transitions
 * - Need to modify auto-target distance to find best view based on object size in view.
 * - Need to impliment non-linear zooming (slow when close, fast when distant)
 * - There is not yet a rotation limit at 5 and 175 degrees to prevent zero-ing problem.
 * - If you jump to the targetted it re-jumps to standard. Need to remove this: if(target = click.object) return;
 * 
 *  Jan 11, 2015
 * Updates:
 * - There is now a rotation limit at the top and bottom of the camera's rotation
 * - Multi-jump to same target issue resolved
 * 
 * In progress:
 * - Should make the smooth variable related to the speed of playback (for optimal smoothing)
 * - Need to make follow function based on relative position as opposed to given distance
 * 
 *  Jan 12, 2015
 * Updates:
 * - The 'LookAt' function has been removed, thus allowing completely free rotation
 * - Also removed the angle limits because free rotation doesn't require them
 * - Camera now follows the planets perfectly as they move (smooth = 100)
 * - Put all of the sun's layers on the non-raycast layer
 * - Linear zooming implimented (scales scroll speed with distance from zoomMin
 * - Camera zooming has been damped, though with the new linear zooming, it is not super necessary
 * 
 * In progress:
 * - Need to damp camera rotation (Lerp)
 * - Need to interpolate smooth transitions on movement between targets
 * - Need to build auto-target distance for best view (required radius or volumetric data)
 * 
 * BUGS:
 * - When pausing the simulation, it appears as though the orbial lines often pause a frame or two earlier than the objects
 * - I don't think the moon (Luna) is orbiting at the correct speed (everything else moves so much faster)
 * 
 *  Jan 15, 2015
 * Updates:
 * - We can do without damping the camera rotation for now. If we decide later on to do it, we'll handle it then
 * - Double click to change targets has been enabled. Single click currently does nothing.
 * 
 * In progress:
 * - Need to create single click functionality to select an object
 * - Smooth transitions between targets not working, probably because the update (instant move) kicks in immediately
 * 
 * 
 * Bugs:
 * - When pausing the simulation, it appears as though the orbial lines often pause a frame or two earlier than the objects
 * - I don't think the moon (Luna) is orbiting at the correct speed (everything else moves so much faster)
 * - When right clicking for the first time (for rotation) the camera instantly moves ~90 degress down
 * 
*/
using UnityEngine;
using System.Collections;

[AddComponentMenu("Infinite Camera-Control/Mouse Orbit with zoom")]
public class CameraUserControl : MonoBehaviour
{
	
		public Transform target;			//the initial target. Should be earth or the sun.
		public float xSpeed = 12.0f;
		public float ySpeed = 12.0f;
		public float scrollSpeed = 10.0f;	//the standard scrolling speed
		public float zoomMin = 0.1f;		//minimal zoom distance
		public float zoomMax = 500.0f;		//maximum zooming distance
		public float zoomMinAngle = Mathf.PI / 3;	//angle of the minimum zoom distance (in radians)
		//	public float thetaMin = 275.0f;		//minimum view angle (degrees)
		//	public float thetaMax = 85.0f;		//maximum view angle (degrees)
		public float distance;				//the distance at a given time between the camera and the target object
		public Vector3 position;			//the position of the camera
		public bool isActivated;			//triggers when the camera is being moved
		Vector3 newPosition;
		public float smooth = 0.01f;		//the smoothing factor for the 'camera follow' motion
		public float zoomSmooth = 0.01f;	//the smoothing factor for the 'zooming' motion
		private float lastClickTime = 0f;	//the timestamp of the most recent mouse click
		public float catchTime = 0.25f;		//the allowed time between clicks for a double click
		public static string input = "";	//the input value in the 'jump to object' GUI window
		public float standardDistance = 0.5f;		//this is a calculated value for the auto-position of a camera around a new target.
		float standardZoomAngle = Mathf.PI / 6;	//angle of the standard zoom distance (in radians)
	
		float x = 0.0f;
		float y = 0.0f;
	
		// Use this for initialization
		void Start ()
		{
				Vector3 angles = transform.eulerAngles;
				x = angles.y;
				y = angles.x;
				moveToNewTarget (target);
		}
	
		void OnGUI ()
		{
				GUI.SetNextControlName ("CamTarget");
				input = GUI.TextField (new Rect (10, 10, 50, 20), input);
		
				//If Enter key is pressed when textfield is focus
				//change the target to be equal to the input's ID
				if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "CamTarget") {
						if (target != GameObject.Find (input).transform) { 	// if it's not the current target
								target = GameObject.Find (input).transform;		//use the input to find the new target
								moveToNewTarget (target);						//move to focus the camera on the new target
						}
				}
		
				//if button is pressed
				//change the target to be equal to the input's ID
				if (GUI.Button (new Rect (70, 10, 100, 25), "Change cam")) {
						if (target != GameObject.Find (input).transform) { 	// if it's not the current target
								target = GameObject.Find (input).transform;		//use the input to find the new target
								moveToNewTarget (target);						//move to focus the camera on the new target
						}
				}
		}
	

	void Update () {
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayHitInfo;
		bool didHit = Physics.Raycast (mouseRay, out rayHitInfo, Mathf.Infinity);

		// Do mouse hover things here
		if (didHit) {
			if (rayHitInfo.collider.CompareTag("DistantPlanetIcon")) {
				Debug.Log ("Mousing Over " + rayHitInfo.collider.transform.parent.name);
			}
			else {
				Debug.Log ("Mousing Over " + rayHitInfo.collider.name);
		   }
		}
		else {
			Debug.Log ("");
		}


				if (Input.GetMouseButtonDown (0)) {
						if (Time.time - lastClickTime < catchTime) {
								//Debug.Log("Double Click Logged");
								// Do double left click things in here
								//Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
								//RaycastHit rayHitInfo;
								//bool didHit = Physics.Raycast (mouseRay, out rayHitInfo);

								if (didHit) {
										//Debug.Log (rayHitInfo.collider.name + " " + rayHitInfo.point);
										if (target != rayHitInfo.collider.transform) { // if we haven't clicked on the current target
												if (rayHitInfo.collider.CompareTag ("DistantPlanetIcon")) { // if we've clicked on a distant blue indicator
														target = rayHitInfo.collider.transform.parent.transform; // get the parent object transform.
														moveToNewTarget (target);					// and move to the new target
												} else { // if we've clicked directly on an object
														target = rayHitInfo.collider.transform;    	// make the clicked object the new target
														moveToNewTarget (target);					// and move to the new target
												}
										}
								} else {
										//Debug.Log ("Hit empty space");
								}
						} else {
								//Debug.Log("Single Click Logged");
								// Do single left click things in here
						}
						lastClickTime = Time.time;
				}
		}
	
		void LateUpdate ()
		{
				// only update if the mousebutton is held down
				if (Input.GetMouseButtonDown (1)) {
						isActivated = true;
				} 
		
				// if mouse button is let UP then stop rotating camera
				if (Input.GetMouseButtonUp (1)) {
						isActivated = false;	
				} 
		
				// here we ensure that the camera moves with the target object
				if (target) {
						position = transform.position - target.position;
						newPosition = -(transform.forward * distance) + target.position;
						transform.position = Vector3.Lerp (transform.position, newPosition, smooth * Time.deltaTime);
				}
		
				if (target && isActivated) { 
						//  get the distance the mouse moved in the respective direction
						x += Input.GetAxis ("Mouse X") * xSpeed;
						y -= Input.GetAxis ("Mouse Y") * ySpeed;	 
			
						//Debug.Log ("y = " + transform.localEulerAngles.y + " x = " + transform.localEulerAngles.x);

						// when mouse moves left and right we actually rotate around local y axis	
						transform.RotateAround (target.position, transform.up, x);
			
						// when mouse moves up and down we actually rotate around the local x axis
						transform.RotateAround (target.position, transform.right, y);
			
						// reset back to 0 so it doesn't continue to rotate while holding the button
						x = 0;
						y = 0; 	
				}
				else {		
						// see if mouse wheel is used 	
						if (Input.GetAxis ("Mouse ScrollWheel") != 0) {	
								// get the distance between camera and target
								distance = Vector3.Distance (transform.position, target.position);	
				
								// get mouse wheel info to zoom in and out	
								//distance = ZoomLimit(distance - Input.GetAxis("Mouse ScrollWheel")*scrollSpeed, zoomMin, zoomMax);
								distance = ZoomLimit (distance - Input.GetAxis ("Mouse ScrollWheel") * determineScrollSpeed (scrollSpeed, distance, standardDistance, zoomMin), zoomMin, zoomMax);
				
								// position the camera FORWARD the right distance towards target
								position = -(transform.forward * distance) + target.position;
				
								// move the camera
								//transform.position = position; 
								transform.position = Vector3.Lerp (transform.position, position, zoomSmooth * Time.deltaTime);
						}
				}
		}
	
		// This function prevents the camera from zooming beyond the maximum distance
		public static float ZoomLimit (float dist, float min, float max)
		{	
				if (dist < min)
						dist = min;
		
				if (dist > max)
						dist = max; 
		
				return dist;
		}
	
		// This function returns a scroll speed linearly proportional to the distance from the object (and zoomMin)
		float determineScrollSpeed (float standardScrollSpeed, float currentDistance, float standardDistance, float zoomMin)
		{
				if (currentDistance <= 1.005 * zoomMin)			//If we're at the zoomMin for the object
						return 1;							//return a speed of 1, to prevent a zero scroll speed error
				return (currentDistance - zoomMin);		//otherwise, return the new scroll speed
		}
	
		// This function repositions the camera to focus on a newly selected target
		void moveToNewTarget (Transform target)
		{
				transform.LookAt (target.position); 							// this forces the camera to always look at a moving object. Does not yet follow.
				position = transform.position - target.position; 			// determine our position relative to our new target (useless)
				//newPosition = -(transform.forward*standardDistance) + target.position; //set new position at standard distance from target
				newPosition = -(transform.forward * standardZoomCalc (target, standardZoomAngle)) + target.position;
				zoomMinCalc (target);	// determine the new 
				//Debug.Log ("Position: " + position + "    New Position: " + newPosition);
				transform.position = Vector3.Lerp (transform.position, newPosition, 0.001f * Time.smoothDeltaTime); //smoothly move us to the new position
				distance = standardDistance;
				return;
		}

		// This function returns the zoomMin distance for the current target
		void zoomMinCalc (Transform target)
		{
				// find the maximum target radius, of three dimension radius
				// determine the distance necessary to make a ___ degree angle with the maximum radius ( return maxRadius/tan(theta);
				float[] radius = {(float)target.GetComponent<OrbitalElements> ().orb_elements.radiusx,
			             	(float)target.GetComponent<OrbitalElements> ().orb_elements.radiusy,
							(float)target.GetComponent<OrbitalElements> ().orb_elements.radiusz};
				zoomMin = (Mathf.Max (radius) + Mathf.Max (radius) * 0.000001f) / 50000000;
				//Debug.Log ("zoomMin = " + zoomMin);
				return;
		}

		// This function returns the standardZoom distance for the current target
		float standardZoomCalc (Transform target, float standardZoomAngle)
		{
				if (target.name == "10") {
					standardDistance = 70.0f;
					distance = standardDistance;
					return 70.0f;
				}
				// find the maximum target radius of three dimension radius
				// determine the distance necessary to make a ____ degree angle with the max radius 
				float[] radius = {(float)target.GetComponent<OrbitalElements> ().orb_elements.radiusx,
							(float)target.GetComponent<OrbitalElements> ().orb_elements.radiusy,
							(float)target.GetComponent<OrbitalElements> ().orb_elements.radiusz};
				//Debug.Log ("rad1 = " + radius[0] + "   rad2 = " + radius[1] + "    rad3 = " + radius[2] + "   MaxRad = " + Mathf.Max (radius)/Mathf.Tan(standardZoomAngle));
				standardDistance = (Mathf.Max (radius) / Mathf.Tan (standardZoomAngle)) / 20000000;
				distance = standardDistance;
				return standardDistance;
		}
}


