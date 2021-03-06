﻿using UnityEngine;
using System.Collections;

public class Pen : Tool
{

	// the speed at which the pen travels to its desired destination
	public float moveSpeed = 10f;

	// how long before it just pops into place
	private float popInPlaceTime = 0;

	// did it pop in place?
	private bool poppedInPlace = true;

	public PainBrush paintBrush;

	public Color[] brushColors;
	private int currentBrushColorIndex = 0;

	public Material penMat;
	public Transform rayHit;

	void Awake(){
		ButtonOptionRB ();
	}

	public override void OnEnable ()
	{
		base.OnEnable ();
		transform.localPosition = Vector3.zero;
		rayHit.localScale = Vector3.one * (0.05f * paintBrush.brushSize);

	}


	public override void Update ()
	{
		base.Update ();

		// If popped in place, dont continue
		if (poppedInPlace)
			return;

		// if transition time left over, move object to deisred destination. 
		Vector3 adjustedPosition =	GetDesiredPosition;
		adjustedPosition.x = 0;
		adjustedPosition.y = 0;
		if (popInPlaceTime > 0) {
			transform.localPosition = Vector3.MoveTowards (transform.localPosition, adjustedPosition, moveSpeed * Time.deltaTime);
			popInPlaceTime -= Time.deltaTime;
		} else {
			transform.localPosition = adjustedPosition;
			poppedInPlace = true;
		}
	
	}

	public override void SetToolAbility (bool incBool)
	{
		// check to make sure this object is turned on
		if (!gameObject.activeSelf)
			return;

		//Debug.Log ("pen On");
		//set a transition time 
		//popInPlaceTime = 1f;

		// enable commands in update
		//poppedInPlace = false;

		// activate the paint brush class
		paintBrush.SetBrush (incBool);

		// set on base
		base.SetToolAbility (incBool);
	}

	public override void SetMovePosition (Vector3 incPos)
	{
		Vector3 adjustedPosition = GetDesiredPosition;
		adjustedPosition.x = 0;
		adjustedPosition.y = 0;
		transform.localPosition = adjustedPosition;
		base.SetMovePosition (incPos);
	}

	public override void ButtonOpetionLT ()
	{
		paintBrush.brushSize += 5;
		rayHit.localScale = Vector3.one * (0.05f * paintBrush.brushSize);
		paintBrush.Init ();
		base.ButtonOpetionLT ();
	}

	public override void ButtonOptionLB ()
	{
		if (paintBrush.brushSize > 5) {
			paintBrush.brushSize -= 5;
			rayHit.localScale = Vector3.one * (0.05f * paintBrush.brushSize);
			paintBrush.Init ();
		}
		base.ButtonOptionLB ();
	}

	public override void ButtonOptionRB ()
	{
		currentBrushColorIndex++;
		if (currentBrushColorIndex == brushColors.Length)
			currentBrushColorIndex = 0;
		penMat.color = brushColors [currentBrushColorIndex];
		paintBrush.color = brushColors [currentBrushColorIndex];
		base.ButtonOptionRB ();
	}

	public override void ButtonOptionRT ()
	{
		currentBrushColorIndex--;
		if (currentBrushColorIndex == -1)
			currentBrushColorIndex = brushColors.Length - 1;
		penMat.color = brushColors [currentBrushColorIndex];
		paintBrush.color = brushColors [currentBrushColorIndex];
		base.ButtonOptionRT ();
	}

	public Color GetBrushColor()
	{
		return brushColors [currentBrushColorIndex];
	}

	public override void SetColor(int col){
		penMat.color = brushColors [col];
		paintBrush.color = brushColors [col];

	}
	public override void SetScale(float scale){
		paintBrush.brushSize = (int)scale;
		rayHit.localScale = Vector3.one * (scale *0.01f) * paintBrush.brushSize;
	}

}
