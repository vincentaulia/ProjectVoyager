/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

public class split : MonoBehaviour {
	public string name;
	public float num1, num2;
	public string[] parts;

	void Main()
	{

		foreach (string line in File.ReadAllLines("output.txt"))
		{
			parts = line.Split(',');
			foreach(string some in parts){

				sw.WriteLine(some);
			}
			i++;

			
		}
		sw.Close ();

		//parts[1] = name;
		//num1 = float.Parse (parts[2], CultureInfo.InvariantCulture.NumberFormat);
		//num2 = float.Parse (parts[3], CultureInfo.InvariantCulture.NumberFormat);

	}
	void Start () {

		}
}*/