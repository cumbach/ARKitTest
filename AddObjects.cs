using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AddObjects: MonoBehaviour {

	public GameObject box;
	public int multiplier = 1; // sets to one meter distance

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void test() {
		Debug.Log ("testing");
		//		Vector3 position = new Vector3 (Random.Range (-1f, 1f), 0, Random.Range (-1f, 1f));
		float xPos = transform.position.x - Camera.main.transform.position.x;
		float zPos = transform.position.z - Camera.main.transform.position.z;

		Vector3 position = new Vector3 (xPos * multiplier, transform.position.y, zPos * multiplier);
		multiplier += 1;
		Instantiate (box, position, Quaternion.Euler (position.x, 0, position.y));
	}
}
