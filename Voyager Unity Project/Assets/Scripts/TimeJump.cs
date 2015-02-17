﻿/*
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
 * 
 * Nov 6 2014 - Added time jump
 * Nov 7 2014 - Added fast forward scaling & pausing
 *
 * - Modified by Rashad Ajward
 * - Updated Feb 7 2015
 * - Added options on the slider, moved slider, added display to say what the slider is displaying
 * - Added a constant, universal time display
 * - Removed fastforwarding speeding up, seems redundant to have both
 * - Going backwards in time may need to be added
 */
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Globalization;

public class TimeJump : MonoBehaviour
{
		// Timeskip variables
		private string strTime = "";
		private string strJumpToTime = "";
		private string strPause = "Play";
		private string strDisplay = " ";
		private int outnum = 0;
		private int savedTimeScale;
		private float sliderValue = 0.0F;
		//private int stepSize = 24*3600;

		void guiTimeSlider ()
		{
				/*
		 * Changes between days and years
		 * 0.0 - 1 Second
		 * 2.0 - 1 Minute
		 * 4.0 - 1 Hour
		 * 6.0 - 1 Day
		 * 8.0 - 7 Days
		 * 10.0 - 30 Days
		 * 12.0 - 120 Days
		 * 14.0 - 180 Days
		 * 16.0 - Years
		 * Add more values if you need decades/centuries/etc.
		 */
				//GUI.contentColor = Color.yellow;
				GUI.backgroundColor = Color.red;
				sliderValue = Mathf.RoundToInt (GUI.HorizontalSlider (new Rect (Screen.width - 285,/*Screen.height-600*/30, 100, 50), sliderValue, 0.0F, 16.0F));
				GUI.backgroundColor = Color.grey;
				//Debug.Log("sliderValue = " + sliderValue);
		}

		void guiTimeJump ()
		{
		
				// Increase the last parameter if you want to input larger numbers
				strJumpToTime = GUI.TextField (new Rect (Screen.width - 50 * 1 - 5 * 1, 5, 50, 20), strJumpToTime, 5);
		
				if (GUI.Button (new Rect (Screen.width - 50 * 2 - 5 * 2, 5, 50, 20), "Jump")) {
						bool res = int.TryParse (strJumpToTime, out outnum);
						if (res) {
								Debug.Log ("outnum = " + outnum);
								//time = outnum*24*3600; // try doing this in fixedupdate
								//strTime = outnum.ToString(); // try doing this in fixedupdate
								if (outnum <= 0)
										outnum = 1;
								switch ((int)sliderValue) {
								case 0:
					// We're moving in seconds
										Global.time = outnum;
										break;
								case 2:
					// We're moving in minutes
										Global.time = outnum * 60;
										break;
								case 4:
					// We're moving in hours
										Global.time = outnum * 3600;
										break;
								case 6:
					// We're moving in days
										Global.time = outnum * 24 * 3600;
										break;
								case 8:
					// We're moving in 7 days
										Global.time = outnum * 24 * 3600 * 7;
										break;
								case 10:
					// We're moving in 30 days
										Global.time = outnum * 24 * 3600 * 30;
										break;
								case 12:
					// We're moving in 120 days
										Global.time = outnum * 24 * 3600 * 120;
										break;
								case 14:
					// We're moving in 180 days
										Global.time = outnum * 24 * 3600 * 180;
										break;
								case 16:
					// We're moving in years
										Global.time = outnum * 365 * 24 * 3600;
										break;
								default:
										break;
								}
								Global.time = outnum;
								strJumpToTime = "";
						} else {
								//doJump = false;
						}
				} else {


						//Slider controls - add more options if you decide to
						//add decades/centuries/etc

						switch ((int)sliderValue) {
						case 0:
								strDisplay = "Seconds";
								Global.time_stepsize = 1;
								break;
						case 2:
								strDisplay = "Minutes";
								Global.time_stepsize = 60;
								break;
						case 4:
								strDisplay = "Hours";
								Global.time_stepsize = 3600;
								break;
						case 6:
								strDisplay = "Days";
								Global.time_stepsize = 24 * 3600;
								break;
						case 8:
								strDisplay = "Weeks";
								Global.time_stepsize = 24 * 3600 * 7;
								break;
						case 10:
								strDisplay = "Months";
								Global.time_stepsize = 24 * 3600 * 30;
								break;
						case 12:
								strDisplay = "4 Months";
								Global.time_stepsize = 24 * 3600 * 120;
								break;
						case 14:
								strDisplay = "6 Months";
								Global.time_stepsize = 24 * 3600 * 180;
								break;
						case 16:
								strDisplay = "Years";
								Global.time_stepsize = 365 * 24 * 3600;
								break;
						}

						strTime = "Time Slider: " + strDisplay;
			
						// how much time has passed since the beginning
						// (Global.time/(Global.time_stepsize)).ToString();
				}
				GUI.Label (new Rect (Screen.width - 300, 5, 150, 20), strTime);

				/////////////////////////////////////////////////////////////////////////////////////////////////////
				//This code below shows the time at the top right of the display
				//it is shown in terms of seconds, minutes, days, and years
				//code is reused from ErrorCalc.cs
				//this part updated by Omar Khan on Fri Jan 23 6:45PM 

				string[] units = {"s", "min", "hr", "d", "yr"};
				long[] divisor = {60, 60, 24, 365};
				long time;
				long remainder;
				string breakDown = "";
				string localTime;
		
				//get the current time
				time = Global.time;

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
				GUI.Label (new Rect (Screen.width - 500, 5, 150, 20), localTime);

				/////////////////////////////////////////////////////////////////////////////////////////////////////
		}

		public void doPause ()
		{
				if (Global.time_doPause) {
						Debug.Log ("RESUMED!");
						strPause = "Pause";
				} else {
						Debug.Log ("PAUSED!");
						strPause = "Play";
				}
				Global.time_doPause = !Global.time_doPause;
		
				//toggle play/pause picture button
				this.GetComponent<MovePca> ().Paws ();
		}
	
		void OnGUI ()
		{
				// Currently, you may only jump to 0 < t <= 999

				guiTimeSlider ();
				guiTimeJump ();
		}
}