/*
 * Created by Jihad El Sheikh
 * 
 * Position the camera near the object to be focused on, and aim the camera at the object.
 * Zoom in and zoom out. (Q and E keys)
 * Rotate the camera around the planet (WASD keys)
 * 
 * Attached to: Main Camera
 * 
 * Files needed:	None
 */

using UnityEngine;
using System.Collections;

public class jumpingCam : MonoBehaviour
{
		public string camTarget = "10";			//focus on the sun to begin with
		public static string input = "";
		public Transform body;
		public int setFieldOfView = 17;
		public bool showInfo;
		public float rotationX = 0, rotationY = 0, rotationD = 5 / 180 * Mathf.PI; //convert degrees to radians
		public float rotatedVertical = 0, rotatedHorizontal = 0;
		public const float rotation = 10;		//set the rotation per key pressed in degrees
		public bool rotate_right = false;

		void changeCam (string s)
		{
				//TO DO: Output a warning message if null


				//makes sure the object exists
				if (GameObject.Find (s) != null) {
						//reset the camera's fov
						this.camera.fieldOfView = setFieldOfView;

						//change the target of focus
						camTarget = s;

						//reset the rotation of the camera
						rotatedVertical = 0;
						rotatedHorizontal = 0;
				}
		}

		void OnGUI ()
		{
				GUI.SetNextControlName ("CamTarget");
				input = GUI.TextField (new Rect (10, 10, 50, 20), input);

				//If Enter key is pressed when textfield is focus
				//call the method to change the focus
				if (Event.current.Equals (Event.KeyboardEvent ("None")) && GUI.GetNameOfFocusedControl () == "CamTarget") {
						changeCam (input);
				}

				//if button is pressed
				//call the method to change the focus
				if (GUI.Button (new Rect (70, 10, 100, 25), "Change cam")) {
						changeCam (input);
				}
		}

		//change focus of the camera
		//read in the id of the required object
		void jump (string target)
		{
				Vector3 camPosition;
				float diameter;
				Vector3 bodyPosition;

				body = GameObject.Find (target).transform;
				bodyPosition = body.transform.position;		//vector of object's position

				//there's a built-in function for the else block
				//I think it's Lerp... I should use it....
				if (target == "10") {
						
						diameter = body.transform.localScale.x;
						camPosition = bodyPosition + new Vector3 (diameter * 3, diameter / 3, 0);
						
				} else {
	
						Vector3 sunPosition = GameObject.Find ("10").transform.position;
						Vector3 distance = sunPosition - bodyPosition;	//get vector from sun to object

						diameter = body.transform.localScale.x;

						float scale;
						scale = diameter * 4;	//position the camera at 4 times the length of the diameter

						distance *= scale / distance.magnitude;		//scales down the distance vector
																	//to the required length

						camPosition = bodyPosition + distance;		//calculate the final position of camera
	
				}
				
				this.camera.farClipPlane = diameter * 5;
				this.transform.position = camPosition;		//set the position of the camera
				transform.LookAt (body);		//aim the lens at the object
		}
		
		//rotate the camera left and right
		void rotateHorizontal ()
		{

				/* Made my own function before realising there's a built-in function for it
			Vector3 arm, newPosition;
			float dist;
			float angle;

			arm = this.transform.position;
			arm -= body.position;
			dist = arm.magnitude;
			angle = Mathf.Atan2(arm.z, arm.x);
			angle += rotatedHorizontal*Mathf.Deg2Rad;
			arm.x = dist * Mathf.Cos (angle);
			arm.z = dist * Mathf.Sin (angle);

			newPosition = body.position + arm;
			this.transform.position = newPosition;
			transform.LookAt (body);
			*/

				this.transform.RotateAround (body.position, Vector3.up, rotatedHorizontal);

		}
		
		//rotate the camera up and down
		void rotateVertical ()
		{
				/*
		Vector3 arm, newPosition;
		float dist;
		fl		oat angle;
		
		arm = this.transform.position;
		arm -= body.position;
		dist = arm.magnitude;
		angle = Mathf.Atan2(arm.y, arm.x);
		angle += rotatedVertical*Mathf.Deg2Rad;
		arm.x = dist * Mathf.Cos (angle);
		arm.y = dist * Mathf.Sin (angle);
		
		newPosition = body.position + arm;
		this.transform.position = newPosition;
		transform.LookAt (body);
		*/	
				Vector3 arm, newPosition, axis;
				arm = this.transform.position;		//position of the camera
				arm -= body.position;				//a vector from object to camera
		
				axis.x = 1;
				axis.y = 0;
				axis.z = -1 * (arm.x) / (arm.z);	//the axis of rotation is a vector that lies
													//in the x-z plane and points to the camera
		
				this.transform.RotateAround (body.position, axis, rotatedVertical);
		}

		// Use this for initialization
		void Awake ()
		{
				// DO nothing
		}

		void Update ()
		{

				//change the fov of camera depending on user's input
				if (this.camera.fieldOfView > 2 && Input.GetKeyDown (KeyCode.Q)) {
						this.camera.fieldOfView --;
				} else if (this.camera.fieldOfView < 25 && Input.GetKeyDown (KeyCode.E)) {
						this.camera.fieldOfView ++;
				
				}

				//rotate the camera depending on the user's input
				if (Input.GetKeyDown (KeyCode.D)) {
						rotatedHorizontal += rotation;

				} else if (Input.GetKeyDown (KeyCode.A)) {
						rotatedHorizontal -= rotation;
				} else if (Input.GetKeyDown (KeyCode.W)) {
						rotatedVertical += rotation;
				} else if (Input.GetKeyDown (KeyCode.S)) {
						rotatedVertical -= rotation;
				}

		}

		// Update is called once per frame
		//position the camera after the planets are positioned
		void LateUpdate ()
		{
				jump (camTarget);

				if (rotatedHorizontal != 0) {
						rotateHorizontal ();
				}
				if (rotatedVertical != 0) {
						rotateVertical ();
				}
		}
}
