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

	public virtual void Update(){
		if (GvrController.ClickButtonDown || Input.GetMouseButtonDown(0)) {
			Debug.Log ("clicked the button");
			SetToolAbility (true);
		} else if (GvrController.ClickButtonUp || Input.GetMouseButtonUp(0)){
			Debug.Log ("unclicked the button");
			SetToolAbility (false);
		}
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
	public virtual void ButtonOpetionLT(){

	}
	public virtual void ButtonOptionLB(){

	}
	public virtual void ButtonOptionRT(){

	}
	public virtual void ButtonOptionRB(){

	}
}
