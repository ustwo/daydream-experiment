using UnityEngine;
using System.Collections;

public class TextureSharer : MonoBehaviour {
	[HideInInspector]
	public bool textureHasChanged = false;
	public Renderer myRenderer;
	private byte[] texturePixelArray;

	public void OnPhotonSerializeView ()
	{
		
			if (textureHasChanged)
				texturePixelArray = (myRenderer.material.mainTexture as Texture2D).EncodeToPNG();

			textureHasChanged = false;


	
			
			if (textureHasChanged)
				(myRenderer.material.mainTexture as Texture2D).LoadImage(texturePixelArray);



	}

}
