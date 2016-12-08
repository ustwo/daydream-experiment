using UnityEngine;
using System.Collections;

public class Move : Tool{


	
	public override void SetMoveTarget (Transform incTarget)
	{
		transform.position = incTarget.position;
		base.SetMoveTarget (incTarget);
	}
}
