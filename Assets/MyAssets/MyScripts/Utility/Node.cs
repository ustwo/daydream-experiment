using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Node : Photon.MonoBehaviour
{
	public float travelForce = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;

	public Renderer TextureRenderer;
	public Shader nodeShader;
	private Material myMaterial;
	private Texture2D myTexture;

	[HideInInspector]
	public Vector3 resetPosition;

	public MicWidget micWidget;
	public STTController sttWidget;

	private ComputeBitmap computeBitmap = new ComputeBitmap ();

	public GameObject recordingIndicator;
	public GameObject preloader;

	bool micIsListening = false;
	bool micIsRecording = false;

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

		recordingIndicator.SetActive (false);
		preloader.SetActive (false);
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

	void Update()
	{
//		Debug.Log ("listening: " + micIsListening + ", recording: " + micIsRecording);

		if(micIsRecording || micIsListening) {
			recordingIndicator.SetActive (true);
			if(preloader.GetActive ()) preloader.SetActive (false);
		} else {
			recordingIndicator.SetActive (false);
		}
	}

	void FixedUpdate ()
	{
		//return;
		if (!photonView.isMine || _targetTransform == null &&_desiredPosition == Vector3.zero )
			return;
		Vector3 force = Vector3.zero;
		if (_targetTransform == null)
			force = (_desiredPosition - transform.position) * (Time.deltaTime * travelForce);
		else
			force = (_targetTransform.position - transform.position) * (Time.deltaTime * travelForce);

		//myRigid.AddForce (force);
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
		preloader.SetActive (true);

		micWidget.ActivateMicrophone ();
//		sttWidget.sttIsListening += IsListening;
//		micWidget.micIsRecording += IsRecording;

		IsListening (true);
		IsRecording (true);
	}

	void IsListening(bool isListening)
	{
		micIsListening = isListening;
	}

	void IsRecording(bool isRecording)
	{
		micIsRecording = isRecording;
	}

	[PunRPC]
	string randomID {
		get {
			int idInt = Random.Range (0, 1000000);
			return idInt.ToString ("0000000");
		}
	}

	public void endSpeech ()
	{
		IsListening (false);
		IsRecording (false);

		StopCoroutine (DelayMic ());
		StartCoroutine (DelayMic ());
//		Debug.Log ("Delayed mic");
//		micWidget.DeactivateMicrophone ();
	}

	IEnumerator DelayMic()
	{
		yield return new WaitForSeconds (0.5F);
		micWidget.DeactivateMicrophone ();
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
