/* Created by Zachary Fejes, May 7, 2015
 * t
 * This script houses a number of functions that can be utilized to assess the mission parameters and updated ship information.
 * 
 * */



using UnityEngine;
using System.Collections;

public class ShipMissionFunctions : MonoBehaviour {

	public float deltaVBudget = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Determines the amount of DeltaV required in total, and returns that value as a scalar float value in [m/s]. 
	//If deltaV_budgt_usage return a -1, there has been an error.
	public void  update_deltaV_budget(double newDeltaV){
		deltaVBudget += (float)newDeltaV;
		return;
	}

/*
	public float fuel_budget_usage_percent(){
		float percentUsed = 0;


		return percentUsed;
	}
*/
}
