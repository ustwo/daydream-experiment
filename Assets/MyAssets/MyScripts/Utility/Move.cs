using UnityEngine;
using System.Collections;

public class Move : Tool
{


	
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
