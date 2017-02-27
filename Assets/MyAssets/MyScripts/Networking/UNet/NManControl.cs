using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NManControl : MonoBehaviour {

	public NetworkManager manager;
	private NetworkConnection conn;
	public bool isClient = false;

	void Start(){
		if (isClient)
			manager.StartClient ();
		else {
			manager.StartHost ();
		}
	}
	void OnFailedToConnect(NetworkConnectionError error){
		Debug.Log ("Failed to connect to host, creating one: " + error.ToString ());
		manager.StartHost ();

	}

	void OnStartClient (){
		Debug.Log ("client connected");
	}

	void OnStartHost(){
		Debug.Log ("Host Created");
	}






//	public void OnStartClient() {
//		
//		manager.StartMatchMaker ();
//
//		StartCoroutine (joinOrCreate ());
//	}
//
//	public IEnumerator joinOrCreate() {
//		//waits for response from list matches
//		yield return new WaitForSeconds (5f);
//		Debug.Log("Done waiting");
//		if (manager.matchInfo == null) {
//			Debug.Log("Join or create?");
//			if (manager.matches == null || manager.matches.Count == 0) {
//				Debug.Log("Create");
//				manager.matchMaker.CreateMatch(manager.matchName,6,true,"","","",0,0, OnJoinedMatch);
//				//manager.matchMaker.CreateMatch (manager.matchName, manager.matchSize, true, "", manager.OnMatchCreate);
//			} else {
//				Debug.Log ("Joining");
//				manager.matchMaker.JoinMatch (manager.matches[0].networkId,"","","",0,0,OnJoinedMatch);
//			}
//		}
//	}
//		
////	void OnClientConnect(NetworkConnection con){
//	//	conn = con;
////	}
//
//	//void OnServerConnect(NetworkConnection con){
//	//	conn = con;
//	//}
//
//	void OnJoinedMatch (bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo responseData)
//	{
//		
//
//		Debug.Log ("Sucessful? " + success);
//		Debug.Log (extendedInfo);
//		if (success) {
//			ClientScene.Ready (ClientScene.readyConnection);
//			ClientScene.AddPlayer (ClientScene.readyConnection, 0);
//		}
//		//ClientScene.AddPlayer (1);
//	}				


		
}
