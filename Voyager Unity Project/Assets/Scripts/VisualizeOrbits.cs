using UnityEngine;
using System.Collections;

public class VisualizeOrbits : MonoBehaviour {

	bool showControls = false;		//controls showing the different windows
	public Rect promptWindow;		//reference the prompt window
	public Rect orbitsWindow;		//reference the window with checkboxes

    //auto toggle buttons
	public static bool a_planetOrbits = true;
	public static bool a_moonOrbits = true;
	public static bool a_asteroidOrbits = false;
    public static bool a_cometOrbits = false;
	public static bool a_shipOrbits = true;

    //manual toggle buttons
    public static bool m_planetOrbits = true;
    public static bool m_moonOrbits = true;
    public static bool m_asteroidOrbits = true;
    public static bool m_cometOrbits = true;
    public static bool m_shipOrbits = true;

	public static bool auto = true;			//toggles control for auto
    private bool manual = false;            //toggles control for manual

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
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        
		auto = GUILayout.Toggle (auto, "Auto");
        manual = !auto;
        

		//if auto is selected, disable the other options
		GUI.enabled = auto;

		a_planetOrbits = GUILayout.Toggle (a_planetOrbits, "Planets");
		a_moonOrbits = GUILayout.Toggle (a_moonOrbits, "Moons");
		a_asteroidOrbits = GUILayout.Toggle (a_asteroidOrbits, "Asteroids");
        a_cometOrbits = GUILayout.Toggle(a_cometOrbits, "Comets");
		a_shipOrbits = GUILayout.Toggle (a_shipOrbits, "Ships");

		//enables the button
		GUI.enabled = true;

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        manual = GUILayout.Toggle(manual, "Manual");
        auto = !manual;

        //if auto is selected, disable the other options
        GUI.enabled = !auto;

        m_planetOrbits = GUILayout.Toggle(m_planetOrbits, "Planets");
        m_moonOrbits = GUILayout.Toggle(m_moonOrbits, "Moons");
        m_asteroidOrbits = GUILayout.Toggle(m_asteroidOrbits, "Asteroids");
        m_cometOrbits = GUILayout.Toggle(m_cometOrbits, "Comets");
        m_shipOrbits = GUILayout.Toggle(m_shipOrbits, "Ships");

        //enables the button
        GUI.enabled = true;

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

		if (GUILayout.Button("Hide")) {
			showControls = false;
				}
        GUILayout.EndVertical();
		GUI.DragWindow ();

		}

	// Use this for initialization
	void Start () {
		//initialize the windows
		promptWindow = new Rect (Screen.width - 140, 40, 130, 50);
		//orbitsWindow = new Rect (Screen.width - 200, 40, 160, 180);
        orbitsWindow = new Rect(Screen.width - 300, 40, 280, 180);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}