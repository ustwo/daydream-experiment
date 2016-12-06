using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum TravelDirectionE{
	right,
	left,
	up,
	down
}

public class MeshTest : MonoBehaviour
{

	public float speed = 10;
	private Mesh mesh;
	Vector3 controlPoint = Vector3.zero;
	Vector3 lastPointAddPosition = Vector3.zero;
	List<Vector3> verts;
	private bool offset = false;
	private TravelDirectionE travelDirectionE;
	public Material brushMaterial;
	public Vector2 firstUV;
	public Vector2 secondUV;
	public Vector2 thirdUV;
	public Vector2 fourthUV;
	public TravelDirectionE lastTravelDirection;



	// Use this for initialization
	void Start ()
	{
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter> ();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		mesh = new Mesh ();
		meshFilter.mesh = mesh;
		verts = new List<Vector3>{ Vector3.zero, Vector3.right, Vector3.up*0.1f };
		mesh.SetVertices (verts);
		List<int> tris = new List<int>{ 0, 1, 2 };
		mesh.SetTriangles (tris, 0);
		meshRenderer.material = brushMaterial;
	}
	
	// Update is called once per frame
	void Update ()
	{
		controlPoint +=(new Vector3 (Input.GetAxis ("Horizontal") *-1, Input.GetAxis ("Vertical"), 0)) * (speed * Time.deltaTime);
		if (Vector3.Distance (controlPoint, lastPointAddPosition) > 0.1f) {
			Vector3 travelDirection = lastPointAddPosition - controlPoint;

			if (Mathf.Abs (travelDirection.x) > Mathf.Abs (travelDirection.y)) {
				//Horizontal
				if (travelDirection.x > 0) {
					// right
					travelDirectionE = TravelDirectionE.right;
				} else {
					// left
					travelDirectionE = TravelDirectionE.left;
				}
			} else {
				// Verticle
				if (travelDirection.y > 0) {
					// up
					travelDirectionE = TravelDirectionE.up;
				} else {
					//down
					travelDirectionE = TravelDirectionE.down;
				}
			}
			lastPointAddPosition = controlPoint;
			AddPoint ();
		}
	}

	void AddPoint ()
	{
		if (lastTravelDirection != travelDirectionE) {
			lastTravelDirection = travelDirectionE;

			return;
		}

		lastTravelDirection = travelDirectionE;
		List<int> tris = mesh.triangles.ToList ();
		verts = mesh.vertices.ToList ();
		mesh.Clear ();
		verts.Add (controlPoint + transform.TransformPoint( offsetPosition));
		tris.Add (verts.Count - (offset? 3 : 2));
		tris.Add (verts.Count - 1);
		tris.Add (verts.Count - (offset? 2 : 3));
		mesh.SetVertices (verts);
		mesh.SetTriangles (tris, 0);
		List<Vector2> uvs = new List<Vector2> ();
		for (int i = 0; i < mesh.vertexCount; i+=4) {
			uvs.Add (firstUV);
			if (i + 1 < mesh.vertexCount)
				uvs.Add (secondUV);
			if (i + 2 < mesh.vertexCount)
				uvs.Add (thirdUV);
			if (i + 3 < mesh.vertexCount)
				uvs.Add (fourthUV);
		}
		mesh.SetUVs (0,uvs);

	}

	Vector3 offsetPosition {
		get {
			offset = !offset;
			if (!offset)
				return Vector3.zero;
			switch (travelDirectionE) {
			case(TravelDirectionE.right):
				return Vector3.up;
			case(TravelDirectionE.left):
				return Vector3.down;
			case(TravelDirectionE.up):
				return Vector3.left;
			case(TravelDirectionE.down):
				return Vector3.right;
			default:
				return Vector3.zero;

			}



		}
	}
}
