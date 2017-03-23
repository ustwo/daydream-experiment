using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMenuItem : MonoBehaviour {
	public InputMode mode;
	public int colorIndex;
	public float brushSize;

	float cooldown = 0;

	void Update(){
		if (cooldown > 0) {
			cooldown -= Time.deltaTime;
			transform.localScale = Vector3.MoveTowards (transform.localScale, Vector3.one *1.5f , Time.deltaTime);

		}
		else
			transform.localScale = Vector3.MoveTowards (transform.localScale, Vector3.one , Time.deltaTime);
			
	}

	public void HighLight(){
		cooldown = 0.1f;
		//transform.localScale = Vector3.one * 1.5f;
	}
	
}
