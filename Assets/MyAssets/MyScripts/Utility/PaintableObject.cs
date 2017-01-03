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
	public Color startColor;
	// Use this for initialization
	void Start ()
	{
		
		//Init ();
	}

	void Init ()
	{
		isInit = true;
		Texture2D myTexture = new Texture2D (textureSize, textureSize);
		myTexture.wrapMode = TextureWrapMode.Clamp;
		Color[] textureColors = new Color[textureSize * textureSize];
		Color blankColor = startColor;
		for (int i = 0; i < textureColors.Length; i++) {
			textureColors [i] = blankColor;
		}
		myTexture.SetPixels (textureColors);
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

	public void RegisterRay (Vector2 uvCords, Texture2D brush, float intencity, Color addColor)
	{
		
		myRenderer.material.mainTexture = computeBitmap.ComputeBitMap (myRenderer.material.mainTexture as Texture2D, brush, uvCords, intencity, addColor);
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
