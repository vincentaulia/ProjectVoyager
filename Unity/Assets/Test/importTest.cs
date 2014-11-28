using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

public class importTest : MonoBehaviour {
	public string myName;
	public float myAge, sisterAge;
	//public string data = File.ReadAllText( @"C:\Users\Sarang\Desktop\importTest1.txt" );
	
	// Use this for initialization
	void Start () {
		string data1 = File.ReadAllText( @"C:\Users\Sarang\Desktop\importTest1.txt" );
		string temp1 = "                                 ";
		string temp2;
		StringBuilder temp = new StringBuilder(temp1);
		StringBuilder data = new StringBuilder(data1);
		int i, iLower, iUpper, count, j, k;
		iLower = 0;
		iUpper = 0;
		count = 0;
		for (i=0; i<(data.Length); i++) {
						if (data [i] == ',') {
								iLower = iUpper;
								iUpper = i;
						}
						if (count == 0) {
								for (j=iLower, k=0; j<iUpper; j++, k++) {
								temp[k] = data[j];
								}
								temp2 = temp.ToString();
								myName=temp2;
								count++;
								temp2 = "                                 ";
						} else if (count == 1) {
								for (j=iLower, k=0; j<iUpper; j++, k++) {
								temp[k] = data[j];
								}
								temp2 = temp.ToString();
								myAge = float.Parse (temp2, CultureInfo.InvariantCulture.NumberFormat);
								count++;
								temp2 = "                                 ";
						} else if (count == 2) {
								for (j=iLower, k=0; j<iUpper; j++, k++) {
								temp[k] = data[j];
								}
								temp2 = temp.ToString();
								sisterAge = float.Parse (temp2, CultureInfo.InvariantCulture.NumberFormat);
								count++;
								temp2 = "                                 ";
						}
				}
		Debug.Log("I am alive and my name is " + myName + " my age is " + myAge + ", my sister's age is " + sisterAge);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}