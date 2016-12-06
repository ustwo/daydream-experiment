using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum TravelDirectionE
{
	right,
	left,
	up,
	down
}

public class BrushGen : MonoBehaviour
{

	public float speed = 10;
	private Mesh mesh;
	Vector3 lastPointAddPosition = Vector3.zero;
	List<Vector3> verts;
	private bool offset = false;
	private TravelDirectionE travelDirectionE;
	public Material brushMaterial;
	private Vector2 firstUV = new Vector2(1,0);
	private Vector2 secondUV = new Vector2(1,1);
	private Vector2 thirdUV = new Vector2(0,0);
	private Vector2 fourthUV = new Vector2(0,1);
	public TravelDirectionE lastTravelDirection;
	public Transform brushEndTransform;
	private Vector3 desiredPosition;
	private float brushSpeed = 50f;
	private bool strokeEnded  = false;



	// Use this for initialization
	void Start ()
	{
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter> ();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		mesh = new Mesh ();
		meshFilter.mesh = mesh;
		verts = new List<Vector3> ();
		meshRenderer.material = brushMaterial;
		lastPointAddPosition = brushEndTransform.position;

	}

	public void UpdateBrushPos(Vector3 incPos){
		//if(desiredPosition== null)
			brushEndTransform.position = incPos;

	//desiredPosition = incPos;
	
	}
	public void EndStroke(){
		strokeEnded = true;
	}
	
	// Update is called once per frame
	void Update ()
	{ 
		if (strokeEnded)
			return;
		//if(desiredPosition!=null)
			//brushEndTransform.position = Vector3.MoveTowards (brushEndTransform.position, desiredPosition, brushSpeed * Time.deltaTime);
		

		if (Vector3.Distance (brushEndTransform.position, lastPointAddPosition) > 0.1f) {
			DeterminTravelDirection ();
			AddPoint ();
		}
	}
	void DeterminTravelDirection(){
		Vector3 travelDirection = lastPointAddPosition - brushEndTransform.position;

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
		lastPointAddPosition = brushEndTransform.position;
	}

	void AddPoint ()
	{

		lastTravelDirection = travelDirectionE;
		verts = mesh.vertices.ToList ();
		List<int> tris = mesh.triangles.ToList ();
		mesh.Clear ();
		Vector3 nextPosition = brushEndTransform.position + brushEndTransform.TransformDirection (offsetPosition);
		verts.Add (nextPosition);
		//GameObject showMe = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		//showMe.transform.position = nextPosition;
		GenMesh (verts,tris);

	}

	void GenMesh(List<Vector3> incVerts, List<int> tris){
		
		mesh.SetVertices (verts);
	//	if(mesh.vertexCount>30)
	//		Debug.LogError ("wtf?");
		if (mesh.vertexCount > 4) {
			tris.Add (verts.Count - (offset ? 3 : 2));
			tris.Add (verts.Count - 1);
			tris.Add (verts.Count - (offset ? 2 : 3));
			mesh.SetTriangles (tris, 0);
			List<Vector2> uvs = new List<Vector2> ();
			for (int i = 0; i < mesh.vertexCount; i += 4) {
				uvs.Add (firstUV);
				if (i + 1 < mesh.vertexCount)
					uvs.Add (secondUV);
				if (i + 2 < mesh.vertexCount)
					uvs.Add (thirdUV);
				if (i + 3 < mesh.vertexCount)
					uvs.Add (fourthUV);
			}
			mesh.SetUVs (0, uvs);
		}

	}

	Vector3 offsetPosition {
		get {
			offset = !offset;
			if (!offset)
				return Vector3.zero;
			switch (travelDirectionE) {
			case(TravelDirectionE.right):
				return Vector3.up * 0.5f;
			case(TravelDirectionE.left):
				return Vector3.down  *0.5f;
			case(TravelDirectionE.up):
				return Vector3.left * 0.5f;
			case(TravelDirectionE.down):
				return Vector3.right * 0.5f;
			default:
				return Vector3.zero;

			}



		}
	}
}
