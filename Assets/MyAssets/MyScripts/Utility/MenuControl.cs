using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour {
	public GameObject menuGO;
	private float angleTol = 0.3f;
	private bool menuIsActive = false;
	public BlackOutFader bgFader;

	
	// Update is called once per frame
	void Update () {
		float angle =  Vector3.Angle(Vector3.down, transform.forward) / -90.0f + 1.0f;
		if (angle > angleTol) {
			if (!menuIsActive) {
				menuGO.SetActive (true);
				menuIsActive = true;
				bgFader.FadeIn ();
//				Vector3 yRotation = transform.eulerAngles;
//				yRotation.x = menuGO.transform.eulerAngles.x;
//				yRotation.z = menuGO.transform.eulerAngles.z;
//				menuGO.transform.eulerAngles = yRotation;
			}
		} else if (menuIsActive) {
			bgFader.FadeOut ();
			menuIsActive = false;
			menuGO.SetActive (false);
		}

	}

	public void  Interrupt(){
		bgFader.FadeOut ();
		menuIsActive = false;
		menuGO.SetActive (false);
	}
}
