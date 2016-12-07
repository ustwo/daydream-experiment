using UnityEngine;
using System.Collections;

public class Move : Tool{


	
	public override void UpdateDesiredPostion (Transform incTarget)
	{
		transform.position = incTarget.position;
		base.UpdateDesiredPostion (incTarget);
	}
}
