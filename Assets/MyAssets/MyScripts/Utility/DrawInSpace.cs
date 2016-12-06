/// <summary>
/// Allows the user to draw a shape in space using the daydream pointer.
/// </summary>

using UnityEngine;
using System.Collections;
using Photon;

public class DrawInSpace : GVRInput
{

	/// <summary>
	/// The object that gets copied when we begin a new stroke.
	/// </summary>
	public GameObject strokePrefab;

	/// NodePrefab to be created and hold contecnt
	public GameObject nodePrefab;
	private Node activeNode;
	private bool nodeReleased = false;

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

	private bool shouldDraw = true;
	private bool isDrawing = false;

	private Node activeMove;

	public Transform rayHitRef;

	private bool offline = true;

	// Use this for initialization
	public override void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		base.Awake ();
	}

	public void Start(){
		offline = !PhotonNetwork.connected;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		if (!pview.isMine && !offline) {
	
			return;
		}
		base.Update ();

		// For Debuging, manually trigger the buttons.
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			OnSwipe (GVRSwipeDirection.down);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			OnSwipe (GVRSwipeDirection.up);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			OnSwipe (GVRSwipeDirection.right);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			AppButtonDown ();
		}
		if (Input.GetMouseButtonDown (0))
			OnButtonDown ();
		if (Input.GetMouseButtonUp (0))
			OnButtonUp ();


		RaycastHit hit;
			
		if (Physics.Linecast (controllerPivot.transform.position, pointerRef.position + controllerPivot.transform.forward, out hit, detectionMask)) {
			rayHitRef.position = hit.point;
			selectedObject = hit.collider.gameObject;
//			Debug.Log (selectedObject.tag);
		} else {
			

			if (isDrawing && activeNode != null)
				EndDrawStroke ();
			selectedObject = null;
			rayHitRef.position = pointerRef.position+controllerPivot.transform.forward*5;
		}


	}




	/// <summary>
	/// Create a new stroke object and parent it to the pointer
	/// </summary>
	public override void OnButtonDown ()
	{
		if (activeNode != null || selectedObject == null && activeNode == null) {
			StartDrawStroke ();
		} else {
			StartMove ();
		}
	}

	void StartDrawStroke ()
	{
		DebugMessage ("Button Down from DrawInSpace");
		isDrawing = true;
		if (!offline)
			activeStroke = PhotonNetwork.Instantiate (strokePrefab.name, rayHitRef.position, Quaternion.identity, 0);
		else
			activeStroke = Instantiate (strokePrefab,rayHitRef.position,Quaternion.identity) as GameObject;
		activeStroke.name = "activeStroke";
		activeStroke.transform.parent = rayHitRef;
		//activeStroke = Instantiate (strokePrefab, pointerRef.position, Quaternion.identity, pointerRef) as GameObject;
	}

	void EndDrawStroke ()
	{
		
		DebugMessage ("Button Up from DrawinSpace");
		if (!pview.isMine&& !offline)
			return;
		isDrawing = false;
		if (activeStroke != null) {
			if (activeNode == null) {
				activeStroke.transform.parent = null;
			} else {
				
				// Bake Stroke
				GameObject thisStroke = activeStroke.GetComponent<Trail> ().GetCurrentStroke;
				GameObject cloneStroke = Instantiate (thisStroke,activeNode.nodeTransform) as GameObject;
				for (int i = 0; i < 10; i++) {
					SmoothVertexes(cloneStroke.GetComponent<MeshFilter>().sharedMesh);
				}

				Destroy (activeStroke);
			}
			activeStroke = null;
		}
	}
	void SmoothVertexes(Mesh mesh){
		for (int i = 0; i < mesh.vertexCount; i++) {
			if (i - 2 < 0 || i + 2 >= mesh.vertexCount)
				continue;
			mesh.vertices[i] = GetMeanVector(new Vector3[]{mesh.vertices[i-2],mesh.vertices[i],mesh.vertices[i+2]});
		}
	}
	private Vector3 GetMeanVector(Vector3[] positions)
	{
		if (positions.Length == 0)
			return Vector3.zero;
		float x = 0f;
		float y = 0f;
		float z = 0f;
		foreach (Vector3 pos in positions)
		{
			x += pos.x;
			y += pos.y;
			z += pos.z;
		}
		return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
	}


	void StartMove ()
	{
		if (selectedObject == null || selectedObject.tag == "MicButton")
			return;
		
		activeMove = selectedObject.GetComponent<Node>();
		activeMove.SetTarget(pointerRef);

	}

	void StopMove ()
	{
		activeMove.SetDesiredPosition (pointerRef.position);
		activeMove.SetTarget (null);
		activeMove = null;
	}

	/// <summary>
	/// unparent the stroke object.
	/// </summary>
	public override void OnButtonUp ()
	{
		if (activeMove != null) {
			StopMove ();
		} else {
			EndDrawStroke ();
		}

		if (selectedObject != null && selectedObject.tag == "MicButton") {
			
		}
	}


	public override void OnSwipe (GVRSwipeDirection dir)
	{
		
		if (dir == GVRSwipeDirection.down) {
			if (selectedObject != null)
				ActivateNode (selectedObject);
			else {
				CreateNode ();
			}
				
	
		} else if (dir == GVRSwipeDirection.up)
			CommitNode ();
		else if (dir == GVRSwipeDirection.right) {
			if (activeMove != null)
				return;
			shouldDraw = !shouldDraw;
			DebugMessage ("should draw = " + shouldDraw);
		}
		
		base.OnSwipe (dir);

	}

	void ActivateNode (GameObject incNode)
	{
		activeNode = incNode.GetComponent<Node>();
		Vector3 halfPoint = Vector3.Lerp (pointerRef.position, controllerPivot.transform.position, 0.5f);
		activeNode.SetDesiredPosition(halfPoint);
	
	}

	void CreateNode ()
	{
		if (!offline)
			ActivateNode (PhotonNetwork.Instantiate (nodePrefab.name, pointerRef.position, Quaternion.identity, 0));
		else
			ActivateNode (Instantiate (nodePrefab, pointerRef.position, Quaternion.identity) as GameObject);
	}

	void CommitNode ()
	{
		if (activeNode == null)
			return;
		if (isDrawing)
			EndDrawStroke ();
		DebugMessage ("commiting Node");
//		activeNode.nodeTransform.parent = pointerRef;
//		activeNode.transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, 0);
//		activeNode.transform.parent = null;
		activeNode.SetDesiredPosition(pointerRef.position);
		activeNode = null;

	}

	public override void AppButtonDown ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}



}
