/*
 * TimeJump.cs:
 * - Created by Reuben Kumar
 * - Latest major update: Nov 6 2014
 * - Still need to test with all planets & moons... not sure about performance.
 * 
 * This script handles anything related to time jumping,
 * such as fast-forwarding, rewinding & timeskip stuff.
 * This includes all UI objects related to it.
 * 
 * For now, I'm not doing a rewind feature since it's pretty easy
 * for Global.time to take on negative values.
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
    private string strDisplay = "Day ";
    private int outnum = 0;
    private int savedTimeScale;
    private float sliderValue = 0.0F;

    bool showJump = false;
    public Rect jumpRect;
    public Rect jumpButton;
    public Rect timeBox;
    string time24htxt = "";
    string daytxt = "";
    string monthtxt = "";
    string yeartxt = "";
    int x = Screen.width - 300;

    int timebox_x = Screen.width - 440;
	//int timebox_x = 0;
    int timebox_y = Screen.height - 230;

    long real_year = 2014;
    bool leap_year = false;
    long day_m = 1;

    void guiTimeSlider()
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
        sliderValue = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(timebox_x + 50, timebox_y + 50, 100, 50), sliderValue, 0.0F, 10.0F));
        GUI.backgroundColor = Color.grey;
        //Debug.Log("sliderValue = " + sliderValue);
    }

    void guiTimeJump()
    {

        // Increase the last parameter if you want to input larger numbers
        strJumpToTime = GUI.TextField(new Rect(Screen.width - 50 * 1 - 5 * 1, 5, 50, 20), strJumpToTime, 5);

        if (GUI.Button(new Rect(Screen.width - 50 * 2 - 5 * 2, 5, 50, 20), "Jump"))
        {
            bool res = int.TryParse(strJumpToTime, out outnum);
            if (res)
            {
                Debug.Log("outnum = " + outnum);
                //time = outnum*24*3600; // try doing this in fixedupdate
                //strTime = outnum.ToString(); // try doing this in fixedupdate
                if (outnum <= 0)
                    outnum = 1;
                switch ((int)sliderValue)
                {
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
                        // We're moving in years
                        Global.time = outnum * 365 * 24 * 3600;
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
    }

    public void timeBoxFunc(int windowID)
    {
        switch ((int)sliderValue)
        {
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
                strDisplay = "Years";
                Global.time_stepsize = 365 * 24 * 3600;
                break;
        }
        strTime = "Time Slider: " + strDisplay;

        // how much time has passed since the beginning
        //(Global.time/(Global.time_stepsize)).ToString();
        //GUI.Label (new Rect (70, 30, 260, 20), "Time (24:00)");
        GUI.Label(new Rect(10, 20, 260, 20), strTime);

        string[] units = { "Seconds: ", "Minutes: ", "Hours: ", "Days: ", "Year: " };
        long[] orig_time_vals = { 0, 0, 0, 0, 0, 0 };
        long[] added_time_vals = { 0, 0, 0, 0, 0, 0 };

        long[] divisor = { 60, 60, 24, 365 }; //change to 366 *
        if (leap_year)
            divisor[3] = 366; //change to 367 *if you make both these changes you can get July 31st to show up (but I think this is wrong)


        long time;
        long remainder;
        string localTime = "";

        //get the current time
        time = Global.time;
        //localTime = time.ToString () + "s: ";
        localTime = "";

        //divide it into seconds, minutes, hours, days and years
        for (int i = 0; i < 4 && time != 0; i++)
        {

            remainder = time % divisor[i];
            added_time_vals[i] = remainder;
            orig_time_vals[i] = remainder;

            time -= remainder;
            time /= divisor[i];
        }
        //if there's still time left, then put it in years
        if (time != 0)
        {
            added_time_vals[4] = time;
            orig_time_vals[4] = time;
        }
        //im assuming start date is friday august 1 2014 at midnight

        //added_time_vals [3] += 212; //days
        added_time_vals[4] += 2014; //years


        int[] days_in_month = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        if (leap_year)
            days_in_month[2] = 29;

        //                        j, f, m, a, m, j, j, a, s, o, n, d
        string[] months = { "Blank", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        int month = 0;
        if (added_time_vals[4] >= 2014)
        {
            month = 8;
            if (added_time_vals[3] > 31)
            {
                while (added_time_vals[3] >= 0)
                {
                    added_time_vals[3] -= days_in_month[month];
                    if (added_time_vals[3] <= 0)
                        break;
                    month++;
                    if (month == 13)
                    {
                        month = 1;
                        added_time_vals[4]++;
                    }
                }
            }
        }
        else
        {
            month = 1;
            added_time_vals[3] -= 153;
            if (added_time_vals[3] > 31)
            {
                while (added_time_vals[3] >= 0)
                {
                    added_time_vals[3] -= days_in_month[month];
                    if (added_time_vals[3] <= 0)
                        break;
                    month++;
                    if (month == 13)
                    {
                        month = 1;
                        added_time_vals[4]++;
                    }
                }
            }
        }
        long leap_check = added_time_vals[4];// + 1;
        if (leap_check % 4 == 0)
        {
            leap_year = true;
            Debug.Log("Leap Year!");
        }
        else
        {
            leap_year = false;
        }

        if (month == 8) { //added_time_vals[4] >= 2014
			//Debug.Log("added_time_vals[3]="+added_time_vals[3]+", days_in_month[month]="+days_in_month[month]+".");
            day_m = added_time_vals[3] + days_in_month[month] - 31;

			if (day_m == 0) {
				day_m = 1;
			}
		}
        else {
            day_m = added_time_vals[3] + days_in_month[month];
		}
        added_time_vals[3] = orig_time_vals[3];



        int j;
        for (j = 0; j <= 4; j++)
        {
            GUI.Label(new Rect(10, 70 + j * 20, 150, 20), units[j]);
            GUI.Label(new Rect(70, 70 + j * 20, 150, 20), added_time_vals[j].ToString());// + " " + units[5]);
        }
        //j++;
        GUI.Label(new Rect(10, 70 + j * 20, 150, 20), "Month: ");
        GUI.Label(new Rect(70, 70 + j * 20, 150, 20), months[month]);
        j++;
        GUI.Label(new Rect(10, 70 + j * 20, 150, 20), "Day M: ");
        GUI.Label(new Rect(70, 70 + j * 20, 150, 20), day_m.ToString());
    }

    public void doPause()
    {
        if (Global.time_doPause)
        {
            Debug.Log("RESUMED!");
            strPause = "Pause";
        }
        else
        {
            Debug.Log("PAUSED!");
            strPause = "Play";
        }
        Global.time_doPause = !Global.time_doPause;

        //toggle play/pause picture button
        this.GetComponent<MovePca>().Paws();
    }

    void OnGUI()
    {
        // Currently, you may only jump to 0 < t <= 999

        //show the big window if the user chooses
        if (showJump)
        {
            jumpRect = GUI.Window(77, jumpRect, jumpFunc, "Time Jump Menu");
        }
        else
        {
            jumpButton = GUI.Window(78, jumpButton, jumpButtonFunc, "Time Jump");
        }

        //GUI.Box(new Rect(Screen.width-205, Screen.height-105, 200, 100), "Just a box.");
        guiTimeSlider();
        timeBox = GUI.Window(79, timeBox, timeBoxFunc, "Time");
        guiTimeJump();
        //guiTimeScale();
    }

    void jumpFunc(int windowID)
    {
        time24htxt = GUI.TextField(new Rect(10, 30, 50, 20), time24htxt);
        GUI.Label(new Rect(70, 30, 260, 20), "Time (24:00)");

        daytxt = GUI.TextField(new Rect(10, 60, 50, 20), daytxt);
        GUI.Label(new Rect(70, 60, 260, 20), "Time (24:00)");

        monthtxt = GUI.TextField(new Rect(10, 90, 50, 20), monthtxt);
        GUI.Label(new Rect(70, 90, 260, 20), "Time (24:00)");

        yeartxt = GUI.TextField(new Rect(10, 120, 50, 20), yeartxt);
        GUI.Label(new Rect(70, 120, 260, 20), "Time (24:00)");

        if (GUI.Button(new Rect(100, 170, 100, 25), "Hide"))
        {
            showJump = false;
        }
        GUI.DragWindow();
    }

    void jumpButtonFunc(int windowID)
    {
        //show the error calc window
        if (GUI.Button(new Rect(10, 20, 100, 25), "Show"))
        {
            showJump = true;
        }
        GUI.DragWindow();
    }

    // Use this for initialization
    void Start()
    {
        jumpRect = new Rect(Screen.width - 300, 400, 290, 200);
        jumpButton = new Rect(Screen.width - 140, 400, 130, 50);
        timeBox = new Rect(timebox_x, timebox_y, 200, 230);
        //GUI.Button(jumpButton = new Rect (Screen.width - 140, 500, 130, 50),"TIME JUMP");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Speed up time code

    /*void guiTimeScale() {
		
		
		
        if (GUI.Button(new Rect(Screen.width-50*1-5*1,20+5*2,50,20),">>"))
        {
            if (Global.time_multiplier < 5)
                Global.time_multiplier += 1; //1
			
            Debug.Log("Sped up! TimeScale = " + (Global.time_multiplier).ToString());
        }
		
        if (GUI.Button(new Rect(Screen.width-50*2-5*2,20+5*2,50,20),strPause))
        {
            //if (Global.time_doPause) {
            //	Debug.Log("RESUMED!");
            //	strPause = "Pause";
            //}
            //else {
            //	Debug.Log("PAUSED!");
            //	strPause = "Play";
            //}
            //Global.time_doPause = !Global.time_doPause;
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
		
		
    }*/

}