using UnityEngine;
using System.Collections;

public class HasGizmo : MonoBehaviour {

	public enum GizmoType{
		Sphere,
		Cube
	}
	public GizmoType gizmoType;
	public Color gizmoColor = Color.red;
	public float gizmoSize = 1;

	void OnDrawGizmos(){
		Gizmos.color = gizmoColor;
		switch (gizmoType) {
		case(GizmoType.Sphere):
			Gizmos.DrawSphere (transform.position, 1 * gizmoSize);
			break;
		case(GizmoType.Cube):
			Gizmos.DrawCube (transform.position, Vector3.one * gizmoSize);
			break;

		}
	}
}
