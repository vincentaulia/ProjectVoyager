/* Created by Jihad El Sheikh
 * 
 * This script initializes the diameters of all the objects
 * It is attached to the bary center
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	basic_info.txt
 * 
 */

using UnityEngine;
using System.Collections;

public class InitObjects : MonoBehaviour {

	public GameObject[] planets;
	System.IO.StreamReader basic = new System.IO.StreamReader ("basic_info.txt");

	// Use this for initialization
	void Awake () {

		//make sure the position and rotation of the bary center are set to zero
		this.transform.position = Vector3.zero;
		this.transform.eulerAngles = Vector3.zero;

		int num = 10;	//number of objects (will need to read it from the file later on
						//or read until EOF
		planets = new GameObject[num];
		float diameter;
		string id;
		string[] line;
		float scale = 1e5F;		//scale of all measurements. Is applied to to km

		//read the id and radii from the file
		//set the scale of the object
		for (int i=0; i<num; i++) {

			line = basic.ReadLine ().Split ();

			id = line[0];
			planets [i] = GameObject.Find (id);	//hold the pointer to the planet

			diameter = float.Parse (line [3]) / scale;	//scale down the radius
			diameter *= 2;								//convert to diamter
			planets [i].transform.localScale = new Vector3 (diameter, diameter, diameter);
			
		}
	
	}
}
