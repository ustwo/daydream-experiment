using UnityEngine;
using System.Collections;

public class RotateObserverCam : MonoBehaviour {

	public float mouseSpeed = 10;
	public float speed = 10;
	
	// Update is called once per frame
	void Update () {
		transform.root.Rotate (Vector3.up * speed * Time.deltaTime);
		transform.Rotate (Vector3.up * Input.GetAxis ("Mouse X") * mouseSpeed);
	}
}
