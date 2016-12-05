using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Widgets;

public class MicButton : MonoBehaviour
{
	public MicrophoneWidget microphone;

	bool isActive = false;

	public Material activeMaterial;
	public Material inactiveMaterial;
	private Renderer renderer;

	void Start()
	{
		renderer = GetComponent<Renderer> ();
		renderer.material = inactiveMaterial;
	}

	/// <summary>
	/// unparent the stroke object.
	/// </summary>
	public void ToggleActive ()
	{
		isActive = !isActive;

		if(isActive) {
			Debug.Log ("Mic button is active");
			renderer.material = activeMaterial;
			microphone.ActivateMicrophone ();
		} else {
			Debug.Log ("Mic button is inactive");
			renderer.material = inactiveMaterial;
			microphone.DeactivateMicrophone ();
		}
	}

}
