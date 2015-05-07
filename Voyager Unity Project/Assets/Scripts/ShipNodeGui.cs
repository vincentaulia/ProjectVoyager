
using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class ShipNodeGui : MonoBehaviour 
{
	
	//public GUISkin MenuSkin;
	
	bool toggleTxt;


	string[] selStrings = {"Grid 1", "Grid 2", "Grid 3", "Grid 4"};

	float hSbarValue;
	public string a1 = "0";
	public string a2 = "0";
	public string a3 = "0";
	public string a4 = "0";

	private int open = 1;

	
	void OnGUI() {
		//GUI.skin = MenuSkin;

		if (open == -1) 
		{
			GUI.BeginGroup (new Rect (Screen.width - 130 , Screen.height - 310 , 120, 300));
			GUI.Box (new Rect (0, 0, 120, 300), "Maneuver Node");
/*			GUI.Label (new Rect (10, 30, 100, 20), "Normal");
			a1 = GUI.TextField (new Rect (10, 60, 80, 20), a1, 8);
			a1 = Regex.Replace (a1, "[^.0-9]", "");
			GUI.Label (new Rect (10, 90, 100, 20), "Tangential");
			a2 = GUI.TextField (new Rect (10, 120, 80, 20), a2, 8);
			a2 = Regex.Replace (a2, "[^.0-9]", "");
			GUI.Label (new Rect (10, 150, 100, 20), "Radial");
			a3 = GUI.TextField (new Rect (10, 180, 80, 20), a3, 8);
			a3 = Regex.Replace (a3, "[^.0-9]", ""); */

			GUI.Label (new Rect (10, 210, 100, 20), "Mean Anomalyr");
			a4 = GUI.TextField (new Rect (10, 240, 80, 20), a4, 8);
			a4 = Regex.Replace (a4, "[^.0-9]", "");

			if (GUI.Button(new Rect(10, 270, 100, 20),"Add New Node"))
			{
			//	(float.Parse (a4))*Mathf.PI/180  // ths is the radian version of the degree entry in the field
				open *= -1;
				this.gameObject.GetComponent<shipOEHistory> ().deltaVAdd((double.Parse (a4))*Mathf.PI/180);

			/*	Vector3 a = new Vector3 (0, 0, float.Parse(a1)); //normal
				Vector3 b = new Vector3 (0, float.Parse(a2), 0); //tangential
				Vector3 c = new Vector3 (float.Parse(a3), 0, 0); //radial
				Debug.Log ("Input GUI");
				Debug.Log (a.ToString("F4"));
				Debug.Log (b.ToString("F4"));
				Debug.Log (c.ToString("F4"));
				this.gameObject.GetComponent<shipOEHistory> ().deltavChange(long.Parse (a4), a, b, c) ; */

			}
			GUI.EndGroup ();
		} 
		else 
		{
			if (GUI.Button(new Rect(Screen.width  - 160, Screen.height - 80, 120, 40),"Add DV Node"))
			{
				a4 = Global.time.ToString();
				open *= -1;
			}
		}
	}
}
