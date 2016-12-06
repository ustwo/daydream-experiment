using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Widgets;

public class MicButton : MonoBehaviour {

	private bool isActive = false;

	public Material activeMaterial;
	public Material inactiveMaterial;

	private Renderer renderer;

	public Canvas sttCanvas;

	public MicrophoneWidget micWidget;

	void Start () 
	{
		renderer = GetComponent<Renderer> ();
//		sttCanvas.enabled = false;
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
			micWidget.ActivateMicrophone ();
		} else {
			renderer.material = inactiveMaterial;
			micWidget.DeactivateMicrophone ();
		}

		sttCanvas.enabled = isActive;
	}
}
