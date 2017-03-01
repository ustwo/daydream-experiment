using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
	public float travelSpeed = 5f;
	private Vector3 _desiredPosition = Vector3.zero;
	private Transform _myTransform;
	private Transform _targetTransform;

	public Text transcriptText;
	public Text speechPrompt;


	public Renderer TextureRenderer;
	//public Shader nodeShader;
	private Material myMaterial;
	private Texture2D myTexture;

	[HideInInspector]
	public Vector3 resetPosition;

	//public MicWidget micWidget;
	//public STTController sttWidget;

	private ComputeBitmap computeBitmap = new ComputeBitmap ();
	public PaintableObject paintableObject;

	[HideInInspector]
	public bool textureHasChanged = false;
	private byte[] texturePixelArray;

	public GameObject recordingIndicator;
	public GameObject preloader;

	bool micIsListening = false;
	bool micIsRecording = false;

	public LayerMask nodeMask;

	void OnEnable ()
	{
		//if (photonView.isMine) {
			string generatedName = "Node_" + randomID;
			transform.forward = Vector3.zero - transform.position;
			//photonView.RPC ("SetNetworkName", PhotonTargets.AllBuffered, generatedName);
		//}
	}

	void Start ()
	{
		_myTransform = transform;
		resetPosition = transform.position;


		speechPrompt.enabled = false;

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


	public void SetNetworkName (string name)
	{
		gameObject.name = name;
	}

	void Update()
	{
//		Debug.Log ("listening: " + micIsListening + ", recording: " + micIsRecording);

//		if(micIsRecording || micIsListening) {
//			recordingIndicator.SetActive (true);
//			if(preloader.GetActive ()) preloader.SetActive (false);
//		} else {
//			if(recordingIndicator != null) recordingIndicator.SetActive (false);
//		}
	}

	void FixedUpdate ()
	{
		//return;
		if (_targetTransform == null && _desiredPosition == Vector3.zero)
			return;
		Vector3 force = Vector3.zero;
		if (_targetTransform == null)
			force = (_desiredPosition - transform.position) * (Time.deltaTime * travelSpeed);
		else
			force = (_targetTransform.position - transform.position) * (Time.deltaTime * travelSpeed);
		transform.Translate (force, Space.World);
		Collider[] objectsInTheWay = Physics.OverlapSphere (transform.position, 5,nodeMask);
		if (objectsInTheWay.Length>1) {
			for (int i = 0; i < objectsInTheWay.Length; i++) {
				if (objectsInTheWay [i].gameObject != gameObject) {
					_desiredPosition = transform.position - (objectsInTheWay [i].transform.position - transform.position);
				}
			}
		}
		_myTransform.forward = Vector3.MoveTowards (_myTransform.forward, _myTransform.position, 0.5f);
		if (Vector3.Distance (_myTransform.position, Vector3.zero) > 54) {
			_desiredPosition = _myTransform.position + _myTransform.forward*-1;
		}
	}


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

		//micWidget.ActivateMicrophone ();

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

	}

	IEnumerator DelayMic()
	{
		yield return new WaitForSeconds (0.5F);
		//micWidget.DeactivateMicrophone ();
	}

	public void OnPhotonSerializeView ()
	{
		// send
			if (textureHasChanged)
				texturePixelArray = (paintableObject.myRenderer.material.mainTexture as Texture2D).EncodeToPNG();
		
			// recive
			if (textureHasChanged)
				(paintableObject.myRenderer.material.mainTexture as Texture2D).LoadImage(texturePixelArray);



	}

}
