using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
	public class NewHitTestExample : MonoBehaviour
	{
		public Transform m_HitTransform;

		bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
		{
			List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
			if (hitResults.Count > 0) {
				foreach (var hitResult in hitResults) {
					Debug.Log ("Got hit!");
					m_HitTransform.position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
					m_HitTransform.rotation = UnityARMatrixOps.GetRotation (hitResult.worldTransform);
					Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
					return true;
				}
			}
			return false;
		}

		// Update is called once per frame
		void Update () {
//			if (Input.touchCount > 0 && m_HitTransform != null)
			if (m_HitTransform != null)

			{
//				var touch = Input.GetTouch(0); //
				var touch = new Vector2(396f,689f); 

//				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
				if (true)
				{
//					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
					var screenPosition = Camera.main.ScreenToViewportPoint(touch);

//					Debug.Log (touch.position); //(396.0, 689.0)
//					Debug.Log (touch);

//					Debug.Log (screenPosition); //(0.5, 0.5, 0.0)


					// Simulate touch events from mouse events
					if (Input.touchCount == 0) {
						if (Input.GetMouseButtonDown(0) ) {
							Debug.Log("hello1");
							HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
						}
//						if (Input.GetMouseButton(0) ) {
//							HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
//						}
//						if (Input.GetMouseButtonUp(0) ) {
//							HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
//						}
					}


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
			}
		}

		private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase) {
			switch (touchPhase) {
			case TouchPhase.Began:
				Debug.Log ("hello2");
				// TODO
				break;
			case TouchPhase.Moved:
				// TODO
				break;
			case TouchPhase.Ended:
				// TODO
				break;
			}
		}

	}
}

