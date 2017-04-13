using UnityEngine;
using System.Collections;

public class Mic : Tool
{



	public GameObject micModel;

	public float speed = 10f;
	private float anchorTime = 0;
	private bool anchorToNode = false;

	[HideInInspector]
	public bool IsListening = false;

	[HideInInspector]
	public bool IsRecording = false;

	private AudioSource source;
	public AudioClip micOn;
	public AudioClip micOff;

	bool onClipDidPlay = false;
	bool offClipDidPlay = true;

	public override void OnEnable()
	{
		base.OnEnable ();
		transform.localPosition = Vector3.zero;
	}

	void Update()
	{
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
