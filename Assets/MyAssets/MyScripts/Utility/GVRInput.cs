/// <summary>
/// Generic GVR input class, Must trigger base Awake and update to use.
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum GVRSwipeDirection{
	left,
	right,
	up,
	down

}

public class GVRInput : MonoBehaviour
{

	/// <summary>
	/// Where the pointer line is starts from.
	/// </summary>
	public GameObject controllerPivot;

	/// <summary>
	/// The object we are currently pointing at
	/// </summary>
	protected GameObject selectedObject;

	/// <summary>
	/// Turn changing selectiedObject on and off
	/// </summary>
	protected bool checkForSelection;

	/// <summary>
	/// The physics layer we are checking against
	/// </summary>
	public LayerMask detectionMask;

	///<summary>
	/// Textlabel used for debuging
	/// </summary>
	public Text debugLabel;


	///<summary>>
	/// If detectSwipe = is true, the system will check touch input for swiping moves. 
	/// </summary>
	public bool detectSwipes = false;
	public float minSwipeDistance = 0.5f;
	private bool isSwiping = false;
	private Vector2 startSwipePosition;
	private Vector2 endSwipePosition;

	#if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)

	public virtual void Awake ()
	{
		checkForSelection = true;
	}

	public  virtual void Update ()
	{
		UpdatePointer ();
	}

	/// <summary>
	/// Update the state of the pointer. 
	/// </summary>
	protected virtual void UpdatePointer ()
	{

		///<summary>>
		/// Checking for controller States
		/// </summary>
		if (GvrController.ClickButtonUp) {
			OnButtonUp ();
		}
		if (GvrController.ClickButtonDown) {
			OnButtonDown ();
		}
		if (GvrController.TouchDown) {
			if (detectSwipes) {
				startSwipePosition = touchPosition;
				isSwiping = true;
			}
			OnTouchDown ();
		}
		if (GvrController.TouchUp) {
			if (isSwiping) {
				CheckIfSwipped ();
			}
			OnTouchUp ();
		}
	#if !UNITY_EDITOR
		if (GvrController.State != GvrConnectionState.Connected) {
			controllerPivot.SetActive (false);
		} else {
			controllerPivot.SetActive (true);
			controllerPivot.transform.rotation = GvrController.Orientation;
		}
	#endif
		if (isSwiping) {
			endSwipePosition = touchPosition;
		}

		if (checkForSelection) {
			RaycastHit hit;
			Vector3 rayDirection = GvrController.Orientation * Vector3.forward;
			if (Physics.Raycast (Vector3.zero, rayDirection, out hit, detectionMask)) {
				selectedObject = hit.collider.gameObject;
			} else {
				selectedObject = null;
			}

		}
	}

	/// <summary>
	/// Check if the swipe detected meets minim swipe requriments
	/// </summary>
	private void CheckIfSwipped(){
		float fingerTravelDistance = Vector2.Distance (startSwipePosition, endSwipePosition);
		if (fingerTravelDistance >= minSwipeDistance) {
			Vector2 swipeDirection = endSwipePosition - startSwipePosition;
			if (Mathf.Abs (swipeDirection.x) > Mathf.Abs (swipeDirection.y)) {
				// Horizontal
				if (swipeDirection.x > 0) {
					// right
					OnSwipe(GVRSwipeDirection.right);
					if (debugLabel != null)
						debugLabel.text = "Right : " + fingerTravelDistance.ToString ();
				} else {
					// left
					OnSwipe(GVRSwipeDirection.left);
					if (debugLabel != null)
						debugLabel.text = "Left : " + fingerTravelDistance.ToString ();
				}

			} else {
				// Vertical
				if (swipeDirection.y > 0) {
					// up
					OnSwipe(GVRSwipeDirection.up);
					if (debugLabel != null)
						debugLabel.text = "Up : " + fingerTravelDistance.ToString ();
				} else {
					// down
					OnSwipe(GVRSwipeDirection.down);
					if (debugLabel != null)
						debugLabel.text = "Down : " + fingerTravelDistance.ToString ();
				}
			}
		}
	}

	// returns the current touch position of the daydream pointer. If not touching returns zeors
	protected Vector2 touchPosition {
		get {
			if (GvrController.IsTouching)
				return GvrController.TouchPos;
			else
				return Vector2.zero;
		}
	}

	/// <summary>
	/// Manually set selection of an object.
	/// </summary>
	protected virtual void SelectObject (GameObject obj)
	{
		checkForSelection = false;
		selectedObject = obj;
	}

	///<summary>>
	///following states are overwrited by subclass
	/// </summary>
	public  virtual void OnTouchDown ()
	{

	}

	public  virtual void OnTouchUp ()
	{

	}


	public  virtual void OnButtonDown ()
	{

	}

	public  virtual void OnButtonUp ()
	{

	}

	public  virtual void FixedUpdate ()
	{

	}
	public virtual void OnSwipe(GVRSwipeDirection dir){

	}


	#else
	void Start(){
	Debug.LogError("Not the right version of Unity");
	}
	#endif
}
