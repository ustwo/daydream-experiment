using UnityEngine;
using System.Collections;

public class ComputeBitmap
{
	public Texture2D ComputeBitMap (Texture2D mainTexture, Texture2D brush, Vector2 uvCords,float intencity , Color addColor)
	{

		Texture2D returnTexture = new Texture2D (mainTexture.width, mainTexture.height);
		Vector2 CenterOfMain = new Vector2 (mainTexture.width * uvCords.x, mainTexture.height * uvCords.y);
		int mainWith = mainTexture.width;
		int mainHeight = mainTexture.height;
		int brushHeight = brush.height;
		int brushWidth = brush.width;
		Color[] mainColorArray = mainTexture.GetPixels ();
		Color[] colorMix = mainColorArray;
		Color[] brushColorArray = brush.GetPixels ();
		Vector2 brushStart = new Vector2 (CenterOfMain.x - brushWidth / 2, CenterOfMain.y - brushHeight / 2);

		int pointX = 0;
		int pointY = 0;
		for (int y = 0; y < brushHeight; y++) {
			pointY = y + (int)brushStart.y;
			for (int x = 0; x < brushWidth; x++) {
				pointX = x + (int)brushStart.x;
				int mainColIndex = pointY * mainWith + pointX;
				int BrushColIndex = y * brushWidth + x;

				if (mainColIndex < 0 || mainColIndex>=mainColorArray.Length)
					break;

				colorMix [mainColIndex].r = mainColorArray [mainColIndex].r + brushColorArray [BrushColIndex].r *intencity * addColor.r;
				colorMix [mainColIndex].g = mainColorArray [mainColIndex].g + brushColorArray [BrushColIndex].g * intencity * addColor.g;
				colorMix [mainColIndex].b = mainColorArray [mainColIndex].b + brushColorArray [BrushColIndex].b* intencity * addColor.b;
				colorMix [mainColIndex].a = mainColorArray [mainColIndex].a + brushColorArray [BrushColIndex].r*intencity;
			}
		}
		returnTexture.SetPixels (colorMix);
		returnTexture.Apply ();
		return returnTexture;
	}


}
