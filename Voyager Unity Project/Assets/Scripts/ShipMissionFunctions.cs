/* Created by Zachary Fejes, May 7, 2015
 * t
 * This script houses a number of functions that can be utilized to assess the mission parameters and updated ship information.
 * 
 * */



using UnityEngine;
using System.Collections;
using System;

public class ShipMissionFunctions : MonoBehaviour
{

    public float deltaVBudget = 0;
    float G = 6.67384e-11f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //returns a Vector3 object which contains the velocity of the target object w.r.t. it's parent object
    //Input arguments is the eccentric anomaly E in radians
    public Vector3 calc_current_velocity(float E)
    {
        Elements CurrentOE = GetComponent<shipOEHistory>().currentOE(Global.time);
        float Mu = G * (float)(CurrentOE.mass + CurrentOE.massFocus);
        float a = (float)CurrentOE.axis;
        float e = (float)CurrentOE.ecc;
        Vector3 P = CurrentOE.P;
        Vector3 Q = CurrentOE.Q;

        Vector3 V = (Mathf.Sqrt(Mu / a)) * ((-e * Mathf.Sin(E) * P) + (Mathf.Sqrt(1 - Mathf.Pow(e, 2))) * Mathf.Cos(E) * Q) / (1 - e * Mathf.Cos(E));

        return V;
    }
	

	//returns a double array which contains the velocity of the target object w.r.t. it's parent object
	//Input arguments is the eccentric anomaly E in radians
	public double[] calc_current_velocity_double(double E)
	{
		Elements CurrentOE = GetComponent<shipOEHistory>().currentOE(Global.time);
		double Mu = G * (CurrentOE.mass + CurrentOE.massFocus);
		double a = CurrentOE.axis;
		double e = CurrentOE.ecc;
		double[] P = CurrentOE.P_Vector;
		double[] Q = CurrentOE.Q_Vector;

		double[] V = new double[3];
		V[0] = (Math.Sqrt(Mu / a)) * ((-e * Math.Sin(E) * P[0]) + (Math.Sqrt(1 - Math.Pow(e, 2))) * Math.Cos(E) * Q[0]) / (1 - e * Math.Cos(E));
		V[1] = (Math.Sqrt(Mu / a)) * ((-e * Math.Sin(E) * P[1]) + (Math.Sqrt(1 - Math.Pow(e, 2))) * Math.Cos(E) * Q[1]) / (1 - e * Math.Cos(E));
		V[2] = (Math.Sqrt(Mu / a)) * ((-e * Math.Sin(E) * P[2]) + (Math.Sqrt(1 - Math.Pow(e, 2))) * Math.Cos(E) * Q[2]) / (1 - e * Math.Cos(E));
		
		return V;
	}


    //Determines the amount of DeltaV required in total, and returns that value as a scalar float value in [m/s]. 
    //If deltaV_budgt_usage return a -1, there has been an error.
    public void update_deltaV_budget(double newDeltaV)
    {
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
