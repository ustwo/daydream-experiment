using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Node : MonoBehaviour {

	public Rigidbody myRigid;
	public float travelForce = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;


	void Start(){
		_myTransform = transform;
	}

	public Transform nodeTransform{
		get{
			return _myTransform;
		}
	}
	public void SetTarget(Transform target){
		_targetTransform = target;
	}

	public void SetDesiredPosition(Vector3 incPos){
		_desiredPosition = incPos;
	}

	void FixedUpdate(){
		Vector3 force = Vector3.zero;
		if(_targetTransform == null)
			force = (_desiredPosition - transform.position) * (Time.deltaTime * travelForce);
		else
			force = (_targetTransform.position - transform.position) * (Time.deltaTime * travelForce);

		myRigid.AddForce (force);
		
		_myTransform.forward = Vector3.MoveTowards (_myTransform.forward, transform.position, 0.5f);

	}

}
