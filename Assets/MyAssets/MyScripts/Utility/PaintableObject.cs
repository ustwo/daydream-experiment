using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using System;


public class PaintableObject : NetworkBehaviour
{


	public Renderer myRenderer;
	private ComputeBitmap computeBitmap = new ComputeBitmap ();
	public Shader shader;
	private bool isInit = false;
	public Node node;
	public TextureSharer MirrorRenderer;
	public int textureSize;
	public Color32 startColor;
	public LayerMask paintLayers;



	Texture2D networkTexture;


	// Use this for initialization
	void Start ()
	{
		networkTexture = new Texture2D (1, 1);

		Init ();
	}

	// connected To network. after delay, lets request to see if there is a texture already darwn on this object
	public override void OnStartClient ()
	{
		Invoke ("DelayedRequest", 0.2f);
		base.OnStartClient ();
	}

	void Init ()
	{
		isInit = true;
		Texture2D myTexture = new Texture2D (textureSize, textureSize, TextureFormat.RGBA32, false);
		myTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] textureColors = new Color32[textureSize * textureSize];
		Color32 blankColor = startColor;
		for (int i = 0; i < textureColors.Length; i++) {
			textureColors [i] = blankColor;
		}
		myTexture.SetPixels32 (textureColors);
		myTexture.Apply ();
		myRenderer.material = new Material (shader);
		myRenderer.material.mainTexture = myTexture;
		Debug.Log ("Inited " + myRenderer.material.mainTexture.width);

	}

	// Request a texture from the server.
	void DelayedRequest ()
	{
		Cmd_RequestTexture ();
	}

	// For Debugging
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Init ();
		}

	}

	// pen trys to draw on sruface. Ray detected
	public void RegisterRay (Vector2 uvCords, Texture2D brush, float intencity, Color32 addColor)
	{
		if (!isInit)
			return;

		// Compute Brush application
		computeBitmap.ComputeBitMap (myRenderer.material.mainTexture, brush, uvCords, intencity, addColor);

		if (node != null)
			node.textureHasChanged = true;
		if (MirrorRenderer != null)
			MirrorRenderer.textureHasChanged = true; 
	
	}

	// Called on client  after texture was changed.
	public void SendTexture ()
	{
		Debug.Log ("Sending Texture from Server");
		networkTexture = (Texture2D)myRenderer.material.mainTexture;
		byte[] currentTexByteArray = ((Texture2D)myRenderer.material.mainTexture).EncodeToPNG ();
		Cmd_UpdateTexture (currentTexByteArray);
	}

	// Update texture on the server
	[Command (channel = 2)]
	void Cmd_UpdateTexture (byte[] bytes)
	{
		Debug.Log ("Server Update Texture");
		if (networkTexture.LoadImage (bytes)) {
			Debug.Log ("loading worked");
			myRenderer.material.mainTexture = networkTexture;
			RpcUpdateTexture (bytes);
		}
	}



	[Command (channel = 2)]
	void Cmd_RequestTexture ()
	{
		Debug.Log ("requesting texture.");
		byte[] currentTex = ((Texture2D)myRenderer.material.mainTexture).EncodeToPNG ();
		RpcUpdateTexture (currentTex);
	}

	[ClientRpc (channel = 2)]
	void RpcUpdateTexture (byte[] bytes)
	{
		Debug.Log ("Client UpdateTexture Texture");
		if (bytes == null || networkTexture == null) {
			Debug.Log ("Something is null here");
			return;
		}

		if(networkTexture.LoadImage (bytes))
			myRenderer.material.mainTexture = networkTexture;
	}






	void LateUpdate ()
	{
		if (!isInit) {
			Init ();
		}
	}


}
