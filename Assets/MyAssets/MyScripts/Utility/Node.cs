using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Node : MonoBehaviour {

	public Rigidbody myRigid;
	public float travelForce = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;

	public Text transcriptText;
	public Text speechPrompt;
	[HideInInspector]
	public Vector3 resetPosition;

	void Start(){
		_myTransform = transform;
		resetPosition = transform.position;
		speechPrompt.enabled = false;
		transcriptText.enabled = false;
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


	public void ClearContent(){
		for (int i = transform.childCount -1 ; i > 1; i--) {
			Destroy (transform.GetChild (i).gameObject);
		}
		transcriptText.text = "";
	}

	public void beginSpeech()
	{
		transcriptText.enabled = true;
		transcriptText.text = "";
		speechPrompt.enabled = true;
	}

	public void updateTranscript(string text)
	{
		if (speechPrompt.enabled)
			speechPrompt.enabled = false;
		
		transcriptText.text += text;
	}

	public void endSpeech()
	{
		

	}

}
