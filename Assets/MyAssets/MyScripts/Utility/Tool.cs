using UnityEngine;
using System.Collections;

public class Tool : MonoBehaviour
{

	[HideInInspector]
	public Transform target;
	protected Vector3 targetPosition;
	protected bool isActive = false;

	public virtual void OnEnable ()
	{
		target = null;
		targetPosition = transform.parent.position;
	}

	public virtual void SetMoveTarget (Transform incTarget)
	{
		target = incTarget;
	}

	public virtual void SetMovePosition (Vector3 incPos)
	{
		target = null;
		targetPosition = incPos;
	}

	public Vector3 GetDesiredPosition {
		get {
			if (target != null)
				return target.position;
			return targetPosition;
		}
	}

	public virtual void SetToolAbility (bool incBool)
	{
		isActive = incBool;
	}
}
