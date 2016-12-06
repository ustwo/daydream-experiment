using UnityEngine;
using System.Collections;

public class OKButton : MonoBehaviour 
{

	public Material activeMaterial;
	public Material inactiveMaterial;

	private Renderer renderer;

	public GameObject micButton;

	// Use this for initialization
	void Start () 
	{
		renderer = GetComponent<Renderer> ();
		renderer.material = inactiveMaterial;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnButtonDown()
	{
		renderer.material = activeMaterial;
	}

	public void ConfirmInput()
	{
//		Debug.Log ("howdy");
		renderer.material = inactiveMaterial;
		micButton.GetComponent<MicButton> ().ToggleActive ();
	}

}
