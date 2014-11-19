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
		Instantiate (spaceShip);

		}

	// Update is called once per frame
	void Update () {
	
	}
}
