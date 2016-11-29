/// <summary>
/// Generic GVR input class, Must trigger base Awake and update to use.
/// </summary>

using UnityEngine;
using System.Collections;

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
			OnTouchDown ();
		}
		if (GvrController.TouchUp) {
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


	#else
	void Start(){
	Debug.LogError("Not the right version of Unity");
	}
	#endif
}
