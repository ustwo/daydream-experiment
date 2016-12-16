using UnityEngine;
using System.Collections;

public class Pen : Tool {

	// the speed at which the pen travels to its desired destination
	public float moveSpeed = 10f;

	// how long before it just pops into place
	private float popInPlaceTime = 0;

	// did it pop in place?
	private bool poppedInPlace = true;

	public PainBrush paintBrush;

	public override void OnEnable(){
		base.OnEnable();
		transform.localPosition = Vector3.zero;
	}


	void Update(){

		// If popped in place, dont continue
		if (poppedInPlace)
			return;

		// if transition time left over, move object to deisred destination. 
		Vector3 adjustedPosition =	GetDesiredPosition;
		adjustedPosition.x = 0;
		adjustedPosition.y = 0;
		if (popInPlaceTime > 0) {
			transform.localPosition = Vector3.MoveTowards (transform.localPosition, adjustedPosition ,moveSpeed * Time.deltaTime);
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

		// set a transition time 
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

}
