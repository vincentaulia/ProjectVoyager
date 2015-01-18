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
 * Jan 11, 2015
 * Updates:
 * - There is now a rotation limit at the top and bottom of the camera's rotation
 * 
 * In progress:
 * - 
*/

using UnityEngine;
using System.Collections;

[AddComponentMenu("Infinite Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitInfiniteRotateZoom : MonoBehaviour {
	
	public Transform target;			//the initial target. Should be earth or the sun.
	public float xSpeed = 12.0f;		
	public float ySpeed = 12.0f;		
	public float scrollSpeed = 10.0f;	//the standard scrolling speed
	public float zoomMin = 0.1f;		//minimal zoom distance
	public float zoomMax = 500.0f;		//maximum zooming distance
	public float thetaMin = 275.0f;		//minimum view angle (degrees)
	public float thetaMax = 85.0f;		//maximum view angle (degrees)
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
	
	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () {
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}

	void OnGUI ()
	{
		GUI.SetNextControlName ("CamTarget");
		input = GUI.TextField (new Rect (10, 10, 50, 20), input);
		
		//If Enter key is pressed when textfield is focus
		//change the target to be equal to the input's ID
		if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "CamTarget") {
			target = GameObject.Find(input).transform;
			transform.LookAt(target.position); // this forces the camera to always look at a moving object. Does not yet follow.
			position = transform.position - target.position;
			newPosition = -(transform.forward*standardDistance) + target.position;
			Debug.Log ("Position: " + position + "    New Position: " + newPosition);
			transform.position = newPosition;
		}

		//if button is pressed
		//change the target to be equal to the input's ID
		if (GUI.Button (new Rect (70, 10, 100, 25), "Change cam")) {
			target = GameObject.Find(input).transform;
			transform.LookAt(target.position); // this forces the camera to always look at a moving object. Does not yet follow.
			position = transform.position - target.position;
			newPosition = -(transform.forward*standardDistance) + target.position;
			Debug.Log ("Position: " + position + "    New Position: " + newPosition);
			transform.position = newPosition;
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Single Click Logged");
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayHitInfo;
			bool didHit = Physics.Raycast (mouseRay, out rayHitInfo);
		
			if (didHit) {
				Debug.Log (rayHitInfo.collider.name + " " + rayHitInfo.point);
				target = rayHitInfo.collider.transform;
				transform.LookAt(target.position); // this forces the camera to always look at a moving object. Does not yet follow.
				//standardDistance = 1.0f;
				position = transform.position - target.position;
				newPosition = -(transform.forward*standardDistance) + target.position;
				Debug.Log ("Position: " + position + "    New Position: " + newPosition);
				transform.position = newPosition;
			}
			else {
				Debug.Log ("Hit empty space");
			}
		}
	}

	void LateUpdate () {
		transform.LookAt(target.position); // this forces the camera to always look at a moving object. Does not yet follow.
		
		// only update if the mousebutton is held down
		if (Input.GetMouseButtonDown(1)){
			isActivated = true;
		} 
		
		// if mouse button is let UP then stop rotating camera
		if (Input.GetMouseButtonUp(1))
		{
			isActivated = false;	
		} 

		// here we ensure that the camera moves with the target object
		if (target) {
			position = transform.position - target.position;
			newPosition = -(transform.forward*distance) + target.position;
			transform.position = Vector3.Lerp (transform.position, newPosition, smooth*Time.deltaTime);
		}

		if (target && isActivated) { 
			//  get the distance the mouse moved in the respective direction
			x += Input.GetAxis("Mouse X") * xSpeed;
			y -= Input.GetAxis("Mouse Y") * ySpeed;	 

			Debug.Log ("y = " + transform.localEulerAngles.y + " x = " + transform.localEulerAngles.x);

			// prevent the camera from rotating to the 90 or 270 angle positions (top or bottom)
			if (transform.localEulerAngles.x <= thetaMin && transform.localEulerAngles.x >= 260.0f && y < 0) {
				Debug.Log ("Within lower stop zone");
				y = 0;
			}

			if (transform.localEulerAngles.x >= thetaMax && transform.localEulerAngles.x <= 100.0f && y > 0) {
				Debug.Log ("Within upper stop zone");
				y = 0;
			}

			// when mouse moves left and right we actually rotate around local y axis	
			transform.RotateAround(target.position,transform.up, x);

			// when mouse moves up and down we actually rotate around the local x axis
			transform.RotateAround(target.position,transform.right, y);
			
			// reset back to 0 so it doesn't continue to rotate while holding the button
			x=0;
			y=0; 	
		}
		else {		
			// see if mouse wheel is used 	
			if (Input.GetAxis("Mouse ScrollWheel") != 0) 
			{	
				// get the distance between camera and target
				distance = Vector3.Distance (transform.position , target.position);	

				// get mouse wheel info to zoom in and out	
				distance = ZoomLimit(distance - Input.GetAxis("Mouse ScrollWheel")*scrollSpeed, zoomMin, zoomMax);	
				
				// position the camera FORWARD the right distance towards target
				position = -(transform.forward*distance) + target.position;

				// move the camera
				transform.position = position; 
				//transform.position = Vector3.Lerp (transform.position, position, zoomSmooth*Time.deltaTime);
			}
		}
	}

	// This function prevents the camera from zooming beyond the maximum distance
	public static float ZoomLimit(float dist, float min, float max)
	{	
		if (dist < min)
			dist= min;
		
		if (dist > max)
			dist= max; 
		
		return dist;
	}

	// This function prevents the camera from rotating beyond the max/min angle
	public static float AngleLimit(float angle, float min, float max) {
		if (angle < min)
			angle = min;

		if (angle > max)
			angle = max;

		return angle;
	}
/*
	// This function calculates the standard distance away from a given target, based on it's size
	float stdDistance(Transform target) {
		Mathf.Max (target.localScale); //to use in leau of the radius, the biggest scale point (not a good measure)
		// distance = radius/tan(theta). For testing, let theta = pi/6, and radius = Mathf.Max(target.localScale)
		return (Mathf.Max (target.localScale)) / (Mathf.Tan ((Mathf.PI) / 6));
	}
*/
}


