using UnityEngine;
using System.Collections;

public class TextureSharer : Photon.MonoBehaviour {
	[HideInInspector]
	public bool textureHasChanged = false;
	public Renderer myRenderer;
	private byte[] texturePixelArray;

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (textureHasChanged);
			if (textureHasChanged)
				texturePixelArray = (myRenderer.material.mainTexture as Texture2D).EncodeToPNG();
			stream.SendNext (texturePixelArray);
			textureHasChanged = false;


		} else {
			textureHasChanged = (bool)stream.ReceiveNext ();
			texturePixelArray = (byte[])stream.ReceiveNext ();
			if (textureHasChanged)
				(myRenderer.material.mainTexture as Texture2D).LoadImage(texturePixelArray);


		}
	}

}
