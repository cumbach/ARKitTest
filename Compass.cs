using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour {

	public float direction1;
	public GameObject pointer;
	public static Compass Instance { set; get; }
	private int counter = 0;

	// Use this for initialization
	private void Start () {
		Instance = this;
		DontDestroyOnLoad (gameObject);
		StartCoroutine (StartLocationService ());
	}

	private IEnumerator StartLocationService() {
		Debug.Log ("Starting Connection");

		yield return new WaitForSeconds (3);

		Debug.Log("Finished Waiting");
		#if UNITY_EDITOR
		//Wait until Unity connects to the Unity Remote

		while (!UnityEditor.EditorApplication.isRemoteConnected)
		{
			Debug.Log("Remote not Connected");
			yield return null;
		}
		#endif

		if (!Input.location.isEnabledByUser) {
			Debug.Log("User has not enabled GPS");
			yield break;
		}

		Input.location.Start();
		Input.compass.enabled = true;

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		if (maxWait <= 0) {
			Debug.Log("Timed out");
			yield break;
		}

		if (Input.location.status == LocationServiceStatus.Failed) {
			Debug.Log("Unable to determine device location");
			yield break;
		}


		direction1 = Input.compass.trueHeading;

//		pointer.transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
		yield break;
	}

	void Update() {
		if (direction1 != 0 && counter < 30) {
			Debug.Log (-Input.compass.trueHeading);
			counter++;

		}

		direction1 = Input.compass.trueHeading;
		if (direction1 != 0 && counter == 20) {
			Debug.Log ("hello1");
			Debug.Log (pointer.transform.rotation);
			Debug.Log (-Input.compass.trueHeading);
			Debug.Log ("hello2");
			
			pointer.transform.rotation = Quaternion.Euler (0, -Input.compass.trueHeading, 0);
			Debug.Log (pointer.transform.rotation);
		}

	}
}
