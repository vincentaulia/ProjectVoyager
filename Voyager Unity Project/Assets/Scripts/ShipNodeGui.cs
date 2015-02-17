using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class ShipNodeGui : MonoBehaviour {
	
	//public GUISkin MenuSkin;
	
	bool toggleTxt;


	string[] selStrings = {"Grid 1", "Grid 2", "Grid 3", "Grid 4"};

	float hSbarValue;
	public string a1 = "0";
	public string a2 = "0";
	public string a3 = "0";

	public string b1 = "0";
	public string b2 = "0";
	public string b3 = "0";

	public string c1 = "0";
	public string c2 = "0";
	public string c3 = "0";

	private int open = 1;

	
	void OnGUI() {
		//GUI.skin = MenuSkin;

		if (open == -1) {
						GUI.BeginGroup (new Rect (Screen.width / 2 + 150, Screen.height / 2, 300, 215));
						GUI.Box (new Rect (0, 0, 300, 300), "Delta V Node");
						GUI.Label (new Rect (0, 20, 100, 20), "Normal ( 0, 0, Normal)");
						a1 = GUI.TextField (new Rect (30, 40, 80, 20), a1, 8);
						a1 = Regex.Replace (a1, "[^.0-9]", "");
						a2 = GUI.TextField (new Rect (120, 40, 80, 20), a2, 8);
						a2 = Regex.Replace (a2, "[^.0-9]", "");
						a3 = GUI.TextField (new Rect (210, 40, 80, 20), a3, 8);
						a3 = Regex.Replace (a3, "[^.0-9]", "");

			GUI.Label (new Rect (0, 70, 100, 20), "Tangent  ( 0, tangential, 0)");
						b1 = GUI.TextField (new Rect (30, 100, 80, 20), b1, 8);
						b1 = Regex.Replace (b1, "[^.0-9]", "");
						b2 = GUI.TextField (new Rect (120, 100, 80, 20), b2, 8);
						b2 = Regex.Replace (b2, "[^.0-9]", "");
						b3 = GUI.TextField (new Rect (210, 100, 80, 20), b3, 8);
						b3 = Regex.Replace (b3, "[^.0-9]", "");

			GUI.Label (new Rect (0, 130, 100, 20), "Radial  ( radial, 0, 0)");
						c1 = GUI.TextField (new Rect (30, 150, 80, 20), c1, 8);
						c1 = Regex.Replace (c1, "[^.0-9]", "");
						c2 = GUI.TextField (new Rect (120, 150, 80, 20), c2, 8);
						c2 = Regex.Replace (c2, "[^.0-9]", "");
						c3 = GUI.TextField (new Rect (210, 150, 80, 20), c3, 8);
						c3 = Regex.Replace (c3, "[^.0-9]", "");
			if (GUI.Button(new Rect(120, 180, 100, 20),"Add New Node"))
			{
				Vector3 a = new Vector3 (float.Parse(a1), float.Parse(a2), float.Parse(a3));
				Vector3 b = new Vector3 (float.Parse(b1), float.Parse(b2), float.Parse(b3));
				Vector3 c = new Vector3 (float.Parse(c1), float.Parse(c2), float.Parse(c3));
				Debug.Log ("Input GUI");
				Debug.Log (a.ToString("F4"));
				Debug.Log (b.ToString("F4"));
				Debug.Log (c.ToString("F4"));
				this.gameObject.GetComponent<shipOEHistory> ().deltavChange(Global.time, a, b, c) ;
				open *= -1;
			}

			GUI.EndGroup ();
				} else {
			if (GUI.Button(new Rect(Screen.width / 2 + 300, Screen.height / 2, 120, 20),"Add DV Node"))
				open *= -1;
				}
	}}
