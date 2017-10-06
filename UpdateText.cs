using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpdateText : MonoBehaviour {

	public Text coordinates;

	private void Update() {
		coordinates.text = "dir:" + Compass.Instance.direction1.ToString ();
	}
}
