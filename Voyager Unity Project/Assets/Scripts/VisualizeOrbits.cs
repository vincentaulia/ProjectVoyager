using UnityEngine;
using System.Collections;

public class VisualizeOrbits : MonoBehaviour {

	bool showControls = false;		//controls showing the different windows
	public Rect promptWindow;		//reference the prompt window
	public Rect orbitsWindow;		//reference the window with checkboxes

	public static bool planetOrbits = true;
	public static bool moonOrbits = true;
	public static bool asteroidOrbits = true;
    public static bool cometOrbits = true;
	public static bool shipOrbits = true;

	public static bool auto = true;			//toggles control butween user and interface

	void OnGUI(){

		//alternate between small and big window as the button is pressed
		if (showControls) {
			orbitsWindow = GUI.Window(5, orbitsWindow, orbitsFunc, "Visualizing tracks");
				} else {
			promptWindow = GUI.Window(6, promptWindow, promptFunc, "Visualizing tracks");
				}
	}

	void promptFunc(int windowID){
		//show the orbits window
		if (GUI.Button (new Rect (10, 20, 100, 25), "Show")) {
			showControls = true;
		}
		GUI.DragWindow ();
	}

	void orbitsFunc(int windowID){
		int x = 10;

		auto = GUILayout.Toggle (auto, "Auto");

		//if auto is selected, disable the other options
		GUI.enabled = !auto;

		planetOrbits = GUILayout.Toggle (planetOrbits, "Planets");
		moonOrbits = GUILayout.Toggle (moonOrbits, "Moons");
		asteroidOrbits = GUILayout.Toggle (asteroidOrbits, "Asteroids");
        cometOrbits = GUILayout.Toggle(cometOrbits, "Comets");
		shipOrbits = GUILayout.Toggle (shipOrbits, "Ships");

		//enables the button
		GUI.enabled = true;

		if (GUILayout.Button("Hide")) {
			showControls = false;
				}
		GUI.DragWindow ();

		}

	// Use this for initialization
	void Start () {
		//initialize the windows
		promptWindow = new Rect (Screen.width - 140, 40, 130, 50);
		orbitsWindow = new Rect (Screen.width - 200, 40, 160, 180);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}