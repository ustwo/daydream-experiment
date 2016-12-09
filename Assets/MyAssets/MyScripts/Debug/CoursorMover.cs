using UnityEngine;
using System.Collections;

public class CoursorMover : Photon.MonoBehaviour {

	public float speed = 10;
	// Use this for initialization

	// Update is called once per frame
	void FixedUpdate () {
		if (!photonView.isMine)
			return;
		//Debug.Log (Input.GetAxis ("Horizontal"));
		//transform.Translate (new Vector3 (Input.GetAxis ("Horizontal") * speed , Input.GetAxis ("Vertical") * speed , 0));
		#if UNITY_EDITOR
		transform.Rotate(new Vector3(Input.GetAxis("Mouse Y")*-speed,Input.GetAxis("Mouse X")*speed,0),Space.World);
		#endif
	}
}
