﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.iOS
{

	public class ChrisEditorHitTest : MonoBehaviour {

		public Transform m_HitTransform;
		public Transform m_HitTransform2;
		public float maxRayDistance = 30.0f;
		public LayerMask collisionLayerMask;
		public int clickNum = 0;
		public Vector3 position1;
		public Vector3 position2;

		public ParticleSystem path;
		public GameObject finalLocation;
		private float multiplierX = .1f;
		private float multiplierZ = -.1f;
		private int forwardPoints = 0;
		private int perpendicularPoints = 0;
		public int totalStraight = 10;
		public int totalLeft = 10;
		public string placed;
		private int turn = 0;
		private Vector3 oldPosition;
		public bool buildRoutes = false;

		private List<ParticleSystem> particleSystemList; // Stores all particle systems
		private ParticleSystem particlePoint; // Stores current particle system
		private List<GameObject> endObjectList;
		private GameObject endObject;

		bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
		{
			List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
			if (hitResults.Count > 0) {
				foreach (var hitResult in hitResults) {
					if (placed != "placed") {
						placed = "unplaced";
					}

					if (clickNum == 0) {
						m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
					} else if (clickNum == 1) {
						m_HitTransform2.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
						placed = "placed";
					}

//					Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
					return true;
				}
			}
			return false;
		}


		void Start() {
			particleSystemList = new List<ParticleSystem> ();
			endObjectList = new List<GameObject> ();
		}


		void Update () {
			#if UNITY_EDITOR   //we will only use this script on the editor side, though there is nothing that would prevent it from working on device

			if (Input.GetMouseButton (0)) {// && !EventSystem.current.IsPointerOverGameObject ()) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;


				if( Physics.Raycast( ray, out hit, 100 ) )
				{
					if (hit.transform.gameObject.name == "Sphere") {
						clickNum = 0;
					} else if (hit.transform.gameObject.name == "Sphere2") {
						clickNum = 1;
						placed = "placed";
					}
				}

				//we'll try to hit one of the plane collider gameobjects that were generated by the plugin
				//effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
				if (Physics.Raycast (ray, out hit, maxRayDistance, collisionLayerMask)) {
					if (placed != "placed") {
						placed = "unplaced";
					}
					//we're going to get the position from the contact point
					if (clickNum == 0) {
						m_HitTransform.position = hit.point;
					} else if (clickNum == 1) {
						m_HitTransform2.position = hit.point;
					}
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				if (placed == "placed") {
					clickNum = 2;
				} else if (placed == "unplaced") {
					clickNum += 1;
				}
			}
			#endif

			#if UNITY_IPHONE

			if (Input.touchCount > 0 && m_HitTransform != null)
			{
				var touch = Input.GetTouch(0);

				Touch touchPoint = Input.touches[0];
				Ray touchRay = Camera.main.ScreenPointToRay(touchPoint.position);
				RaycastHit[] hits = Physics.RaycastAll(touchRay);
				foreach( RaycastHit hit in hits ) {

					if (hit.transform.gameObject.name == "Sphere") {
						clickNum = 0;
					} else if (hit.transform.gameObject.name == "Sphere2") {
						clickNum = 1;
						placed = "placed";
					}

				}

				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
				{
					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
					ARPoint point = new ARPoint {
						x = screenPosition.x,
						y = screenPosition.y
					};

					// prioritize reults types
					ARHitTestResultType[] resultTypes = {
						ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
						// if you want to use infinite planes use this:
						//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
						ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
						ARHitTestResultType.ARHitTestResultTypeFeaturePoint
					}; 

					foreach (ARHitTestResultType resultType in resultTypes)
					{
						if (HitTestWithResultType (point, resultType))
						{
							return;
						}
					}
				}
				if (touch.phase == TouchPhase.Ended) {
					if (placed == "placed") {
						clickNum = 2;
					} else if (placed == "unplaced") {
						clickNum += 1;
					}
				}
			}
			#endif
		}


		public void DestroyDots() {
			foreach (ParticleSystem p in particleSystemList) {
				Destroy (p);
			}
			foreach (GameObject o in endObjectList) {
				Destroy (o);
			}
				
			particleSystemList = new List<ParticleSystem> ();
			endObjectList = new List<GameObject> ();
			turn = 0;
		}


		public void AddDots () {
			float distanceX;
			float distanceY;
			float slope;
			float b;
			bool assignNew = false;
			if (buildRoutes) {
				assignNew = true;
			}

			while (forwardPoints < (10 * totalStraight) || perpendicularPoints <= (10 * totalLeft)) {

				if (turn != 0) {
					if (buildRoutes && assignNew) {
						oldPosition = position2;
					} else if (assignNew) {
//						oldPosition = new Vector3 (m_HitTransform2.position.x, m_HitTransform2.position.y, m_HitTransform2.position.z);
					}
					assignNew = false;

					distanceX = oldPosition.x + (multiplierX * (m_HitTransform2.position.x - m_HitTransform.position.x));
					distanceY = m_HitTransform2.position.y + 1f; // Add when ready for height to particles

					// Calculates slope and intersection points for first vector
//					y = mx + b;
					slope = (m_HitTransform2.position.z - m_HitTransform.position.z) / (m_HitTransform2.position.x - m_HitTransform.position.x);
//					slope = -1 / slope;
					b = oldPosition.z - (slope * oldPosition.x);
				} else {
					distanceX = m_HitTransform2.position.x + (multiplierX * (m_HitTransform2.position.x - m_HitTransform.position.x));
					distanceY = m_HitTransform2.position.y + 1f; // Add when ready for height to particles

					// Calculates slope and intersection points for first vector
					slope = (m_HitTransform2.position.z - m_HitTransform.position.z) / (m_HitTransform2.position.x - m_HitTransform.position.x);
					b = m_HitTransform2.position.z - (slope * m_HitTransform2.position.x);
				}

				// If first vector:
				if (forwardPoints < (10 * totalStraight)) {
					float Xdist = distanceX;
					float Zdist = slope * Xdist + b; // Y = mX + b
					position1 = new Vector3 (Xdist, distanceY, Zdist);
			
					particlePoint = Instantiate (path, position1, Quaternion.Euler (0, m_HitTransform2.position.y, 0));

					multiplierX += .1f;
					forwardPoints += 1;


				} else {
					// X position in relation to old X position with former delta Z
					float Xdist = position1.x + (multiplierZ * (m_HitTransform2.position.z - m_HitTransform.position.z));

					// Z position in relation to old Z position with former delta Z
					float Zdist = position1.z + (-1 / slope * multiplierZ * (m_HitTransform2.position.z - m_HitTransform.position.z));

					position2 = new Vector3 (Xdist, distanceY, Zdist);

					if (perpendicularPoints < (10 * totalLeft)) {
						particlePoint = Instantiate (path, position2, Quaternion.Euler (0, m_HitTransform2.position.y, 0));
					} else {
						endObject = Instantiate (finalLocation, position2, Quaternion.Euler (0, m_HitTransform2.position.y, 0));
						endObjectList.Add (endObject);
					}

					multiplierZ -= .1f;
					perpendicularPoints += 1;
					
				}
				particleSystemList.Add (particlePoint);

			}
			if (buildRoutes) {
//				oldPosition = position2;
			} else if (!buildRoutes && turn == 0) {
				oldPosition = new Vector3 (m_HitTransform2.position.x, m_HitTransform2.position.y, m_HitTransform2.position.z);
			}
			multiplierX = .1f;
			multiplierZ = -.1f;
			forwardPoints = 0;
			perpendicularPoints = 0;
			turn += 1;

		}


		public void dropdown (int index) {
			if (index == 1) {
				totalStraight = 10;
				totalLeft = 10;
			} else if (index == 2) {
				totalStraight = 26;
				totalLeft = 28;
			} else {
				totalStraight = 10 + index;
				totalLeft = 10 + index;
			}
		}

		public void toggleRoutes () {
			buildRoutes = !buildRoutes;
		}
	}
}
