using UnityEngine;
using System.Collections;

public class PhotonNetworkingSetup : Photon.MonoBehaviour {

	// Use this for initialization
	void Start(){
		Debug.Log ("trying to connect to photon cloud");
		PhotonNetwork.ConnectUsingSettings ("v0.1");
	}

	void OnJoinedLobby  () {
		Debug.Log ("Joined Lobby, joining room");
		RoomOptions roomOp = new RoomOptions() { isVisible = false, maxPlayers = 20 };
	
		PhotonNetwork.JoinOrCreateRoom ("draw",roomOp,TypedLobby.Default);

	}
	
	public void OnJoinedRoom(){
		PhotonNetwork.Instantiate ("pointerPrefab", Vector3.zero, Quaternion.identity, 0);
		Debug.Log ("Joined Room");
	}
}
