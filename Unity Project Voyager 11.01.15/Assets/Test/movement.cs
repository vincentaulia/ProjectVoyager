using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class movement : MonoBehaviour {
	public int forces;
	public float mass, massEarth;
	public string dataMass = File.ReadAllText( @"C:\Users\Sarang\Desktop\massTest.txt" );
	public string myName;
	public string data = File.ReadAllText( @"C:\Users\Sarang\Desktop\importTest.txt" );
	
	// Use this for initialization
	void Start () {
		Rigidbody rb = GetComponent<Rigidbody>();
		
		// Add a force to the Rigidbody.
		rb.mass = float.Parse(dataMass, CultureInfo.InvariantCulture.NumberFormat);
		massEarth = float.Parse(dataMass, CultureInfo.InvariantCulture.NumberFormat);
		rb.AddForce(Vector3.left * forces);
		transform.position = Vector3.right * 10;
		rb.velocity = new Vector3(0, 1, 0);
		myName = data;
		Debug.Log("I am alive and my name is " + myName + ". Mass of the Earth is " + massEarth);
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
