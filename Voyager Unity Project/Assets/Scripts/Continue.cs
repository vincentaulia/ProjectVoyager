//Created by Sarang Nerkar
using UnityEngine;
using System.Collections;

public class Continue : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				//Loads the game from the introduction screen
				if (Input.GetKeyDown (KeyCode.Space))
						Application.LoadLevel (1);
		}
}
