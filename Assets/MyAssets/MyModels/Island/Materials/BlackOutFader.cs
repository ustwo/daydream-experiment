using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOutFader : MonoBehaviour {

	private bool isVisiable = false;
	public Material fadeOutMat;
	public float fadeSpeed = 2f;

	public void Start(){
		FadeOut ();
	}
	
	public void FadeOut(){
		isVisiable = false;
	}

	public void FadeIn(){
		isVisiable = true;
	}

	void Update(){
		Color ajustedColor = fadeOutMat.color;

		if (isVisiable)
			ajustedColor.a = Mathf.MoveTowards (ajustedColor.a, 0.75f, Time.deltaTime);
		else
			ajustedColor.a = Mathf.MoveTowards (ajustedColor.a, 0, Time.deltaTime);
		
		fadeOutMat.color = ajustedColor;
	}
}
