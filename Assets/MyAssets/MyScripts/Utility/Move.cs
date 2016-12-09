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
		transform.position = GetDesiredPosition;
		base.SetMoveTarget (incTarget);
	}

	public override void SetMovePosition (Vector3 incPos)
	{
		transform.position = GetDesiredPosition;
		base.SetMovePosition (incPos);
	}

}
