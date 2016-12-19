using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Widgets;

public class Node : Photon.MonoBehaviour
{
	public float travelForce = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;

	public Text transcriptText;
	public Text speechPrompt;



	[HideInInspector]
	public Vector3 resetPosition;

	public MicrophoneWidget micWidget;
	public Transform micAnchor;

	private ComputeBitmap computeBitmap = new ComputeBitmap ();
	public PaintableObject paintableObject;

	[HideInInspector]
	public bool textureHasChanged = false;
	private byte[] texturePixelArray;

	void OnEnable ()
	{
		if (photonView.isMine) {
			string generatedName = "Node_" + randomID;
			photonView.RPC ("SetNetworkName", PhotonTargets.AllBuffered, generatedName);
		}
	}

	void Start ()
	{
		_myTransform = transform;
		resetPosition = transform.position;
		speechPrompt.enabled = false;
	}

	void PaintStroke (Vector2 uvCords)
	{

	}


	public Transform nodeTransform {
		get {
			return _myTransform;
		}
	}

	public void SetTarget (Transform target)
	{
		_targetTransform = target;
	}

	public void SetDesiredPosition (Vector3 incPos)
	{
		_desiredPosition = incPos;
	}

	[PunRPC]
	public void SetNetworkName (string name)
	{
		gameObject.name = name;
	}

	void FixedUpdate ()
	{
		//return;
		if (!photonView.isMine || _targetTransform == null && _desiredPosition == Vector3.zero)
			return;
		Vector3 force = Vector3.zero;
		if (_targetTransform == null)
			force = (_desiredPosition - transform.position) * (Time.deltaTime * travelForce);
		else
			force = (_targetTransform.position - transform.position) * (Time.deltaTime * travelForce);
		transform.Translate (force, Space.World);
		_myTransform.forward = Vector3.MoveTowards (_myTransform.forward, transform.position, 0.5f);

	}

	[PunRPC]
	public void ClearContent ()
	{
		if (transform.childCount < 3)
			Destroy (gameObject);
		for (int i = transform.childCount - 1; i > 1; i--) {
			Destroy (transform.GetChild (i).gameObject);
		}
	}

	public void beginSpeech ()
	{
		micWidget.ActivateMicrophone ();
	}

	[PunRPC]
	public void updateTranscript (string text)
	{
		if (speechPrompt.enabled)
			speechPrompt.enabled = false;

		transcriptText.text += text;
	}

	string randomID {
		get {
			int idInt = Random.Range (0, 1000000);
			return idInt.ToString ("0000000");
		}
	}

	public void endSpeech ()
	{
		micWidget.DeactivateMicrophone ();
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			stream.SendNext (textureHasChanged);
			if (textureHasChanged)
				texturePixelArray = (paintableObject.myRenderer.material.mainTexture as Texture2D).EncodeToPNG();
			stream.SendNext (texturePixelArray);


		} else {
			transform.position = (Vector3)stream.ReceiveNext ();
			transform.rotation = (Quaternion)stream.ReceiveNext ();
			textureHasChanged = (bool)stream.ReceiveNext ();
			texturePixelArray = (byte[])stream.ReceiveNext ();
			if (textureHasChanged)
				(paintableObject.myRenderer.material.mainTexture as Texture2D).LoadImage(texturePixelArray);


		}
	}

}
