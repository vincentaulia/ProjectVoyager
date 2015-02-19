/*
 * Created by Jihad El Sheikh
 * 
 * This program moves all the bodies accroding to PCA
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	None
 */
using UnityEngine;
using System.Collections;

public class MovePca : MonoBehaviour
{

		bool doPaws = false;		//controls pausing the game, not used right now. Global variable is used instead.
		public Rect button;		//holds the dimensions of the button
		int pic = 0;			//holds whether it's a pause or play
		public Texture[] pawsPic = new Texture[2];	//element 0 is play. element 1 is pause
		GameObject bary;
		bool onlyonce = true;

		// Use this for initialization
		void Start ()
		{
				Debug.Log ("count: " + Global.body.Count);

				//sets the starting positions of all bodies except for the sun
				for (int i=1; i<Global.body.Count; i++) {
						Global.body [i].transform.localPosition = PcaPosition.findPos (Global.body [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.body [i]);
						//lossy works only when all x,y,z have the same scale
						//the localPosition need to be scaled relative to the scale of the parent
						Global.body [i].transform.localPosition /= Global.body [i].transform.parent.lossyScale.x;
				}

				//for the ships
				for (int i=0; i<Global.ship.Count; i++) {
						Global.ship [i].transform.position = Global.ship [i].GetComponent<shipOEHistory> ().findShipPos (Global.time);
						//get object that it is orbiting
						GameObject orbiting = GameObject.Find (Global.ship [i].GetComponent<shipOEHistory> ().currentOE (Global.time).IDFocus);
						//add position of ship to the position of planet it is orbiting
						Global.ship [i].transform.position += orbiting.transform.position;
						//OLD Global.ship [i].transform.position = PcaPosition.findPos (Global.ship [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.ship [i]);

				}
				button = new Rect (10, 170, 60, 60);

				bary = GameObject.Find ("Bary Center");
		}
	
		// Update is called once per frame
		/*void Update () {

		//if the P button is pressed.. toggle pausing the game
		if (Input.GetKeyDown(KeyCode.P)) {
			Paws ();
		}

		//only advance the time if the game is not paused
		if (!doPaws) {
			Global.time += 24*60*60;

			//updates the positions of all bodies except for the sun
			for (int i=1; i<Global.body.Count; i++) {
				Global.body[i].transform.position = PcaPosition.findPos (Global.body[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.body[i]);
			}

			//move the ships
			for (int i=0; i<Global.ship.Count; i++) {

				Global.ship[i].transform.position = PcaPosition.findPos (Global.ship[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.ship[i]);
			}
		}
	 }
*/

		// Update is called once per frame
		void FixedUpdate ()
		{
				if (!Global.time_doPause) {
						Global.time += (long)(Global.time_stepsize * Global.time_multiplier);
						//if time goes into negative, rest the stepsize and multiplier to positive values
						//pause the game
						if (Global.time < 0) {
								Global.time = 0;
								Debug.Log ("Time is going into negative.");
								Global.time_stepsize = Mathf.Abs (Global.time_stepsize);
								Global.time_multiplier = Mathf.Abs (Global.time_multiplier);

								bary.GetComponent<TimeJump> ().doPause ();

						}
						//move the planets and moons
						//starts from 1 to skip the sun
						for (int i=1; i<Global.body.Count; i++) {
								Global.body [i].transform.localPosition = PcaPosition.findPos (Global.body [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.body [i]);
								//lossy works only when all x,y,z have the same scale
								//the localPosition need to be scaled relative to the scale of the parent
								Global.body [i].transform.localPosition /= Global.body [i].transform.parent.lossyScale.x;
						}
			
						//move the ships
						for (int i=0; i<Global.ship.Count; i++) {
								Global.ship [i].transform.position = Global.ship [i].GetComponent<shipOEHistory> ().findShipPos (Global.time);
								//get object that it is orbiting
								GameObject orbiting = GameObject.Find (Global.ship [i].GetComponent<shipOEHistory> ().currentOE (Global.time).IDFocus);
								//add position of ship to the position of planet it is orbiting
								Global.ship [i].transform.position += orbiting.transform.position;
								//OLD Global.ship [i].transform.position = PcaPosition.findPos (Global.ship [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.ship [i]);
						}
				}

		/*
				if (onlyonce) {
						GameObject earth = GameObject.Find ("399");
						GameObject thing = GameObject.Find ("199");
						Elements one = earth.GetComponent<OrbitalElements> ().orb_elements;
						Elements two = thing.GetComponent<OrbitalElements> ().orb_elements;
						if (one == two) {
								Debug.Log ("YES equal");
						}else{
				Debug.Log ("NOT EQUAL");
			}

						onlyonce = false;
				}*/


		}

		void OnGUI ()
		{
				if (GUI.Button (button, pawsPic [pic])) {
						bary.GetComponent<TimeJump> ().doPause ();
				}

		}

		//Pauses or unpauses the game
		public void Paws ()
		{
				//Debug.Log ("I GOT PAWS");
				//Pauses the game //reduendant after Reuben's code
				//doPaws = !doPaws;
				//toggles the play and pause pics on the button
				pic += 1;
				pic %= 2;
		}

}
