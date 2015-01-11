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

		bool doPaws = true;		//controls pausing the game
		public Rect button;		//holds the dimensions of the button
		int pic = 1;			//holds whether it's a pause or play
		public Texture[] pawsPic = new Texture[2];	//element 0 is play. element 1 is pause
		GameObject bary;

		// Use this for initialization
		void Start ()
		{
				Debug.Log ("count: " + Global.body.Count);

				//sets the starting positions of all bodies except for the sun
				for (int i=1; i<Global.body.Count; i++) {

						Global.body [i].transform.position = PcaPosition.findPos (Global.body [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.body [i]);
				}

				//for the ships
				for (int i=0; i<Global.ship.Count; i++) {
						Global.ship [i].transform.position = PcaPosition.findPos (Global.ship [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.ship [i]);

				}
				button = new Rect (10, 170, 60, 60);

				Time.timeScale = 0.125f;

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
						Global.time += Global.time_multiplier * 1;
						//move the planets and moons
						//starts from 1 to skip the sun
						for (int i=1; i<Global.body.Count; i++) {
								Global.body [i].transform.position = PcaPosition.findPos (Global.body [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.body [i]);
						}
			
						//move the ships
						for (int i=0; i<Global.ship.Count; i++) {
				
								Global.ship [i].transform.position = PcaPosition.findPos (Global.ship [i].GetComponent<OrbitalElements> ().orb_elements, Global.time, Global.ship [i]);
						}
				}
		
		
	

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
