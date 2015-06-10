
using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class ShipNodeGui : MonoBehaviour
{

    //public GUISkin MenuSkin;

    public string anom = "0";
    public string orbits = "0";

    private int open = 1;
    public bool showButton = true;  //condition to show the DV button

    void OnGUI()
    {
        //GUI.skin = MenuSkin;
        //to make sure that the buttons are enabled only if the ship is selected
        if (Camera.main.GetComponent<CameraUserControl>().target.name != this.name)
        {
            return;
        }
        if (open == -1)
        {
            GUI.BeginGroup(new Rect(Screen.width - 130, Screen.height - 310, 120, 300));
            GUI.Box(new Rect(0, 0, 120, 300), "Maneuver Node");

            GUI.Label(new Rect(10, 120, 100, 20), "Mean Anomaly");
            anom = GUI.TextField(new Rect(10, 150, 80, 20), anom, 8);
            anom = Regex.Replace(anom, "[^.0-9]", "");

            GUI.Label(new Rect(10, 180, 100, 20), "# of Full Orbits");
            orbits = GUI.TextField(new Rect(10, 210, 80, 20), orbits, 3);
            orbits = Regex.Replace(orbits, "[^0-9]", "");

            if (GUI.Button(new Rect(10, 240, 100, 20), "Add New Node"))
            {
                //make sure the input is with
                if (float.Parse(anom) < 0 || float.Parse(anom) > 360)
                {
                    Debug.Log("ERROR: invalid input");
                }
                else
                {

                    //	(float.Parse (anom))*Mathf.PI/180  // ths is the radian version of the degree entry in the field
                    open *= -1;
                    //make sure the conversion works
                    if (orbits == "")
                    {
                        orbits = "0";
                    }
                    this.gameObject.GetComponent<shipOEHistory>().deltaVAdd((double.Parse(anom)) * Mathf.PI / 180, int.Parse(orbits));
                }
            }
            if (GUI.Button(new Rect(10, 270, 100, 20), "Cancel"))
            {
                open *= -1;
                showButton = true;
            }

            GUI.EndGroup();
        }
        else if (showButton)
        {
            if (GUI.Button(new Rect(Screen.width - 160, Screen.height - 80, 120, 40), "Add DV Node"))
            {
                anom = Global.time.ToString();
                open *= -1;
                //hide the button
                showButton = false;
            }
        }

    }
}
