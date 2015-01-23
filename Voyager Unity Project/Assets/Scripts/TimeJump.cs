/*
 * TimeJump.cs:
 * - Created by Reuben Kumar
 * - Latest major update: Jan 23 2015 6:45PM
 * - Still need to test with all planets & moons... not sure about performance.
 * 
 * This script handles anything related to time jumping,
 * such as fast-forwarding, rewinding & timeskip stuff.
 * This includes all UI objects related to it.
 * 
 * For now, I'm not doing a rewind feature since it's pretty easy
 * for Global.time to take on negative values.
 * 
 * Nov 06 2014 - Added time jump
 * Nov 07 2014 - Added fast forward scaling & pausing
 * Jan 23 2015 - Changed the format in which time is displayed on top-right of screen
 */

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;



public class TimeJump : MonoBehaviour {
	// Timeskip variables
	private string strTime = "";
	private string strJumpToTime = "";
	private string strPause = "Play";
	private string strDisplay = " ";
	private int outnum = 0;
	private int savedTimeScale;
	private float sliderValue = 0.0F;
	//private int stepSize = 24*3600;

	void guiTimeSlider() {
		/*
		 * Changes between days and years
		 * 1.0 - Days
		 * 2.0 - Years
		 * Add more values if you need decades/centuries/etc.
		 */
		//GUI.contentColor = Color.yellow;
		sliderValue = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(Screen.width-200,Screen.height-50,100,50),sliderValue,1.0F,2.0F));
		//Debug.Log("sliderValue = " + sliderValue);
	}


	void guiTimeJump() {
		
		// Increase the last parameter if you want to input larger numbers
		strJumpToTime = GUI.TextField(new Rect(Screen.width - 50*1-5*1,5,50,20),strJumpToTime,5);
		
		if (GUI.Button(new Rect(Screen.width-50*2-5*2,5,50,20),"Jump"))
		{
			bool res = int.TryParse(strJumpToTime,out outnum);
			if (res)
			{
				Debug.Log ("outnum = "+outnum);
				//time = outnum*24*3600; // try doing this in fixedupdate
				//strTime = outnum.ToString(); // try doing this in fixedupdate
				if (outnum <= 0)
					outnum = 1;
				switch ((int)sliderValue) {
				case 1:
					// We're moving in days
					Global.time = outnum * 24*3600;
					break;
				case 2:
					// We're moving in years
					Global.time = outnum * 365*24*3600;
					break;
				default:
					break;
				}
				Global.time = outnum;
				strJumpToTime = "";
			}
			else
			{
				//doJump = false;
			}
		}
		else
		{
			//strTime = (time/(24*3600)).ToString(); // try doing this in fixedupdate
			//doJump = false;
			
			/*
			 * Slider controls - add more options if you decide to
			 * add decades/centuries/etc
			 */
			switch ((int)sliderValue) {
			case 1: 
				strDisplay = "Day ";
				Global.time_stepsize = 24*3600;
				break;
			case 2:
				strDisplay = "Year ";
				Global.time_stepsize = 365*24*3600;
				break;
			}

			strTime = strDisplay + (Global.time/(Global.time_stepsize)).ToString();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		/* This code below shows the time at the top right of the display
		 * it is shown in terms of seconds, minutes, days, and years
		 * code is reused from ErrorCalc.cs
		 * this part updated by Omar Khan on Fri Jan 23 6:45PM */

		string[] units = {"s", "min", "hr", "d", "yr"};
		long[] divisor = {60, 60, 24, 365};
		long time;
		long remainder;
		string breakDown = "";
		string localTime;
		
		//get the current time
		time = Global.time;
		
		//localTime = time.ToString () + "s: ";
		localTime = "";
		
		//divide it into seconds, minutes, hours, days and years
		for (int i = 0; i<4 && time != 0; i++) {
			
			remainder = time % divisor [i];
			breakDown += remainder.ToString () + units [i] + " ";
			time -= remainder;
			time /= divisor [i];
		}

		//if there's still time left, then put it in years
		if (time != 0) {
			breakDown += time.ToString () + units [4];
		}

		//store as string into localTime
		localTime += breakDown;

		//update localTime onto GUI
		GUI.Label(new Rect(Screen.width - 100*4-5*3,5,150,20), localTime);

		/////////////////////////////////////////////////////////////////////////////////////////////////////
	}
	
	void guiTimeScale() {
		
		
		
		if (GUI.Button(new Rect(Screen.width-50*1-5*1,20+5*2,50,20),">>"))
		{
			if (Global.time_multiplier < 5)
				Global.time_multiplier += 1; //1
			
			Debug.Log("Sped up! TimeScale = " + (Global.time_multiplier).ToString());
		}
		
		if (GUI.Button(new Rect(Screen.width-50*2-5*2,20+5*2,50,20),strPause))
		{/*
			if (Global.time_doPause) {
				Debug.Log("RESUMED!");
				strPause = "Pause";
			}
			else {
				Debug.Log("PAUSED!");
				strPause = "Play";
			}
			Global.time_doPause = !Global.time_doPause;*/
			doPause ();
		}
		
		if (GUI.Button(new Rect(Screen.width-50*3-5*3,20+5*2,50,20),"<<"))
		{
			if (Global.time_multiplier > 1)
				Global.time_multiplier -= 1; //1
			
			Debug.Log("Slowed down! TimeScale = " + (Global.time_multiplier).ToString());
		}
		
		string strPlaybackLabel = "Playback at x" + Global.time_multiplier.ToString() + " speed";
		GUI.Label(new Rect(Screen.width - 50*3-5*1,20*2+5*3,150,20), strPlaybackLabel);
		
		
	}
	
	public void doPause(){
		if (Global.time_doPause) {
			Debug.Log("RESUMED!");
			strPause = "Pause";
		}
		else {
			Debug.Log("PAUSED!");
			strPause = "Play";
		}
		Global.time_doPause = !Global.time_doPause;
		
		//toggle play/pause picture button
		this.GetComponent<MovePca> ().Paws ();
	}
	
	void OnGUI() {
		// Currently, you may only jump to 0 < t <= 999
		
		//GUI.Box(new Rect(Screen.width-205, Screen.height-105, 200, 100), "Just a box.");
		guiTimeSlider();
		guiTimeJump();
		guiTimeScale();
	}
	
	/*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		
	}
	*/
}