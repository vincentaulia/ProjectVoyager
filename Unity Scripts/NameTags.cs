//Created by Sarang Nerkar on 08/23/2014
using UnityEngine;
using System.Collections;

public class NameTags : MonoBehaviour {
	public Transform target;
	public Transform SunCam;
	public Transform MercuryCam;
	public Transform VenusCam;
	public Transform MoonCam;
	public Transform EarthCam;
	public Transform MarsCam;
	public Transform JupiterCam;
	public Transform SaturnCam;
	public Transform UranusCam;
	public Transform NeptuneCam;
	void Update ()
	{
		if(Input.GetKey("0")){
			target = SunCam;
		}
		if(Input.GetKey("1")){
			target = MercuryCam;
		}
		if(Input.GetKey("2")){
			target = VenusCam;
		}
		if(Input.GetKey("3")){
			target = MoonCam;
		}
		if(Input.GetKey("4")){
			target = EarthCam;
		}
		if(Input.GetKey("5")){
			target = MarsCam;
		}
		if(Input.GetKey("6")){
			target = JupiterCam;
		}
		if(Input.GetKey("7")){
			target = SaturnCam;
		}
		if(Input.GetKey("8")){
			target = UranusCam;
		}
		if(Input.GetKey("9")){
			target = NeptuneCam;
		}
		transform.LookAt(target);
	}
}
