using UnityEngine;
using System.Collections;

public class Pen : Tool
{

	// the speed at which the pen travels to its desired destination
	public float moveSpeed = 10f;

	// how long before it just pops into place
	private float popInPlaceTime = 0;

	// did it pop in place?
	private bool poppedInPlace = true;

	public PainBrush paintBrush;

	public Color[] brushColors;
	private int currentBrushColorIndex = 0;

	public Material penMat;
	public Transform rayHit;

	[HideInInspector]
	public Material playerPenMat;

	public GameObject[] penComponents;

	public override void OnEnable ()
	{
		base.OnEnable ();
		transform.localPosition = Vector3.zero;
		rayHit.localScale = Vector3.one * (0.05f * paintBrush.brushSize);

	}

	void Start()
	{
		playerPenMat = new Material (penMat);

		foreach(GameObject item in penComponents) {
			item.GetComponent<Renderer> ().material = playerPenMat;
		}

		currentBrushColorIndex = (int)Random.Range (0, brushColors.Length - 1);
		UpdateBrushColor ();
	}

	void Update ()
	{

		// If popped in place, dont continue
		if (poppedInPlace)
			return;

		// if transition time left over, move object to deisred destination. 
		Vector3 adjustedPosition =	GetDesiredPosition;
		adjustedPosition.x = 0;
		adjustedPosition.y = 0;
		if (popInPlaceTime > 0) {
			transform.localPosition = Vector3.MoveTowards (transform.localPosition, adjustedPosition, moveSpeed * Time.deltaTime);
			popInPlaceTime -= Time.deltaTime;
		} else {
			transform.localPosition = adjustedPosition;
			poppedInPlace = true;
		}
	}

	public override void SetToolAbility (bool incBool)
	{
		// check to make sure this object is turned on
		if (!gameObject.activeSelf)
			return;

		Debug.Log ("pen On");
		//set a transition time 
		//popInPlaceTime = 1f;

		// enable commands in update
		//poppedInPlace = false;

		// activate the paint brush class
		paintBrush.SetBrush (incBool);

		// set on base
		base.SetToolAbility (incBool);
	}

	public override void SetMovePosition (Vector3 incPos)
	{
		Vector3 adjustedPosition = GetDesiredPosition;
		adjustedPosition.x = 0;
		adjustedPosition.y = 0;
		transform.localPosition = adjustedPosition;
		base.SetMovePosition (incPos);
	}

	public override void ButtonOpetionLT ()
	{
		paintBrush.brushSize += 5;
		rayHit.localScale = Vector3.one * (0.05f * paintBrush.brushSize);
		paintBrush.Init ();
		base.ButtonOpetionLT ();
	}

	public override void ButtonOptionLB ()
	{
		if (paintBrush.brushSize > 5) {
			paintBrush.brushSize -= 5;
			rayHit.localScale = Vector3.one * (0.05f * paintBrush.brushSize);
			paintBrush.Init ();
		}
		base.ButtonOptionLB ();
	}

	public override void ButtonOptionRB ()
	{
		currentBrushColorIndex++;
		if (currentBrushColorIndex == brushColors.Length)
			currentBrushColorIndex = 0;

		UpdateBrushColor ();

		base.ButtonOptionRB ();
	}

	public override void ButtonOptionRT ()
	{
		currentBrushColorIndex--;
		if (currentBrushColorIndex == -1)
			currentBrushColorIndex = brushColors.Length - 1;

		UpdateBrushColor ();

		base.ButtonOptionRT ();
	}

	public Color GetBrushColor()
	{
		return brushColors [currentBrushColorIndex];
	}

	public int GetColorIndex()
	{
		return currentBrushColorIndex;
	}

	void UpdateBrushColor()
	{
//		Debug.Log ("color index: " + currentBrushColorIndex);
		playerPenMat.color = brushColors [currentBrushColorIndex];
		paintBrush.color = brushColors [currentBrushColorIndex];
	}

}
