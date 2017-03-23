/// <summary>
/// Generic GVR input class, Must trigger base Awake and update to use.
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum GVRSwipeDirection
{
	left,
	right,
	up,
	down

}

public class GVRInput : NetworkBehaviour
{

	public static GVRInput singilton;

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
		singilton = this;
		checkForSelection = true;
		if (GameObject.FindWithTag ("debug") != null) {
			debugLabel = GameObject.FindWithTag ("debug").GetComponent<Text> ();
			debugLabel.text = "Good to go";
		}
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
		if (GvrController.AppButtonDown) {
			DebugMessage ("App Button Down");
			AppButtonDown ();
		}
		if (GvrController.AppButtonUp) {
			DebugMessage ("App Button Up");
			AppButtonUp ();
		}
		if (GvrController.ClickButtonUp) {
			DebugMessage ("Button Up");
			OnButtonUp ();
		}
		if (GvrController.IsTouching) {

		}
		if (GvrController.ClickButtonDown) {
			Vector2 tempTouchPos = touchPosition;
			float touchDistace = Vector2.Distance (Vector2.one * 0.5f, tempTouchPos);

			DebugMessage ("Button Down:"+ tempTouchPos.ToString()+ "  Distance:" + touchDistace);
			if (touchDistace < 0.4f) {
				OnButtonDown ();
			} else {
				if (tempTouchPos.x > 0.5f) {
					// right side
					if (tempTouchPos.y < 0.5f) {
						// top right
						ButtonOptionRT ();
					} else {
						ButtonOptionRB ();
					}
				} else {
					// left side 
					if (tempTouchPos.y < 0.5f) {
						// top left
						ButtonOpetionLT ();
					} else {
						ButtonOptionLB ();
					}
				}
			}

		}
		if (GvrController.TouchDown) {
			DebugMessage ("TouchDown");
			if (detectSwipes) {
				startSwipePosition = touchPosition;
				isSwiping = true;
			}
			OnTouchDown ();
		}
		if (GvrController.TouchUp) {
			DebugMessage ("Touch Up");
			if (isSwiping) {
				CheckIfSwipped ();
			}
			OnTouchUp ();
		}
		#if !UNITY_EDITOR
		if (GvrController.State != GvrConnectionState.Connected) {
	foreach(Transform child in controllerPivot.transform){
	child.gameObject.SetActive(false);
	}
			
		} else {
	foreach(Transform child in controllerPivot.transform){
	child.gameObject.SetActive(true);
	}
			controllerPivot.transform.rotation = GvrController.Orientation;
		}
		#endif
		if (isSwiping) {
			endSwipePosition = touchPosition;
		}


	}

	/// Debug Messages
	public static void DebugMessage (string msg)
	{
		Debug.Log (msg);
		if (GVRInput.singilton.debugLabel == null)
			return;
		GVRInput.singilton.debugLabel.text += "\n" + msg;
	}

	/// <summary>
	/// Check if the swipe detected meets minim swipe requriments
	/// </summary>
	private void CheckIfSwipped ()
	{
		float fingerTravelDistance = Vector2.Distance (startSwipePosition, endSwipePosition);
		if (fingerTravelDistance >= minSwipeDistance) {
			Vector2 swipeDirection = endSwipePosition - startSwipePosition;
			if (Mathf.Abs (swipeDirection.x) > Mathf.Abs (swipeDirection.y)) {
				// Horizontal
				if (swipeDirection.x > 0) {
					// right
					OnSwipe (GVRSwipeDirection.right);
					DebugMessage ("Right : " + fingerTravelDistance.ToString ("0.00"));
						
				} else {
					// left
					OnSwipe (GVRSwipeDirection.left);
					DebugMessage ("Left : " + fingerTravelDistance.ToString ("0.00"));
				}

			} else {
				// Vertical
				if (swipeDirection.y < 0) {
					// up
					OnSwipe (GVRSwipeDirection.up);
					DebugMessage ("Up : " + fingerTravelDistance.ToString ("0.00"));
				} else {
					// down
					OnSwipe (GVRSwipeDirection.down);
					DebugMessage ("Down : " + fingerTravelDistance.ToString ("0.00"));
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
				return Vector2.one * 0.5f;
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

	public virtual void OnSwipe (GVRSwipeDirection dir)
	{

	}

	public virtual void AppButtonDown ()
	{

	}

	public virtual void AppButtonUp ()
	{

	}

	public virtual void ButtonOpetionLT ()
	{

	}

	public virtual void ButtonOptionLB ()
	{

	}

	public virtual void ButtonOptionRT ()
	{

	}

	public virtual void ButtonOptionRB ()
	{

	}


	#else
	void Start(){
	Debug.LogError("Not the right version of Unity");
	}
	#endif
}
