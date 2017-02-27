using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerComponentControl : NetworkBehaviour {

	public MonoBehaviour[] classes;
	public GameObject[] objects;


	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			for (int i = 0; i < objects.Length; i++) {
				objects [i].SetActive (true);
			}
			for (int i = 0; i < classes.Length; i++) {
				classes [i].enabled = true;
			}
		}
	}
	

}
