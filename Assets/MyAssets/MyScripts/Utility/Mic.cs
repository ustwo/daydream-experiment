using UnityEngine;
using System.Collections;

public class Mic : Tool
{

	public Material activeMaterial;
	public Material idleMat;
	public Renderer micRend;

	public float speed = 10f;
	private float anchorTime = 0;
	private bool anchorToNode = false;

	public override void OnEnable()
	{
		base.OnEnable ();
		transform.localPosition = Vector3.zero;
//		transform.localRotation = Vector3.zero;
	}

	void Update()
	{
//		Debug.Log ("current: " + transform.position + ", destination: " + target.position);

		if (target == null)
			return;

		if (anchorToNode && anchorTime > 0) {
			transform.position = Vector3.MoveTowards (transform.position, target.position, speed * Time.deltaTime);
			transform.rotation = target.rotation;
			anchorTime -= Time.deltaTime;
		} else {
			transform.position = target.position;
			anchorToNode = false;
		}
	}

	public override void SetToolAbility (bool incBool)
	{
		// check to make sure this object is turned on
		if (!gameObject.activeSelf)
			return;

		// set a transition time 
		anchorTime = 0.5f;

		// enable commands in update
		anchorToNode = true;

		// set on base
		base.SetToolAbility (incBool);
	}

}
