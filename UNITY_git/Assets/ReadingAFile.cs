//Created by Sarang Nerkar
//transform.localScale += new Vector3(0.1F, 0, 0);
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

public class ReadingAFile : MonoBehaviour {
	public string[] parts;
	public Transform Sun;
	public Transform Mercury;
	public Transform Venus;
	public Transform Earth;
	public Transform Mars;
	public Transform Jupiter;
	public Transform Saturn;
	public Transform Uranus;
	public Transform Neptune;

	
	// Use this for initialization
	void Start () {
		int i;
		string[] allLines = File.ReadAllLines(@"tenyear.txt");

		for (i=0; i<36540; i++) {
						if (((i + 1) % 9) == 1) {
								string line = string.Empty;
								line = allLines [i];	
								parts = line.Split (' ');
				Sun.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
						}
			else if (((i + 1) % 9) == 2) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Mercury.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 3) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Venus.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 5) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Earth.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 6) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Mars.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 7) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Jupiter.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 8) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Saturn.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 9) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Uranus.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
			else if (((i + 1) % 9) == 0) {
				string line = string.Empty;
				line = allLines [i];	
				parts = line.Split (' ');
				Neptune.position = new Vector3 (((float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[4], CultureInfo.InvariantCulture.NumberFormat))/10000000), ((float.Parse(parts[5], CultureInfo.InvariantCulture.NumberFormat))/10000000));
			}
				}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
