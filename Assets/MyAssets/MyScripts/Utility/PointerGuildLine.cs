// Used to control the line renders points. 

using UnityEngine;
using System.Collections;

public class PointerGuildLine : MonoBehaviour {

	// origin of the controller
	public Transform pivot;

	// object the controller is pointing at
	public Transform target;

	// the line displayed from origin to target
	public LineRenderer lineRend;

	void Start(){
		lineRend.SetVertexCount (2);
	}

		
	void LateUpdate () {
		Vector3[] points = new[] {pivot.position,target.position};
		lineRend.SetPositions (points);
	}
}
