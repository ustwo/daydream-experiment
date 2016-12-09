using UnityEngine;
using System.Collections;

public class Move : Tool
{

	private float showupDelay = 0.25f;
	public Renderer myRenderer;

	public override void OnEnable ()
	{
		myRenderer.enabled = false;
		base.OnEnable ();
		Invoke ("ActivateSelf", showupDelay);
	}

	public void ActivateSelf ()
	{
		if (gameObject.activeSelf)
			myRenderer.enabled = true;
	}

	public void OnDisable ()
	{
		myRenderer.enabled = false;
	}

	
	public override void SetMoveTarget (Transform incTarget)
	{
		base.SetMoveTarget (incTarget);
		transform.position = GetDesiredPosition;
	}

	public override void SetMovePosition (Vector3 incPos)
	{
		base.SetMovePosition (incPos);
		Vector3 adjustedMovePosition = GetDesiredPosition;
		adjustedMovePosition.x = 0;
		adjustedMovePosition.y = 0;
		transform.localPosition = adjustedMovePosition;
	}

}
