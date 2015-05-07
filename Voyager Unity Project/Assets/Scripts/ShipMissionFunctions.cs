/* Created by Zachary Fejes, May 7, 2015
 * 
 * This script houses a number of functions that can be utilized to assess the mission parameters and updated ship information.
 * 
 * */



using UnityEngine;
using System.Collections;

public class ShipMissionFunctions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Determines the amount of DeltaV used in total, and returns that value as a percentage or a number in m/s. 
	//The 'output' variable specifies the output type requested. output = 1 is [m/s], output = 2 is [%]
	//If deltaV_budgt_usage return a -1, there has been an error.
/*	public float deltaV_budget_usage(int output){
		if (output <= 0 || output >= 3) {
			Debug.Log ("unknown output type requested. Value outside of acceppted range.");
			return -1;
		}

		float deltaVUsed = 0; 
		float deltaV_budget = calculate delta V budget here
		float deltaV_list[] = delta_v_list;

		for (int i = 0; delta_v_list.length, i++) {
			deltaVUsed += delta_v_list(i).t + delta_v_list(i).r + delta_v_list(i).n;
		}	

		if (output == 1) 
			return deltaVUsed;
		else if (output == 2)
			return 100*(deltaVUsed/deltaV_budget);
		else
			return -1;
	}


	public float fuel_budget_usage_percent(){
		float percentUsed = 0;


		return percentUsed;
	}
	*/
}
