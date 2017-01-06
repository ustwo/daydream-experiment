using UnityEngine;
using System.Collections;

public class PaintableObject : MonoBehaviour
{

	public Renderer myRenderer;
	private ComputeBitmap computeBitmap = new ComputeBitmap ();
	public Shader shader;
	private bool isInit = false;
	public Node node;
	public TextureSharer textureSharerereer;
	public int textureSize = 200;
	public Color32 startColor;
	// Use this for initialization
	void Start ()
	{
		
		//Init ();
	}

	void Init ()
	{
		isInit = true;
		Texture2D myTexture = new Texture2D (textureSize, textureSize,TextureFormat.RGBA32,false);
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
	}

	// For Debugging
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Init ();
		}
	}

	public void RegisterRay (Vector2 uvCords, Texture2D brush, float intencity, Color32 addColor)
	{
		//myRenderer.material.mainTexture = 
		computeBitmap.ComputeBitMap (myRenderer.material.mainTexture , brush, uvCords, intencity, addColor);
		if (node != null)
			node.textureHasChanged = true;
		if (textureSharerereer != null)
			textureSharerereer.textureHasChanged = true;
	}

	void LateUpdate ()
	{
		if (!isInit) {
			Init ();
		}
	}


}
