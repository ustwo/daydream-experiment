using UnityEngine;
using System.Collections;

public class PainBrush : MonoBehaviour
{

	PaintableObject activeSurface;
	public float rayReach = 20f;
	private float spacing = 0.1f;
	[Range (0.001f, 0.1f)]
	public  float maxSpacing = 0.1f;
	public Texture2D activeBrush;

	public float intencity = 3;
	Vector2 lastUVCords;
	public Color color;
	private Texture2D scaledBrush;
	public int brushSize;
	public LayerMask paintLayers;
	private bool brushActivated = false;
	public Transform pointer;
	private bool resetBrushCords;
	private PaintableObject lastSurface;
	Vector2 currentCords;
	Vector2 lerpCords;


	void Start ()
	{
		Init ();

	}

	public void Init(){
		spacing = maxSpacing;
		scaledBrush = Instantiate (activeBrush);
		//scaledBrush = new Texture2D(1,1,TextureFormat.RGBA32,false);
		//scaledBrush.SetPixels32 (new Color32[]{ new Color32 ((byte)255, (byte)255, (byte)255, (byte)125) });
		//Debug.Log (scaledBrush.format);
		TextureScale.Bilinear (scaledBrush, brushSize, brushSize);
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			Init ();
		}
	}

	public void SetBrush (bool toggle){
		//Debug.Log ("Brush set to " + toggle);
		brushActivated = toggle;
		resetBrushCords = toggle;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!brushActivated)
			return;
		RaycastHit hit;
		if (Physics.Raycast (pointer.position, pointer.forward, out hit, rayReach,paintLayers.value)) {
			//Debug.Log ("Raycast hit");
			activeSurface = hit.transform.GetComponent<PaintableObject> ();
			if (activeSurface != null) {
				currentCords = hit.textureCoord;
				if (resetBrushCords || lastSurface != activeSurface) {
					scaledBrush = Instantiate (activeBrush);
					TextureScale.Bilinear (scaledBrush, (int)(brushSize * ((float)activeSurface.textureSize / 256)), (int)(brushSize * ((float)activeSurface.textureSize / 256)));
					//Debug.Log (scaledBrush.format);
					lastUVCords = currentCords;
					resetBrushCords = false;
					activeSurface.RegisterRay (currentCords, scaledBrush, intencity,color);
					lastUVCords = currentCords;
					lastSurface = activeSurface;
					return;
				}

				lerpCords = Vector2.MoveTowards (lastUVCords, currentCords, (0.001f*brushSize));
				//Debug.Log (" Draw distance = " + (lerpCords-currentCords).sqrMagnitude + " maxSpacing = " + maxSpacing);
				if ((lerpCords-currentCords).sqrMagnitude < maxSpacing )
					return;
				activeSurface.RegisterRay (lerpCords, scaledBrush, intencity,color);
				lastUVCords = lerpCords;
			}

		}
	}
}
