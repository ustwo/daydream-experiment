using UnityEngine;
using System.Collections;

public class TrackpadPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {
			Debug.Log (Input.touches [0].position);
		}
	}
}
