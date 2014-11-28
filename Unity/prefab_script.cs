using UnityEngine;
using System.Collections;

public class prefab_script : MonoBehaviour
{

		public GameObject[] planets;
		System.IO.StreamReader radius = new System.IO.StreamReader ("mass_rad_id.txt");
		System.IO.StreamReader file = new System.IO.StreamReader ("tenyear.txt");
		System.IO.StreamReader id = new System.IO.StreamReader ("id.txt");
		string[] line;
		float scale = 1e5F;
		static int num = 10;
		public GameObject camera;

		// Use this for initialization
		void Start ()
		{
				planets = new GameObject[num];
				float rad;

				for (int i=0; i<num; i++) {
		
						planets [i] = GameObject.Find (id.ReadLine ());
				}
				
		camera = GameObject.Find ("Main Camera");

				

				for (int i=0; !radius.EndOfStream; i++) {
						line = radius.ReadLine ().Split ();
				rad = float.Parse (line [2]) / scale;
								planets [i].transform.localScale = new Vector3 (rad, rad, rad);
						

				}
	
				for (int i=0; i<num; i++) {
						line = file.ReadLine ().Split ();
						
								float[] pos = {
			float.Parse (line [1]) / scale,
			float.Parse (line [3]) / scale,
			float.Parse (line [2]) / scale
		};
						Vector3 temp = new Vector3 (pos [0], pos [1], pos [2]);
						planets [i].transform.position = temp;
						
				}

	
		}
	
		// Update is called once per frame
		void Update ()
		{
				
	for (int i=0; i<num && !file.EndOfStream; i++) {
						line = file.ReadLine ().Split ();
						
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
		//camera.transform.position = new Vector3 (planets [1].transform.position.x + 2, planets [1].transform.position.y + 2, planets [1].transform.position.z + 2);
		//camera.transform.LookAt (planets [1].transform.position);

	
		}
}
