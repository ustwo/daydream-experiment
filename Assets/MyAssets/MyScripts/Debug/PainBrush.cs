using UnityEngine;
using System.Collections;

public class PainBrush : MonoBehaviour
{

	PaintableObject activeSurface;
	public float rayReach = 20f;
	private float spacing = 0.1f;
	[Range (0.00001f, 0.1f)]
	public  float maxSpacing = 0.1f;
	public Texture2D activeBrush;

	public float intencity = 3;
	Vector2 lastUVCords;
	public Color color;
	private Texture2D scaledBrush;
	[Range (1,10)]
	public int brushSize;
	public LayerMask paintLayers;
	private bool brushActivated = false;
	public Transform pointer;
	private bool resetBrushCords;
	private PaintableObject lastSurface;
	Vector2 currentCords;
	Vector2 lerpCords;
	private bool draw = false;
	public float brushSpeed = 10;
	public float brushSmoothTime = 1;


	void Start ()
	{
		Init ();
		//SetBrush (true);

	}

	public void Init(){
		spacing = maxSpacing;
		scaledBrush = Instantiate (activeBrush);
		//scaledBrush = new Texture2D(1,1,TextureFormat.RGBA32,false);
		//scaledBrush.SetPixels32 (new Color32[]{ new Color32 ((byte)255, (byte)255, (byte)255, (byte)125) });
		//Debug.Log (scaledBrush.format);
		TextureScale.Bilinear (scaledBrush, brushSize, brushSize);
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
		Debug.DrawRay (pointer.position, pointer.forward * rayReach);
		if (Physics.Raycast (pointer.position, pointer.forward, out hit, rayReach, paintLayers.value)) {
			
			//Debug.Log ("Raycast hit");
			activeSurface = hit.transform.root.GetComponent<PaintableObject> ();
			if (activeSurface != null) {
				draw = true;
				currentCords = hit.textureCoord;
				if (resetBrushCords || lastSurface != activeSurface) {
					scaledBrush = Instantiate (activeBrush);
					TextureScale.Bilinear (scaledBrush, (int)(brushSize * ((float)activeSurface.textureSize / 256)), (int)(brushSize * ((float)activeSurface.textureSize / 256)));
					//Debug.Log (scaledBrush.format);
					lastUVCords = currentCords;
					resetBrushCords = false;
					activeSurface.RegisterRay (currentCords, scaledBrush, intencity, color);
					lastUVCords = currentCords;
					lastSurface = activeSurface;
					return;
				}

				/// Draw from here
			} else {
				draw = false;
			}

		} else {
			draw = false;
		}
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			Init ();
		}
		// to here 
		if (draw) {
			for (int i = 0; i < 2; i++) {
				lerpCords = Vector2.MoveTowards (lastUVCords, currentCords, brushSize * 0.001f);
				activeSurface.RegisterRay (lerpCords, scaledBrush, intencity, color);
				lastUVCords = lerpCords;
			}
		}
	}
}
