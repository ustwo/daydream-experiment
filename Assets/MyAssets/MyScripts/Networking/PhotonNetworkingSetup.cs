using UnityEngine;
using System.Collections;

public class PhotonNetworkingSetup : Photon.MonoBehaviour {

	public bool offline = true;

	[Tooltip("This game object must be in the \"Resources\" folder")]
	public GameObject playerControlledPrefab;

	// Use this for initialization
	void Start(){
		if (!offline) {
			Debug.Log ("trying to connect to photon cloud");
			PhotonNetwork.ConnectUsingSettings ("v0.1");
		} else {
			Instantiate (playerControlledPrefab);
		}
	}

	void OnJoinedLobby  () {
		Debug.Log ("Joined Lobby, joining room");
		RoomOptions roomOp = new RoomOptions() { isVisible = false, maxPlayers = 20 };
	
		PhotonNetwork.JoinOrCreateRoom ("draw",roomOp,TypedLobby.Default);

	}
	
	public void OnJoinedRoom(){
		PhotonNetwork.Instantiate (playerControlledPrefab.name, Vector3.zero, Quaternion.identity, 0);
		Debug.Log ("Joined Room");
	}
}
