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
			if (i>83){
				Debug.Log (Global.body[i].name);
				//Debug.Log (Global.body[83].transform.position);
				/*
				Debug.Log ("anom: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.anom);
				Debug.Log ("arg: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.arg);
				Debug.Log ("asc: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.asc);
				Debug.Log ("axis: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.axis);
				Debug.Log("ecc: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.ecc);
				Debug.Log("incl: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.incl);
				Debug.Log("mass: " + Global.body[83].GetComponent<OrbitalElements>().orb_elements.mass);
				*/
			}
			Global.body[i].transform.position = PcaPosition.findPos (Global.body[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.body[i]);
				}

		//for the ships
		for (int i=0; i<Global.ship.Count; i++) {
			Global.ship[i].transform.position = PcaPosition.findPos (Global.ship[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.ship[i]);

		}
		Debug.Log ("first loop: " + Global.body[83].transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("MovePca: " + Global.body[83].transform.position);

		//if the P button is pressed.. toggle pausing the game
		if (Input.GetKeyDown(KeyCode.P)) {
			doPaws = !doPaws;
		}
		
		//only advance the time if the game is not paused
		if (!doPaws) {
			Debug.Log ("I GOT PAWS");
			Global.time += 360;

			//updates the positions of all bodies except for the sun
			for (int i=1; i<Global.body.Count; i++) {
				Global.body[i].transform.position = PcaPosition.findPos (Global.body[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.body[i]);
			}

			//move the ships
			for (int i=0; i<Global.ship.Count; i++) {

				Global.ship[i].transform.position = PcaPosition.findPos (Global.ship[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.ship[i]);
				/*Vector3 vect;
				vect = PcaPosition.findPos (Global.ship[i].GetComponent<OrbitalElements>().orb_elements, Global.time, Global.ship[i]);
				Global.ship[i].transform.position = vect;
				Debug.Log ("vect: " + vect.ToString());
				*/
				float distance;
				distance = Vector3.Distance(GameObject.Find ("399").transform.position, Global.ship[0].transform.position);
				distance = distance * 1e5f;
				Debug.Log("distace: " + distance);
			}
		}


	}
}
