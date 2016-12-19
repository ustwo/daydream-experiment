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

public class BrushGen : Photon.MonoBehaviour
{

	public float speed = 10;
	private Mesh mesh;
	Vector3 lastPointAddPosition = Vector3.zero;
	List<Vector3> verts;
	List<int> tris;
	private bool offset = false;
	private TravelDirectionE travelDirectionE;
	public Material[] brushMaterial;
	private Vector2 firstUV = new Vector2 (1, 0);
	private Vector2 secondUV = new Vector2 (1, 1);
	private Vector2 thirdUV = new Vector2 (0, 0);
	private Vector2 fourthUV = new Vector2 (0, 1);
	public TravelDirectionE lastTravelDirection;
	public Transform brushEndTransform;
	private Vector3 desiredPosition;
	private float brushSpeed = 50f;
	private bool strokeEnded = false;
	private bool meshChanged = false;
	public float brushSize = 0.5f;




	// Use this for initialization
	void Start ()
	{
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter> ();

		mesh = new Mesh ();
		mesh.name = "brushStrokeMesh";
		meshFilter.mesh = mesh;
		verts = new List<Vector3> ();
		tris = new List<int> ();
		lastPointAddPosition = brushEndTransform.position;


	}

	[PunRPC]
	public void SetMaterial (int index)
	{
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		if (index < brushMaterial.Length)
			meshRenderer.material = brushMaterial [index];
	}

	public void UpdateBrushPos (Transform pointRef)
	{
		brushEndTransform.position = pointRef.position;
		brushEndTransform.forward = pointRef.forward;
	}

	public void EndStroke ()
	{
		strokeEnded = true;
	}
	
	// Update is called once per frame
	void Update ()
	{ 
		
		if (strokeEnded || !photonView.isMine)
			return;

		if (verts.Count == 0) {
			AddPoint ();
			return;
		}

		if (Vector3.Distance (brushEndTransform.position, lastPointAddPosition) > 0.1f) {
			DeterminTravelDirection ();
			AddPoint ();
		}
	}

	void DeterminTravelDirection ()
	{
		
		Vector3 travelDirection = lastPointAddPosition - brushEndTransform.position;
		//brushEndTransform.forward = travelDirection;
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
		tris = mesh.triangles.ToList ();
		mesh.Clear ();
		if (verts.Count == 0) {
			Debug.Log ("adding Init point");
			verts.Add (brushEndTransform.position + brushEndTransform.TransformDirection ((Vector3.right + Vector3.up))*brushSize);
			verts.Add (brushEndTransform.position + brushEndTransform.TransformDirection ((Vector3.right + Vector3.down))*brushSize);
			verts.Add (brushEndTransform.position + brushEndTransform.TransformDirection ((Vector3.left + Vector3.up))*brushSize);
			verts.Add (brushEndTransform.position + brushEndTransform.TransformDirection ((Vector3.left + Vector3.down))*brushSize);
			tris.Add (0);
			tris.Add (1);
			tris.Add (2);
			tris.Add (2);
			tris.Add (1);
			tris.Add (3);
		} else {
			Vector3 nextPosition = brushEndTransform.position + brushEndTransform.TransformDirection (altOffset);
			verts.Add (nextPosition);
		}
		GenMesh (verts, tris);
		meshChanged = true;
	}

	void GenMesh (List<Vector3> incVerts, List<int> incTris)
	{
		if (mesh == null || verts == null || tris == null)
			return;
		mesh.SetVertices (verts);
		if (mesh.vertexCount >= 4) {
			incTris.Add (verts.Count - (offset ? 3 : 2));
			incTris.Add (verts.Count - 1);
			incTris.Add (verts.Count - (offset ? 2 : 3));
			mesh.SetTriangles (incTris, 0);
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

	[PunRPC]
	public void SetNetworkParent (string parentName)
	{
		GameObject newParent = GameObject.Find (parentName);
		if(newParent != null)
			transform.parent = newParent.transform;
	}
	Vector3 altOffset{
		get{
			offset = !offset;
			if (offset)
				return Vector3.right * brushSize;
			else
				return Vector3.left * brushSize;
		}
	}

	Vector3 offsetPosition {
		get {
			offset = !offset;
			switch (travelDirectionE) {
			case(TravelDirectionE.right):
				if (offset)
					return Vector3.up * brushSize;
				else
					return Vector3.down * brushSize;
			case(TravelDirectionE.left):
				if (offset)
					return Vector3.down * brushSize;
				else
					return Vector3.up * brushSize;
			case(TravelDirectionE.up):
				if (offset)
					return Vector3.left * brushSize;
				else
					return Vector3.right * brushSize;
			case(TravelDirectionE.down):
				if (offset)
					return Vector3.right * brushSize;
				else
					return Vector3.left * brushSize;
			default:
				return Vector3.zero;

			}



		}
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			Vector3[] vertArray = verts.ToArray ();
			stream.SendNext (vertArray);
			int[] triArray = tris.ToArray ();
			stream.SendNext (triArray);
			stream.SendNext (meshChanged);
			meshChanged = false;
		} else {
			Vector3[] vertArray = stream.ReceiveNext () as Vector3[];
			verts = vertArray.ToList ();
			int[] triArray = stream.ReceiveNext () as int[];
			tris = triArray.ToList ();
			meshChanged = (bool)stream.ReceiveNext ();
			if (meshChanged) {
				//Debug.Log ("Verts Count " + verts.Count + " Tris Count " + tris.Count);
				GenMesh (verts, tris);
			}
		

		}
	}
}
