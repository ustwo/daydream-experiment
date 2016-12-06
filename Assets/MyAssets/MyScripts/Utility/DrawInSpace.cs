/// <summary>
/// Allows the user to draw a shape in space using the daydream pointer.
/// </summary>

using UnityEngine;
using System.Collections;
using Photon;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Widgets;

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
	private BrushGen activeStroke;

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

	private bool drawingOnBackground = false;

	/// <summary>
	/// Input mode.
	/// </summary>
	enum InputMode {
		DRAW,
		MICROPHONE
	}

	private InputMode currentInputMode;
	private int modeNum = 0;
	private Dictionary<int, InputMode> modeDict;

	public MicrophoneWidget micWidget;

	public SpeechToTextWidget sttWidget;


	// Use this for initialization
	public override void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		base.Awake ();
	}

	public void Start ()
	{
		modeDict = new Dictionary<int, InputMode> ();
		modeDict.Add (0, InputMode.DRAW);
		modeDict.Add (1, InputMode.MICROPHONE);

		currentInputMode = InputMode.DRAW;
		modeNum = 0;

		offline = !PhotonNetwork.connected;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		if (!pview.isMine && !offline) {
	
			return;
		}
		if (activeStroke != null)
			activeStroke.UpdateBrushPos (rayHitRef.position);
		
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

		if (Input.GetKeyUp (KeyCode.Alpha9)) {
			modeNum--;
			UpdateMode ();
		}
			
		if (Input.GetKeyUp (KeyCode.Alpha0)) {
			modeNum++;
			UpdateMode ();
		}


		RaycastHit hit;

		if (Physics.Linecast (controllerPivot.transform.position, pointerRef.position + controllerPivot.transform.forward, out hit, detectionMask) && !drawingOnBackground) {
			rayHitRef.position = hit.point + (controllerPivot.transform.position - rayHitRef.position).normalized * 0.1f;
			selectedObject = hit.collider.gameObject;
//			Debug.Log (selectedObject.tag);
		} else {
			

			if (isDrawing && activeNode != null)
				EndDrawStroke ();
			selectedObject = null;
			rayHitRef.position = pointerRef.position + controllerPivot.transform.forward * 5;
		}


	}




	/// <summary>
	/// Create a new stroke object and parent it to the pointer
	/// </summary>
	public override void OnButtonDown ()
	{
		if (activeNode != null || selectedObject == null && activeNode == null) {

			switch(currentInputMode) {
			case InputMode.DRAW:
				StartDrawStroke ();
				break;
			case InputMode.MICROPHONE:
				StartMicrophone ();
				break;
			default:
				break;
			}

		} else {
			StartMove ();
		}
	}

	void StartDrawStroke ()
	{
		
		DebugMessage ("Button Down from DrawInSpace");
		isDrawing = true;
		activeStroke = (Instantiate (strokePrefab, Vector3.zero,Quaternion.identity) as GameObject).GetComponent<BrushGen> ();
		activeStroke.gameObject.name = "activeStroke";
		if (activeNode != null)
			activeStroke.transform.parent = activeNode.transform;
		else
			drawingOnBackground = true;
		
		//activeStroke = Instantiate (strokePrefab, pointerRef.position, Quaternion.identity, pointerRef) as GameObject;
	}

	void EndDrawStroke ()
	{
		drawingOnBackground = false;
		DebugMessage ("Button Up from DrawinSpace");
		if (!pview.isMine && !offline)
			return;
		isDrawing = false;
		if (activeStroke != null) {
			activeStroke.EndStroke ();
			if (activeNode == null) {
				activeStroke.transform.parent = null;
			} else {
				
				// Bake Stroke
				//GameObject thisStroke = activeStroke.GetComponent<Trail> ().GetCurrentStroke;
				//GameObject cloneStroke = Instantiate (thisStroke, activeNode.nodeTransform) as GameObject;
		

			//	Destroy (activeStroke);
			}
			activeStroke = null;
		}
	}


	void StartMove ()
	{
		if (selectedObject == null)
			return;
		activeMove = selectedObject.GetComponent<Node> ();
		activeMove.SetTarget (pointerRef);

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
			switch(currentInputMode) {
			case InputMode.DRAW:
				EndDrawStroke ();
				break;
			case InputMode.MICROPHONE:
				StopMicrophone ();
				break;
			default:
				break;
			}
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
		} 
		else if (dir == GVRSwipeDirection.up) {
			CommitNode ();
		}
			
		else if (dir == GVRSwipeDirection.right) {
			
			if (activeMove != null) {
				return;
			}

			modeNum++;
			UpdateMode ();

			shouldDraw = !shouldDraw;
			DebugMessage ("should draw = " + shouldDraw);
		} 
		else if(dir == GVRSwipeDirection.left) {
			modeNum--;
			UpdateMode ();
		}
		
		base.OnSwipe (dir);

	}

	void UpdateMode() 
	{
		if(modeNum < 0) {
			modeNum = modeDict.Count - 1;
		}

		if(modeNum > modeDict.Count - 1) {
			modeNum = 0;
		}

		currentInputMode = modeDict [modeNum];

		Debug.Log (currentInputMode);
	}

	void ActivateNode (GameObject incNode)
	{
		activeNode = incNode.GetComponent<Node> ();
		Vector3 halfPoint = Vector3.Lerp (pointerRef.position, controllerPivot.transform.position, 0.5f);
		activeNode.SetDesiredPosition (halfPoint);
	
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
		activeNode.SetDesiredPosition (pointerRef.position);
		activeNode = null;

	}

	public override void AppButtonDown ()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}

	/// <summary>
	/// Starts the microphone.
	/// </summary>
	void StartMicrophone() 
	{
		Debug.Log ("Starting microphone");
	}

	/// <summary>
	/// Stops the microphone.
	/// </summary>
	void StopMicrophone()
	{
		Debug.Log ("Stopping microphone");
	}


}
