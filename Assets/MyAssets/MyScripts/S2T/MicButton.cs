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

	public GameObject confirmationButton;

	public GameObject confirmationText;

	void Start () 
	{
		renderer = GetComponent<Renderer> ();
//		sttCanvas.enabled = false;
		confirmationButton.GetComponent<Renderer> ().enabled = false;
		confirmationText.GetComponent<Renderer> ().enabled = false;
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
			confirmationButton.GetComponent<Renderer> ().enabled = true;
			confirmationText.GetComponent<Renderer> ().enabled = true;
		} else {
			renderer.material = inactiveMaterial;
			micWidget.DeactivateMicrophone ();
			confirmationButton.GetComponent<Renderer> ().enabled = false;
			confirmationText.GetComponent<Renderer> ().enabled = false;
		}

		sttCanvas.enabled = isActive;
	}
}
