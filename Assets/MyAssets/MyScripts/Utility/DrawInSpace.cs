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

	public Transform ToolGuideGizmo;
	public Transform ToolGuideAnchor;

	public Tool[] toolCollection;

	/// <summary>
	/// Input mode.
	/// </summary>
	public enum InputMode
	{
		DRAW = 0,
		MICROPHONE = 1,
		MOVE = 2
	}


	private InputMode currentInputMode;
	private InputMode lastInputMode;

	private int modeNum = 0;
	private Dictionary<int, InputMode> selectableModeDict;

	public GameObject micPrefab;
	private MicrophoneWidget micWidget;

	public GameObject sttPrefab;
	private STTController sttWidget;

	private Quaternion wantedRotation;

	public float rotationSpeed = 10f;
	//	public GameObject sttCanvas;


	// Use this for initialization
	public override void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		base.Awake ();
	}

	public void Start ()
	{
		wantedRotation = transform.rotation;
		selectableModeDict = new Dictionary<int, InputMode> ();
		selectableModeDict.Add (0, InputMode.DRAW);
		selectableModeDict.Add (1, InputMode.MICROPHONE);

		SetMode (InputMode.DRAW);

		micWidget = (Instantiate (micPrefab, pointerRef.position, Quaternion.identity) as GameObject).GetComponent<MicrophoneWidget> ();
		sttWidget = (Instantiate (sttPrefab, pointerRef.position, Quaternion.identity) as GameObject).GetComponent<STTController> ();
//		sttCanvas = Instantiate (sttCanvas, pointerRef.position, Quaternion.identity) as GameObject;

		offline = !PhotonNetwork.connected;
		rayHitRef.parent = null;
		rayHitRef.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		//DebugMessage (transform.position.ToString());
		if (!pview.isMine && !offline) {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, wantedRotation, rotationSpeed * Time.deltaTime);
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
			rayHitRef.position = hit.point + (hit.normal).normalized * 0.5f;
			rayHitRef.forward = hit.normal;
			selectedObject = hit.collider.gameObject;
			if (activeNode == null && selectedObject.GetComponent<Node> () != null) {
				if (currentInputMode != InputMode.MOVE)
					lastInputMode = currentInputMode;
				toolCollection [(int)InputMode.MOVE].SetMoveTarget (rayHitRef);
				SetMode (InputMode.MOVE);

			}
			rayHitRef.gameObject.SetActive (true);
//			Debug.Log (selectedObject.tag);
		} else {
			rayHitRef.gameObject.SetActive (false);
			if (currentInputMode == InputMode.MOVE)
				SetMode (lastInputMode);

			if (isDrawing && activeNode != null)
				EndDrawStroke ();
			selectedObject = null;
			rayHitRef.position = pointerRef.position + controllerPivot.transform.forward * 5;
		}
		if (activeStroke != null)
			activeStroke.UpdateBrushPos (rayHitRef);

	}




	/// <summary>
	/// Create a new stroke object and parent it to the pointer
	/// </summary>
	public override void OnButtonDown ()
	{
		if (!pview.isMine)
			return;
		//Debug.Log ("OnButtonDown");
		toolCollection [modeNum].SetToolAbility (true);

		if (activeNode != null && selectedObject != null && selectedObject.GetComponent<Node> () == activeNode || selectedObject == null && activeNode == null) {
			toolCollection [modeNum].SetMoveTarget (rayHitRef);
		

			switch (currentInputMode) {
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
		if (offline)
			activeStroke = (Instantiate (strokePrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<BrushGen> ();
		else
			activeStroke = PhotonNetwork.Instantiate (strokePrefab.name, Vector3.zero, Quaternion.identity, 0).GetComponent<BrushGen> ();
		activeStroke.gameObject.name = "activeStroke";
		if (activeNode != null) {
			activeStroke.photonView.RPC ("SetNetworkParent", PhotonTargets.AllBuffered, activeNode.transform.name);
			activeStroke.photonView.RPC ("SetMaterial", PhotonTargets.AllBuffered, 0);

		} else {
			drawingOnBackground = true;
			activeStroke.photonView.RPC ("SetMaterial", PhotonTargets.AllBuffered, 1);
		}
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
			activeStroke = null;
		}
	}


	void StartMove ()
	{
		if (selectedObject == null)
			return;
		activeMove = selectedObject.GetComponent<Node> ();
		if (!activeMove)
			return;
		if (!activeMove.photonView.isMine)
			activeMove.photonView.RequestOwnership ();
		activeMove.SetTarget (pointerRef);

	}

	void StopMove ()
	{
		activeMove.SetDesiredPosition (pointerRef.position);
		activeMove.resetPosition = pointerRef.position;
		activeMove.SetTarget (null);
		activeMove = null;
	}

	/// <summary>
	/// unparent the stroke object.
	/// </summary>
	public override void OnButtonUp ()
	{
		if (!pview.isMine)
			return;
		
		 

		if (activeMove != null) {
			StopMove ();
		} else {
			switch (currentInputMode) {
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
		toolCollection [modeNum].SetMoveTarget (ToolGuideAnchor);
		toolCollection [modeNum].SetToolAbility (false);
	}


	public override void OnSwipe (GVRSwipeDirection dir)
	{
		if (!pview.isMine)
			return;
		if (dir == GVRSwipeDirection.down) {
			if (selectedObject != null) {
				if (activeNode != null)
					CommitNode ();
				ActivateNode (selectedObject);
			} else {
				CreateNode ();
			}
		} else if (dir == GVRSwipeDirection.up) {
			CommitNode ();
		} else if (dir == GVRSwipeDirection.right) {
			
			if (activeMove != null) {
				return;
			}

			modeNum++;
			UpdateMode ();

			shouldDraw = !shouldDraw;
			DebugMessage ("should draw = " + shouldDraw);
		} else if (dir == GVRSwipeDirection.left) {
			modeNum--;
			UpdateMode ();
		}
		
		base.OnSwipe (dir);

	}

	void UpdateMode ()
	{
		if (modeNum < 0) {
			modeNum = selectableModeDict.Count - 1;
		}

		if (modeNum > selectableModeDict.Count - 1) {
			modeNum = 0;
		}

		SetMode (selectableModeDict [modeNum]);

		Debug.Log (currentInputMode);
	}


	// Universal set mode.
	public void SetMode (InputMode incMode)
	{
		modeNum = (int)incMode;
		currentInputMode = (InputMode)modeNum;
		TurnOnTool (modeNum);

	}

	// Turn on current tool and turn all others off.
	public void TurnOnTool (int incIndex)
	{
		for (int i = 0; i < toolCollection.Length; i++) {
			if (i == incIndex) {
				toolCollection [i].gameObject.SetActive (true);
			} else {
				toolCollection [i].gameObject.SetActive (false);
			}
		}
	}

	// If a tool is on and not out of index, tell it that it's being used and no longer idle.
	public void TriggerToolAbility (bool incBool)
	{
		if (modeNum >= toolCollection.Length)
			return;
		toolCollection [modeNum].SetToolAbility (incBool);
	}

	void ActivateNode (GameObject incNode)
	{
		
		activeNode = incNode.GetComponent<Node> ();
		if (!activeNode.photonView.isMine)
			activeNode.photonView.RequestOwnership ();
		Vector3 halfPoint = controllerPivot.transform.position + (controllerPivot.transform.forward * 11);
		activeNode.SetDesiredPosition (halfPoint);
		if (currentInputMode == InputMode.MOVE)
			SetMode (InputMode.DRAW);
	
	}

	void CreateNode ()
	{
		if (activeNode != null) {
			if (selectedObject != null) {
				Node selectedNode = selectedObject.GetComponent<Node> ();
				if (selectedNode != null && selectedNode == activeNode) {
					activeNode.photonView.RPC ("ClearContent", PhotonTargets.AllBuffered);
					activeNode = null;
					return;
				} else if (selectedNode != null && selectedNode != activeNode) {

					CommitNode ();
					ActivateNode (selectedObject);
					return;

				}
			}
			CommitNode ();
		}
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
		if (!activeNode.photonView.isMine)
			activeNode.photonView.RequestOwnership ();
		DebugMessage ("commiting Node");
		activeNode.SetDesiredPosition (activeNode.resetPosition);
		activeNode = null;

	}

	public override void AppButtonDown ()
	{
		if (!pview.isMine)
			return;
		//UnityEngine.SceneManagement.SceneManager.LoadScene (0);
		if (activeNode != null)
			activeNode.photonView.RPC ("ClearContent", PhotonTargets.AllBuffered);
		else if (selectedObject != null)
			Destroy (selectedObject);
	}

	/// <summary>
	/// Starts the microphone.
	/// </summary>
	void StartMicrophone ()
	{
		TriggerToolAbility (true);
		Debug.Log ("Starting microphone");
		if (activeNode == null)
			CreateNode ();
		micWidget.ActivateMicrophone ();
		activeNode.beginSpeech ();
		sttWidget.OnTranscriptUpdated += OnTranscriptUpdated;
	}

	void OnTranscriptUpdated (string text)
	{
		Debug.Log ("OnTranscriptUpdated: " + text);
		activeNode.photonView.RPC ("updateTranscript", PhotonTargets.AllBuffered, text);
	}

	/// <summary>
	/// Stops the microphone.
	/// </summary>
	void StopMicrophone ()
	{
		Debug.Log ("Stopping microphone");
		TriggerToolAbility (false);
		micWidget.DeactivateMicrophone ();
		sttWidget.OnTranscriptUpdated -= OnTranscriptUpdated;
		activeNode.endSpeech ();
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (transform.rotation);
			stream.SendNext ((int)currentInputMode);
			stream.SendNext (toolCollection [modeNum].transform.InverseTransformPoint (toolCollection [modeNum].GetDesiredPosition));
		} else {
			wantedRotation = (Quaternion)stream.ReceiveNext ();
			int curTool = (int)stream.ReceiveNext ();
			toolCollection [curTool].SetMovePosition ((Vector3)stream.ReceiveNext ());
			if (curTool != modeNum) {
				SetMode ((InputMode)curTool);
			}

		}
	}

}
