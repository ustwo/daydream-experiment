using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour {
	public GameObject menuGO;
	private float angleTol = 0.2f;
	private bool menuIsActive = false;
	
	// Update is called once per frame
	void Update () {
		float angle =  Vector3.Angle(Vector3.down, transform.forward) / -90.0f + 1.0f;
		if (angle > angleTol) {
			if (!menuIsActive) {
				menuGO.SetActive (true);
				menuIsActive = true;
				Vector3 yRotation = transform.eulerAngles;
				yRotation.x = menuGO.transform.eulerAngles.x;
				yRotation.z = menuGO.transform.eulerAngles.z;
				menuGO.transform.eulerAngles = yRotation;
			}
		} else if (menuIsActive) {
			menuIsActive = false;
			menuGO.SetActive (false);
		}

	}
}
