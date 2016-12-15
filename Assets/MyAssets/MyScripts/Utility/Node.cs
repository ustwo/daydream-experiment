using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Node : Photon.MonoBehaviour
{

	public Rigidbody myRigid;
	public float travelForce = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;

	public Text transcriptText;
	public Text speechPrompt;

	public Renderer TextureRenderer;
	public Shader nodeShader;
	private Material myMaterial;
	private Texture2D myTexture;
	public Texture2D brush;

	[HideInInspector]
	public Vector3 resetPosition;

	private ComputeBitmap computeBitmap = new ComputeBitmap();

	void OnEnable ()
	{
		if (photonView.isMine) {
			string generatedName = "Node_" + randomID;
			photonView.RPC ("SetNetworkName", PhotonTargets.AllBuffered, generatedName);
		}
	}

	void Start ()
	{
		myMaterial = new Material (nodeShader);
		myTexture = new Texture2D (500, 500);
		Color[] textureColors = new Color[500 * 500];
		Color blankColor = Color.black;
		for (int i = 0; i < textureColors.Length; i++) {
			textureColors [i] = blankColor;
		}
		myTexture.SetPixels (textureColors);
		myTexture.Apply ();
		myMaterial.mainTexture = myTexture;
		TextureRenderer.material = myMaterial;
		_myTransform = transform;
		resetPosition = transform.position;
		speechPrompt.enabled = false;
		//transcriptText.enabled = false;
	//	myMaterial.mainTexture = computeBitmap.ComputeBitMap(myTexture,brush) as Texture;
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
		return;
		if (!photonView.isMine)
			return;
		Vector3 force = Vector3.zero;
		if (_targetTransform == null)
			force = (_desiredPosition - transform.position) * (Time.deltaTime * travelForce);
		else
			force = (_targetTransform.position - transform.position) * (Time.deltaTime * travelForce);

		myRigid.AddForce (force);
		
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
		transcriptText.text = "";
	}

	public void beginSpeech ()
	{
		transcriptText.enabled = true;
		transcriptText.text = "";
		speechPrompt.enabled = true;
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
		

	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
		} else {
			transform.position = (Vector3)stream.ReceiveNext ();
			transform.rotation = (Quaternion)stream.ReceiveNext ();

		}
	}

}
