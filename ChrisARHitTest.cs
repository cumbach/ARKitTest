using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.iOS
{
	public class ChrisARHitTest : MonoBehaviour
	{
		public Transform m_HitTransform;
		public Transform m_HitTransform2;
		public int clickNum = 0;
		public Vector3 position1;

		public ParticleSystem path;
		public GameObject finalLocation;
		public float multiplierX = .1f;
		public float multiplierZ = .1f;
		public int n = 10;
		public float forwardPoints = 0f;
		public float perpendicularPoints = 0f;
		public int totalRight = 10;
		public string placed;

		bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
		{
			List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
			if (hitResults.Count > 0) {
				foreach (var hitResult in hitResults) {
					Debug.Log ("Got hit!");
					if (placed != "placed") {
						placed = "unplaced";
					}
//					m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
//
////					m_HitTransform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
//					transform.LookAt(Camera.main.transform.position);
//					transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

					if (clickNum == 0) {
						m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
//						clickNum += 1;
					} else if (clickNum == 1) {
						m_HitTransform2.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
						placed = "placed";
//						clickNum += 1;
					}

					Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
					return true;
				}
			}
			return false;
		}

		// Update is called once per frame
		#if UNITY_IPHONE
		void Update () {
			if (Input.touchCount > 0 && m_HitTransform != null)
			{
				var touch = Input.GetTouch(0);

				Touch touchPoint = Input.touches[0];
				Ray touchRay = Camera.main.ScreenPointToRay(touchPoint.position);
				RaycastHit[] hits = Physics.RaycastAll(touchRay);
				foreach( RaycastHit hit in hits ) {

					if (hit.transform.gameObject.name == "Sphere") {
						Debug.Log (hit.transform.gameObject.name);
						clickNum = 0;
					} else if (hit.transform.gameObject.name == "Sphere2") {
						Debug.Log (hit.transform.gameObject.name);
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
					Debug.Log("touchup phone");
					if (placed == "placed") {
						clickNum = 2;
					} else if (placed == "unplaced") {
						clickNum += 1;
					}
				}
			}
		}

		public void AddDots () {
			Debug.Log ("AddDotsPhone");
			while (forwardPoints < (10 * n) || perpendicularPoints <= (10 * totalRight)) {
				// Gets distance for how far first vector elements go
				float distanceX = m_HitTransform2.position.x + (multiplierX * (m_HitTransform2.position.x - m_HitTransform.position.x));
				float distanceY = m_HitTransform2.position.y + 1f; // Add when ready for height to particles
//				float distanceZ = m_HitTransform2.position.z + (multiplierX * (m_HitTransform2.position.z - m_HitTransform.position.z));

				// Calculates slope and intersection points for first vector
				float slope = (m_HitTransform2.position.z - m_HitTransform.position.z) / (m_HitTransform2.position.x - m_HitTransform.position.x);
				float b = m_HitTransform2.position.z - (slope * m_HitTransform2.position.x);

				// If first vector:
				if (forwardPoints < (10 * n)) {
					Debug.Log ("first Phone");
					float Xdist = distanceX;
					float Zdist = slope * Xdist + b; // Y = mX + b
					Debug.Log (Zdist);
					position1 = new Vector3 (Xdist, distanceY, Zdist);


					Instantiate (path, position1, Quaternion.Euler (0, m_HitTransform2.position.y, 0));
					multiplierX += .1f;
					forwardPoints += 1;


				} else {
					// X position in relation to old X position with former delta Z
					float Xdist = position1.x + (multiplierZ * (m_HitTransform2.position.z - m_HitTransform.position.z));
					Debug.Log ("next Phone");
					Debug.Log (position1);
					Debug.Log (Xdist);

					// Z position in relation to old Z position with former delta Z
					float Zdist = position1.z + (-1 / slope * multiplierZ * (m_HitTransform2.position.z - m_HitTransform.position.z));
					Debug.Log (Zdist);

					Vector3 position = new Vector3 (Xdist, distanceY, Zdist);
					Debug.Log (perpendicularPoints);
					Debug.Log (totalRight);
					if (perpendicularPoints < (10 * totalRight)) {
						Debug.Log ("perp Phone");
						Instantiate (path, position, Quaternion.Euler (0, m_HitTransform2.position.y, 0));
					} else {
						Debug.Log ("last Phone");
						Instantiate (finalLocation, position, Quaternion.Euler (0, m_HitTransform2.position.y, 0));
					}

					multiplierZ += .1f;
					perpendicularPoints += 1;

				}
			}
			multiplierX = .1f;
			multiplierZ = .1f;
			forwardPoints = 0;
			perpendicularPoints = 0;



		}

		public void dropdown (int index) {
			Debug.Log ("dropdown");
			n = 10 + index;
		}

		#endif



	}
}

