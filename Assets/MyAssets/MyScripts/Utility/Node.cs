using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using IBM.Watson.DeveloperCloud.Widgets;

public class Node : Photon.MonoBehaviour {

	public Rigidbody myRigid;
	public float travelForce = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;

	[HideInInspector]
	public Vector3 resetPosition;

	public MicrophoneWidget micWidget;
	public Transform micAnchor;

	void OnEnable(){
		if (photonView.isMine) {
			string generatedName = "Node_" + randomID;
			photonView.RPC ("SetNetworkName", PhotonTargets.AllBuffered, generatedName);
		}
	}

	void Start(){
		_myTransform = transform;
		resetPosition = transform.position;
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
	[PunRPC]
	public void SetNetworkName(string name){
		gameObject.name = name;
	}

	void FixedUpdate(){
		if (!photonView.isMine)
			return;
		Vector3 force = Vector3.zero;
		if(_targetTransform == null)
			force = (_desiredPosition - transform.position) * (Time.deltaTime * travelForce);
		else
			force = (_targetTransform.position - transform.position) * (Time.deltaTime * travelForce);

		myRigid.AddForce (force);
		
		_myTransform.forward = Vector3.MoveTowards (_myTransform.forward, transform.position, 0.5f);

	}

	[PunRPC]
	public void ClearContent(){
		if (transform.childCount < 3)
			Destroy (gameObject);
		for (int i = transform.childCount -1 ; i > 1; i--) {
			Destroy (transform.GetChild (i).gameObject);
		}
	}

	public void beginSpeech()
	{
		micWidget.ActivateMicrophone ();
	}
	[PunRPC]

	string randomID {
		get{
			int idInt = Random.Range (0, 1000000);
			return idInt.ToString ("0000000");
		}
	}

	public void endSpeech()
	{
		micWidget.DeactivateMicrophone ();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
		}
		else
		{
			transform.position = (Vector3)stream.ReceiveNext ();
			transform.rotation = (Quaternion)stream.ReceiveNext ();

		}
	}

}
