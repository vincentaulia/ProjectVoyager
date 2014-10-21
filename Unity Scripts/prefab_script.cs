/*
 * Created by Jihad El Sheikh
 * 
 * Move the planets according to positions read from files.
 * 
 * Attached to: Bary Center
 * 
 * Files needed:	tenyear.txt
 * 					basic_info.txt
 * 
 */

using UnityEngine;
using System.Collections;

public class prefab_script : MonoBehaviour
{
	
	public GameObject[] planets;
	System.IO.StreamReader file = new System.IO.StreamReader ("tenyear.txt");
	System.IO.StreamReader basic = new System.IO.StreamReader ("basic_info.txt");
	
	string[] line;
	float scale = 1e5F;					//the scale for measurements. Applies to km
	static int num = 10;				//number of objects. Latet should be read from a file
	bool doPause = false;

	//call this method every frame to move the planets one day
	void moveBody() {

		//read the next 10 lines to get positions of planets
		for (int i=0; i<num && !file.EndOfStream; i++) {
			line = file.ReadLine ().Split ();

			//switch the y and z coordinates so the planets rotate in the horizontal plane of the game
			float[] pos = {
				float.Parse (line [1]) / scale,
				float.Parse (line [3]) / scale,
				float.Parse (line [2]) / scale
			};
			
			Vector3 temp = new Vector3 (pos [0], pos [1], pos [2]);
			planets [i].transform.position = temp;

			
		}
		
		if (file.EndOfStream) {
			Debug.Log ("Done");
		}
	}

	//set the start positions of the objects
	void initBodies() {
		planets = new GameObject[num];
		float diameter;
		string id;

		//read ids of planets and radii
		for (int i=0; i<num; i++) {
			line = basic.ReadLine ().Split ();

			id = line[0];
			planets [i] = GameObject.Find (id);

			diameter = float.Parse (line [3]) / scale;	//scale down the radius
			diameter *= 2;								//because scale is the diamter
			planets [i].transform.localScale = new Vector3 (diameter, diameter, diameter);
			
		}

		//read the initial positions (first 'num' lines in the file)
		for (int i=0; i<num; i++) {
			line = file.ReadLine ().Split ();

			//switch the y and z coordinates so the planets rotate in the horizontal plane of the game
			float[] pos = {
				float.Parse (line [1]) / scale,
				float.Parse (line [3]) / scale,
				float.Parse (line [2]) / scale
			};

			Vector3 temp = new Vector3 (pos[0], pos[1], pos[2]);
			planets[i].transform.position = temp;
			
		}
		
	}
	
	// Use this for initialization
	void Start ()
	{
		//to make sure Bary Center is set correctly
		this.transform.position = Vector3.zero;
		this.transform.eulerAngles = Vector3.zero;

		initBodies();
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//toggle pause if P button is pressed
		if (Input.GetKeyDown(KeyCode.P)) {
			doPause = !doPause;			
			Debug.Log("doPause = " + doPause);
		}

		//only move the planets if the Pause button is not pressed
		if (!doPause){
			moveBody();
		}
		
		
		
	}
}
