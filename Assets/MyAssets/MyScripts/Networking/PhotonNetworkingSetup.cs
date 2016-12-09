using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhotonNetworkingSetup : Photon.MonoBehaviour
{

	public bool offline = true;
	private Text debugLabel;

	[Tooltip ("This game object must be in the \"Resources\" folder")]
	public GameObject playerControlledPrefab;

	public void Awake ()
	{
		
		if (GameObject.FindWithTag ("debug") != null) {
			debugLabel = GameObject.FindWithTag ("debug").GetComponent<Text> ();
			debugLabel.text = "Good to go";
		}
	}

	// Use this for initialization
	void Start ()
	{
		if (!offline) {
			Debug.Log ("trying to connect to photon cloud");
			PhotonNetwork.ConnectUsingSettings ("v0.2");
		} else {
			Instantiate (playerControlledPrefab);
		}
	}

	void OnJoinedLobby ()
	{
		DebugMessage ("\nJoined Lobby", false);
		Debug.Log ("Joined Lobby, joining room");
		RoomOptions roomOp = new RoomOptions () { isVisible = false, maxPlayers = 20 };
		PhotonNetwork.JoinOrCreateRoom ("draw", roomOp, TypedLobby.Default);

	}

	public void OnJoinedRoom ()
	{
		PhotonNetwork.Instantiate (playerControlledPrefab.name, Vector3.zero, Quaternion.identity, 0);
		DebugMessage ("\nJoined Room, Playercount = " + PhotonNetwork.playerList.Length.ToString (), false);
		Debug.Log ("Joined Room");
		Debug.Log ("Player Count " + PhotonNetwork.playerList.Length.ToString ());
	}

	/// Debug Messages
	protected void DebugMessage (string msg, bool isNew)
	{
		if (debugLabel == null)
			return;
		if (isNew)
			debugLabel.text = msg;
		else
			debugLabel.text += msg;
	}
}
