using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using System;


public class PaintableObject : NetworkBehaviour
{
	//	public class SyncListByte : SyncList<byte[]> {
	//		protected override void SerializeItem (NetworkWriter writer, byte[] item)
	//		{
	//			writer.Write (item);
	//		}
	//		protected override byte DeserializeItem (NetworkReader reader)
	//		{
	//			return reader.ReadBytesAndSize ();
	//		}
	//	}

	public Renderer myRenderer;
	private ComputeBitmap computeBitmap = new ComputeBitmap ();
	public Shader shader;
	private bool isInit = false;
	public Node node;
	public TextureSharer MirrorRenderer;
	public int textureSize;
	public Color32 startColor;
	public LayerMask paintLayers;
	private float sendcooldown = 0;


	//	public struct textureByteHolder{
	//		public byte[] bytes;
	//	}
	//	public class byteHoldClass : SyncListStruct<textureByteHolder>{}
	//	byteHoldClass byteHolder = new byteHoldClass();

	Texture2D networkTexture;

	//private SyncListInt networkTexture = new SyncListInt();
	// Use this for initialization
	void Start ()
	{
		networkTexture = new Texture2D (1, 1);
		//networkTexture.Callback = ChangeNetWorkTexture;
		Init ();
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

	// For Debugging
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Init ();
		}
		if (sendcooldown > 0)
			sendcooldown -= Time.deltaTime;
	}

	public void RegisterRay (Vector2 uvCords, Texture2D brush, float intencity, Color32 addColor)
	{
		if (!isInit)
			return;
		//myRenderer.material.mainTexture = 
		computeBitmap.ComputeBitMap (myRenderer.material.mainTexture, brush, uvCords, intencity, addColor);
		if (node != null)
			node.textureHasChanged = true;
		if (MirrorRenderer != null)
			MirrorRenderer.textureHasChanged = true; 
		//	Debug.Log(myRenderer.material.mainTexture.width);
		//byte[] currentTex = ((Texture2D)myRenderer.material.mainTexture).EncodeToPNG ();
		//string stringByteArray = Convert.ToBase64String (currentTex);
		//.Log ("TextureSize " + stringByteArray.Length);


//		if (sendcooldown <= 0) {
//			sendcooldown = 0.3f;
//			if (isServer)
//				RpcUpdateTexture (currentTex);
//			else
//				Cmd_UpdateTexture (currentTex);
//		}


		//byte[] textureBytes = ((Texture2D)myRenderer.material.mainTexture).EncodeToPNG ();
		//networkTexture = ((Texture2D)myRenderer.material.mainTexture).EncodeToPNG ();
	}

	public void SendTexture ()
	{
		byte[] currentTex = ((Texture2D)myRenderer.material.mainTexture).EncodeToPNG ();
		if (sendcooldown <= 0) {
			sendcooldown = 0.3f;
			if (isServer)
				RpcUpdateTexture (currentTex);
			else
				Cmd_UpdateTexture (currentTex);
		}
	}

	[ClientRpc(channel = 2)]
	void RpcUpdateTexture (byte[] bytes)
	{
		if (bytes == null || networkTexture == null) {
			Debug.Log ("Something is null here");
			return;
		}
		//byte[] intArrayToByte = Convert.FromBase64String (bytes);
		networkTexture.LoadImage (bytes);
		myRenderer.material.mainTexture = networkTexture;
	}

	[Command(channel = 2)]
	void Cmd_UpdateTexture (byte[] bytes)
	{
		networkTexture.LoadImage (bytes);
		myRenderer.material.mainTexture = networkTexture;
	}

	//	void TextureChanged(SyncListStruct<byteHoldClass>.Operation op, int itemIndex)
	//	{
	//		Texture2D recivedTexture = new Texture2D (1, 1);
	//		recivedTexture.LoadImage (((textureByteHolder)op).bytes);
	//		//recivedTexture.LoadImage(op
	//		//myRenderer.material.mainTexture =
	//		Debug.Log("texture changed:" + op);
	//	}
	//	void ChangeNetWorkTexture( SyncListInt.Operation recivedBytesAsInt,int index){
	//		int[] intArray = recivedBytesAsInt.ToArray ();
	//		Texture2D recivedTexture = new Texture2D (textureSize, textureSize);
	//		byte[] byteArray = intArray.Select(x => (byte)x).ToArray();
	//		recivedTexture.LoadImage (byteArray);
	//		myRenderer.material.mainTexture = recivedTexture;
	//	}


	void LateUpdate ()
	{
		if (!isInit) {
			Init ();
		}
	}


}
