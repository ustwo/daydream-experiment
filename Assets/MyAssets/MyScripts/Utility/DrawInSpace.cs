/// <summary>
/// Allows the user to draw a shape in space using the daydream pointer.
/// </summary>

using UnityEngine;
using System.Collections;
using Photon;

public class DrawInSpace : GVRInput {

	/// <summary>
	/// The object that gets copied when we begin a new stroke.
	/// </summary>
	public GameObject strokePrefab;

	/// <summary>
	/// Ref to the currently working stroke
	/// </summary>
	private GameObject activeStroke;

	/// <summary>
	/// Ref to the sphear at the end of the pointer
	/// </summary>
	public Transform pointerRef;

	/// <summary>
	/// The PhotonView of this object. 
	/// </summary>
	public PhotonView pview;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (!pview.isMine)
			return;
		base.Update ();

		// For Debuging, manually trigger the buttons.
		if (Input.GetMouseButtonDown (0))
			OnButtonDown ();
		if (Input.GetMouseButtonUp (0))
			OnButtonUp ();
	}

	/// <summary>
	/// Create a new stroke object and parent it to the pointer
	/// </summary>
	public override void OnButtonDown(){
		if (!pview.isMine)
			return;
		activeStroke = PhotonNetwork.Instantiate ("strokePrefab", pointerRef.position, Quaternion.identity, 0);
		activeStroke.transform.parent = pointerRef;
		//activeStroke = Instantiate (strokePrefab, pointerRef.position, Quaternion.identity, pointerRef) as GameObject;
	}

	/// <summary>
	/// unparent the stroke object.
	/// </summary>
	public override void OnButtonUp(){
		if (!pview.isMine)
			return;
		if (activeStroke != null) {
			activeStroke.transform.parent = null;
			activeStroke = null;
		}
	}



}
