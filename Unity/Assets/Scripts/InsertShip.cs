/* Created by Jihad El Sheikh
 * 
 * This file creates a ship from a prefab when the user clicks a button
 * 
 * Attached to: ForShip
 * 
 * Files needed:	None
 * 
 */

using UnityEngine;
using System.Collections;

public class InsertShip : MonoBehaviour {
	public GameObject spaceShip;

	// Use this for initialization
	void Start () {
	
	}

	void OnGUI(){
		if (GUI.Button (new Rect (10, 75, 100, 25), "Add ship")) {
			createShip ();
				}
	}


	void createShip(){
		int i = Global.ship.Count;
		//create a ship object
		Global.ship.Add ((GameObject)Instantiate (spaceShip));
		//name the object
		Global.ship [i].name = "Ship" + (i+1).ToString();
		//calculate the orbital elements for it
		Global.ship [i].GetComponent<OrbitalElements> ().getElements ();
		float size = 0.005f;
		Global.ship [i].transform.localScale = new Vector3 (size, size, size);
		}

	// Update is called once per frame
	void Update () {
	
	}
}
