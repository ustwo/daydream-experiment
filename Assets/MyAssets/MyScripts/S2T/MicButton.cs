using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MicButton : MonoBehaviour {

	private bool isActive = false;

	public Material activeMaterial;
	public Material inactiveMaterial;

	private Renderer renderer;

	public Canvas sttCanvas;

	void Start () 
	{
		renderer = GetComponent<Renderer> ();
		sttCanvas.enabled = false;
	}

	void Update () 
	{
	
	}

	public void ToggleActive()
	{
		isActive = !isActive;

		Debug.Log ("Button state: " + isActive);

		if(isActive) {
			renderer.material = activeMaterial;
		} else {
			renderer.material = inactiveMaterial;
		}

		sttCanvas.enabled = isActive;
	}
}
