using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class ChrisGeneratePlanes : MonoBehaviour
	{
		public GameObject planePrefab;
		private UnityARAnchorManager unityARAnchorManager;
		private bool planeDetected = false;
		public GameObject warningText;

		// Use this for initialization
		void Start () {
			unityARAnchorManager = new UnityARAnchorManager();
			UnityARUtility.InitializePlanePrefab (planePrefab);
		}

		void OnDestroy()
		{
			unityARAnchorManager.Destroy ();
		}

		void OnGUI()
		{
			List<ARPlaneAnchorGameObject> arpags = unityARAnchorManager.GetCurrentPlaneAnchors ();
			if (arpags.Count >= 1) {
				if (!planeDetected) {
					Destroy (warningText);
					planeDetected = true;
				}
					//ARPlaneAnchor ap = arpags [0].planeAnchor;
					//GUI.Box (new Rect (100, 100, 800, 60), string.Format ("Center: x:{0}, y:{1}, z:{2}", ap.center.x, ap.center.y, ap.center.z));
					//GUI.Box(new Rect(100, 200, 800, 60), string.Format ("Extent: x:{0}, y:{1}, z:{2}", ap.extent.x, ap.extent.y, ap.extent.z));
				}
			}
		}
	}

