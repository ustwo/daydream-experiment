using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Recording : MonoBehaviour 
{

	public Renderer icon;

	private bool isEnabled = false;

	float currentTime = 0.0f;
	float previousTime = 0.0f;

	// Use this for initialization
	void Start () {
		icon.enabled = isEnabled;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime = Time.time;
		float diff = currentTime - previousTime;

		if(Math.Abs (diff) >= 0.5f) {
//			Debug.Log ("cur: " + currentTime + ", prev: " + previousTime + ", diff: " + diff);

			previousTime = currentTime;
			isEnabled = !isEnabled;
			icon.enabled = isEnabled;
		}
	}
}
