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
	public Texture2D newActiveBrush;
	public int brushSize;

	void Start ()
	{
		Init ();
	}

	void Init(){
		lastDrawPoint = Vector3.zero;
		spacing = maxSpacing;
		newActiveBrush = activeBrush;
		newActiveBrush.Resize (brushSize, brushSize);
		newActiveBrush.Apply ();
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			Init ();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		transform.Translate (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * (10 *Time.deltaTime), Space.World);

		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, rayReach)) {
			float distance = (hit.point - lastDrawPoint).sqrMagnitude;
			if (distance < maxSpacing)
				return;
			int brushCount = Mathf.FloorToInt (distance / maxSpacing);
			activeSurface = hit.transform.GetComponent<PaintableObject> ();
			if (activeSurface != null) {
				Vector2 currentCords = hit.textureCoord;
				activeSurface.RegisterRay (currentCords, newActiveBrush, intencity,color);
				lastDrawPoint = hit.point;


			}
		}
	}
}
