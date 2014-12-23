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

public class MovePca : MonoBehaviour {

	bool doPaws = true;		//controls pausing the game

	// Use this for initialization
	void Start () {
		Debug.Log ("count: " + Global.body.Count);

		//sets the starting positions of all bodies except for the sun
		for (int i=1; i<Global.body.Count; i++) {

			Global.body[i].transform.position = PcaPosition.findPos (Global.body[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.body[i]);
				}

		//for the ships
		for (int i=0; i<Global.ship.Count; i++) {
			Global.ship[i].transform.position = PcaPosition.findPos (Global.ship[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.ship[i]);

		}
	}
	
	// Update is called once per frame
	void Update () {

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
				float distance;
				distance = Vector3.Distance(GameObject.Find ("399").transform.position, Global.ship[0].transform.position);
				distance = distance * 1e5f;
			}
		}


	}

	//Pauses or unpauses the game
	void Paws(){
		Debug.Log ("I GOT PAWS");
		doPaws = !doPaws;
	}
}
