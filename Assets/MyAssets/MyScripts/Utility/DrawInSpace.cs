/// <summary>
/// Allows the user to draw a shape in space using the daydream pointer.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//using IBM.Watson.DeveloperCloud.Widgets;
public enum InputMode
{
	DRAW = 0,
	MICROPHONE = 1,
	MOVE = 2
}

public class DrawInSpace : GVRInput
{

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
	public Transform endOfLineRef;


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


	private InputMode currentInputMode;
	private InputMode lastInputMode;

	private int modeNum = 0;
	private Dictionary<int, InputMode> selectableModeDict;

	private Quaternion wantedRotation;

	public float rotationSpeed = 10f;

	//	Anchor for the mic
	public GameObject micAnchor;

	private LineRenderer lineRenderer;

	public TouchGuide touchGuide;

	private AudioSource source;
	public AudioClip micOn;
	public AudioClip micOff;

	public ToolMenuItem activeMenuItem;
	public 	MenuControl menuControl;


	// Use this for initialization
	public override void Awake ()
	{
		DontDestroyOnLoad (gameObject);
		base.Awake ();
	}

	public void Start ()
	{
		//	micAnchor = GameObject.FindGameObjectWithTag ("MicCameraAnchor");
		currentInputMode = InputMode.DRAW;
		lineRenderer = gameObject.GetComponent<LineRenderer> ();
		source = gameObject.GetComponent<AudioSource> ();
		wantedRotation = transform.rotation;
		selectableModeDict = new Dictionary<int, InputMode> ();
		selectableModeDict.Add (0, InputMode.DRAW);
		selectableModeDict.Add (1, InputMode.MICROPHONE);
		SetMode (InputMode.DRAW);
		rayHitRef.parent = null;
		rayHitRef.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		

		//touchGuide.UpdateIndicatorPosition (touchPosition);
		
		base.Update ();

		// For Debuging, manually trigger the buttons.
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			OnSwipe (GVRSwipeDirection.down);
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			OnSwipe (GVRSwipeDirection.up);
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			OnSwipe (GVRSwipeDirection.right);
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			OnSwipe (GVRSwipeDirection.left);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			AppButtonDown ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			ButtonOpetionLT ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			ButtonOptionLB ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			ButtonOptionRT ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha8)) {
			ButtonOptionRB ();
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
		if (Input.GetKeyDown (KeyCode.M)) {
			SetMode (InputMode.MICROPHONE);
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			SetMode (InputMode.DRAW);
		}

			

		RaycastHit hit;
		if (Physics.Linecast (controllerPivot.transform.position, endOfLineRef.position + controllerPivot.transform.forward, out hit, detectionMask) && !drawingOnBackground) {

			if (currentInputMode == InputMode.MICROPHONE && activeNode != null)
				lineRenderer.enabled = false;
			else {
				lineRenderer.enabled = true;
				lineRenderer.SetPosition (1, hit.point);
			}

			selectedObject = hit.collider.gameObject;



			if (activeNode == null && selectedObject.GetComponent<Node> () != null) {
				rayHitRef.position = hit.point + (hit.normal).normalized * 0.5f;
				rayHitRef.forward = hit.normal;
				if (currentInputMode != InputMode.MOVE)
					lastInputMode = currentInputMode;
				toolCollection [(int)InputMode.MOVE].SetMoveTarget (rayHitRef);
				SetMode (InputMode.MOVE);

			} else if (selectedObject.GetComponent<ToolMenuItem> () != null) {
				ToolMenuItem toolItem = selectedObject.GetComponent<ToolMenuItem> ();
				toolItem.HighLight ();
				activeMenuItem = toolItem;
				SetMode (toolItem.mode);

			}

			rayHitRef.gameObject.SetActive (true);


		} else {
			activeMenuItem = null;
			lineRenderer.SetPosition (1, endOfLineRef.position);
			rayHitRef.position = endOfLineRef.position + controllerPivot.transform.forward * 5;
			if (activeMove == null) {
				rayHitRef.gameObject.SetActive (false);
				if (currentInputMode == InputMode.MOVE)
					SetMode (lastInputMode);

				if (isDrawing && activeNode != null)
					EndDrawStroke ();
				selectedObject = null;

			}
		}

		if (activeStroke != null)
			activeStroke.UpdateBrushPos (rayHitRef);

		Color brushColor = toolCollection [(int)InputMode.DRAW].GetComponent<Pen> ().GetBrushColor ();
		lineRenderer.material.color = brushColor;

	}


	void SelecteToolMenuItem (ToolMenuItem menuItem)
	{
		switch (menuItem.mode) {
		case(InputMode.DRAW):
			SetMode (InputMode.DRAW);
			toolCollection [modeNum].SetColor (menuItem.colorIndex);
			toolCollection [modeNum].SetScale (menuItem.brushSize);
			break;
		case(InputMode.MICROPHONE):
			break;
		}
	}


	/// <summary>
	/// Create a new stroke object and parent it to the pointer
	/// </summary>
	public override void OnButtonDown ()
	{
		if (activeMenuItem != null) {
			SelecteToolMenuItem (activeMenuItem);
			menuControl.Interrupt ();
			return;

		}
		//Debug.Log ("OnButtonDown");
		toolCollection [modeNum].SetToolAbility (true);
		// If we have a note and it is the one we are pointing at or we dont have have an active note and we are not pointing at one?
		if (activeNode != null && selectedObject != null && selectedObject.GetComponent<Node> () == activeNode
		    || selectedObject == null && activeNode != null) {

		

			switch (currentInputMode) {
			case InputMode.DRAW:
				toolCollection [modeNum].SetMoveTarget (rayHitRef);
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


	}

	void EndDrawStroke ()
	{
		if (activeNode == null || !activeNode.textureHasChanged)
			return;
		activeNode.paintableObject.SendTexture ();


	}


	void StartMove ()
	{
		if (selectedObject == null)
			return;

		Cmd_AuthorityRequest_OnHostFromClient (selectedObject.GetComponent<NetworkIdentity> ().netId);
	
		activeMove = selectedObject.GetComponent<Node> ();
		if (!activeMove)
			return;
		activeMove.SetTarget (endOfLineRef);

	}

	void StopMove ()
	{
		activeMove.SetDesiredPosition (endOfLineRef.position);
		activeMove.resetPosition = endOfLineRef.position;
		activeMove.SetTarget (null);
		activeMove = null;
	}

	/// <summary>
	/// unparent the stroke object.
	/// </summary>
	public override void OnButtonUp ()
	{
		GVRInput.DebugMessage ("OnButtonUp:");
		if (activeMove != null) {
			GVRInput.DebugMessage ("OnButtonUp: activeMove is not null, stopping move");
			StopMove ();
		} else {
			switch (currentInputMode) {
			case InputMode.DRAW:
				EndDrawStroke ();
				break;
			case InputMode.MICROPHONE:
				GVRInput.DebugMessage ("OnButtonUp: Stopping mic");
				StopMicrophone ();
				//CommitNode ();
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
		if (dir == GVRSwipeDirection.down) {
			if (selectedObject != null) {
				if (activeNode != null)
					CommitNode ();
				ActivateNode (selectedObject);
			} else {
				CreateNode ();
			}
		} else if (dir == GVRSwipeDirection.up) {
			GVRInput.DebugMessage ("swipe up");
			CommitNode ();
		} else if (dir == GVRSwipeDirection.right) {
			
			if (activeMove != null) {
				return;
			}

			//	modeNum++;
			//	UpdateMode ();

			shouldDraw = !shouldDraw;
	
		} else if (dir == GVRSwipeDirection.left) {
			//	modeNum--;
			//		UpdateMode ();
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

		//	Debug.Log (currentInputMode);
	}


	// Universal set mode.
	public void SetMode (InputMode incMode)
	{
		if (currentInputMode != InputMode.MOVE)
			lastInputMode = currentInputMode;
		modeNum = (int)incMode;
		currentInputMode = (InputMode)modeNum;
		TurnOnTool (modeNum);

		if (currentInputMode == InputMode.MICROPHONE) {
			lineRenderer.enabled = false;
		} else {
			lineRenderer.enabled = true;
		}
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
		if (incNode == null)
			return;
		GVRInput.DebugMessage ("ActivateNode:");
		Cmd_AuthorityRequest_OnHostFromClient (incNode.GetComponent<NetworkIdentity> ().netId);
		activeNode = incNode.GetComponent<Node> ();
		Vector3 halfPoint = controllerPivot.transform.position + (controllerPivot.transform.forward * 11);
		activeNode.SetDesiredPosition (halfPoint);
		if (currentInputMode == InputMode.MOVE)
			SetMode (lastInputMode);
	
	}

	void CreateNode ()
	{
		if (activeNode != null) {
			if (selectedObject != null) {
				Node selectedNode = selectedObject.GetComponent<Node> ();

				// Return if we are already in edit mode with the node. 
				if (selectedNode != null && selectedNode == activeNode) { 
					return;
				} 
				// If  We are in edit mode but we are trying to create a node, commit the one we have first. 
				else if (selectedNode != null && selectedNode != activeNode) {
					debugLabel.text = " CreateNode: selectedNode is null or selectedNode is not active Node";
					CommitNode ();
					CmdSpawnNode (endOfLineRef.position, Quaternion.identity);
					return;

				}
			}
			GVRInput.DebugMessage (" CreateNode: selectedObject is null ");
			CommitNode ();
		}
		CmdSpawnNode (endOfLineRef.position, Quaternion.identity);
	}

	void DelayedActivation ()
	{
		ActivateNode (selectedObject);
	}

	[Command]
	void CmdSpawnNode (Vector3 position, Quaternion rotation)
	{
		GameObject newNode = Instantiate (nodePrefab, position, rotation)as GameObject;
		NetworkServer.SpawnWithClientAuthority (newNode, connectionToClient);
		Target_assignNode (connectionToClient, newNode.GetComponent<NetworkIdentity>().netId);
	}

	[TargetRpc] 
	public void Target_assignNode (NetworkConnection target, NetworkInstanceId objectID)
	{
		ActivateNode(ClientScene.FindLocalObject(objectID));
	}

	[Command] 
	void Cmd_AuthorityRequest_OnHostFromClient (NetworkInstanceId netID)
	{
		Debug.LogWarning ("trying to transfer ownership for server");
		NetworkIdentity selectedNetworkAuthority = NetworkServer.FindLocalObject (netID).GetComponent<NetworkIdentity> ();
		if (selectedNetworkAuthority.clientAuthorityOwner != null)
			selectedNetworkAuthority.RemoveClientAuthority (selectedNetworkAuthority.clientAuthorityOwner);
		selectedNetworkAuthority.AssignClientAuthority (connectionToClient);
	}

	[Command]
	void Cmd_RemoveAuthority (NetworkInstanceId netID)
	{
		NetworkIdentity selectedNetworkAuthority = NetworkServer.FindLocalObject (netID).GetComponent<NetworkIdentity> ();
		selectedNetworkAuthority.RemoveClientAuthority (selectedNetworkAuthority.clientAuthorityOwner);
	}

	[ClientRpc] 
	public void Rpc_AuthorityRequest_OnClientFromHost (NetworkInstanceId netID)
	{
		NetworkIdentity selectedNetworkAuthority = ClientScene.FindLocalObject (netID).GetComponent<NetworkIdentity> ();
		Debug.LogWarning ("Trying to transfer ownership from client");
		if (selectedNetworkAuthority.hasAuthority)
			return;
		selectedNetworkAuthority.RemoveClientAuthority (selectedNetworkAuthority.connectionToClient);
		selectedNetworkAuthority.AssignClientAuthority (connectionToClient);
	}





	void CommitNode ()
	{
		GVRInput.DebugMessage (" Commiting Node");
		if (isDrawing)
			EndDrawStroke ();
		if (activeNode == null) {
			GVRInput.DebugMessage ("CommitNode: have no active node");
			return;
		}
		GVRInput.DebugMessage (" Commited Note");
		activeNode.SetDesiredPosition (activeNode.resetPosition);
		activeNode = null;

	}

	public override void AppButtonDown ()
	{
		if (selectedObject != null)
			Destroy (selectedObject);
	}

	/// <summary>
	/// Starts the microphone.
	/// </summary>
	void StartMicrophone ()
	{
		GVRInput.DebugMessage ("StartMicrophone:");
		TriggerToolAbility (true);
		if (activeNode == null)
			return; //CreateNode ();
		Debug.LogWarning ("Mode Number " + modeNum );
		Debug.LogWarning ("  micAnchor " + micAnchor.name);
		Debug.LogWarning (" nodename " + activeNode.name);
		toolCollection [modeNum].SetMoveTarget (micAnchor.transform);
		activeNode.beginSpeech ();
	//	source.PlayOneShot (micOn);
	}


	/// <summary>
	/// Stops the microphone.
	/// </summary>
	void StopMicrophone ()
	{
		GVRInput.DebugMessage ("StopMicrophone:");
		TriggerToolAbility (false);
		if (activeNode == null)
			return; //CreateNode ();		
		toolCollection [modeNum].SetMoveTarget (ToolGuideAnchor);
//		source.PlayOneShot (micOff);
//		GVRInput.DebugMessage ("Stopping the Mic");
		activeNode.endSpeech ();
	}



	public override void ButtonOpetionLT ()
	{
		toolCollection [(int)currentInputMode].ButtonOpetionLT ();
		base.ButtonOpetionLT ();
	}

	public override void ButtonOptionLB ()
	{
		toolCollection [(int)currentInputMode].ButtonOptionLB ();
		base.ButtonOptionLB ();
	}

	public override void ButtonOptionRB ()
	{
		toolCollection [(int)currentInputMode].ButtonOptionRB ();
		base.ButtonOptionRB ();
	}

	public override void ButtonOptionRT ()
	{
		toolCollection [(int)currentInputMode].ButtonOptionRT ();
		base.ButtonOptionRT ();
	}

}
