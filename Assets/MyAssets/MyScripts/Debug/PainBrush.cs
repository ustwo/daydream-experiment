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
	private Vector3 lastDrawPoint;
	public float intencity = 3;
	Vector2 lastUVCords;
	public Color color;
	private Texture2D scaledBrush;
	public int brushSize;
	public LayerMask paintLayers;
	private bool brushActivated = false;
	public Transform pointer;
	public LineRenderer debugline;
	private bool resetBrushCords;

	void Start ()
	{
		Init ();
		debugline = gameObject.AddComponent<LineRenderer> ();
	}

	void Init(){
		lastDrawPoint = Vector3.zero;
		spacing = maxSpacing;
		scaledBrush = Instantiate (activeBrush) as Texture2D;
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

		//transform.Translate (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * (10 *Time.deltaTime), Space.World);
		if (!brushActivated)
			return;
		RaycastHit hit;
		debugline.SetPosition (0, pointer.position);
		debugline.SetPosition (1, pointer.position + pointer.forward * 10);
		//Debug.Log ("It got here");
		if (Physics.Raycast (pointer.position, pointer.forward, out hit, rayReach,paintLayers)) {
		//	Debug.Log ("Hit the paintable Objects");
			float distance = (hit.point - lastDrawPoint).sqrMagnitude;
			int brushCount = Mathf.FloorToInt (distance / maxSpacing);
			activeSurface = hit.transform.GetComponent<PaintableObject> ();
			if (activeSurface != null) {
				Vector2 currentCords = hit.textureCoord;
				if (resetBrushCords) {
					lastUVCords = currentCords;
					resetBrushCords = false;
				}
				Debug.Log ("distance = " + distance);
				Vector2 lerpCords = Vector2.Lerp (lastUVCords, currentCords, 0.1f);
				if (distance < maxSpacing && (lerpCords-currentCords).sqrMagnitude < maxSpacing )
					return;
				activeSurface.RegisterRay (lerpCords, scaledBrush, intencity,color);
				lastDrawPoint = hit.point;
				lastUVCords = lerpCords;
			}

		}
	}
}
