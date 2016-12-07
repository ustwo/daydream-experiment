using UnityEngine;
using System.Collections;

public class Tool : MonoBehaviour {

	protected Transform target;
	protected bool isActive = false;
	public virtual void UpdateDesiredPostion(Transform incTarget){
		target = incTarget;
	}
	public virtual void SetIsActive(bool incBool){
		isActive = incBool;
	}
}
