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
	private string strPause = "Pause";
	private int outnum = 0;
	private int savedTimeScale;
	
	void guiTimeJump() {
		strJumpToTime = GUI.TextField(new Rect(Screen.width - 50*1-5*1,5,50,20),strJumpToTime,3);
		
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
			strTime = "Day " + (Global.time).ToString();
		}
		GUI.Label(new Rect(Screen.width - 50*3-5*3,5,50,20), strTime);
	}
	
	void guiTimeScale() {
		
		if (GUI.Button(new Rect(Screen.width-50*1-5*1,20+5*2,50,20),">>"))
		{
			if (Global.time_multiplier < 5)
				Global.time_multiplier += 1;
			
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
				Global.time_multiplier -= 1;
			
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
