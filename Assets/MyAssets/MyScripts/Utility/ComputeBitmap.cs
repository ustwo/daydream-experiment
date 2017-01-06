using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ComputeBitmap
{
	private Texture lastTexture;
	private Color32[] storedMainTexture;
	private int computeItiration = 0;
	private Color32[] storedBrush;
	private Texture2D lastBrush;


	public void StoreMainTexture(Texture mainTex){
	//	Debug.Log ("assigning storedMainTexture");
		Texture2D mainTexConv = mainTex as Texture2D;
		storedMainTexture = mainTexConv.GetPixels32 ();
		lastTexture = mainTex;
	}

	public void ApplyToMainTexture(Texture mainTexture){
		computeItiration = 0;
		Texture2D mainTexConv = mainTexture as Texture2D;
		//Debug.Log (mainTexConv.format);
		mainTexConv.SetPixels32 (storedMainTexture);
		mainTexConv.Apply ();
	}

	public void ComputeBitMap (Texture mainTexture, Texture2D brush, Vector2 uvCords,float intencity , Color32 addColor)
	{
		if (lastBrush != brush) {
			lastBrush = brush;
			storedBrush = brush.GetPixels32 ();
			//Debug.Log ("new brush");
		}

		if (lastTexture != mainTexture) {
			if (lastTexture != null)
				ApplyToMainTexture (mainTexture);
			StoreMainTexture (mainTexture);
		}

		//Texture2D returnTexture = new Texture2D (mainTexture.width, mainTexture.height);
		Vector2 CenterOfMain = new Vector2 (mainTexture.width * uvCords.x, mainTexture.height * uvCords.y);
		int mainWith = mainTexture.width;
		int mainHeight = mainTexture.height;
		int brushHeight = brush.height;
		int brushWidth = brush.width;
		/// Replace with stored MainTex
		//Color[] mainColorArray = mainTexture.GetPixels ();
		Color32[] colorMix = storedMainTexture;
		Color32[] brushColorArray = storedBrush;
		Vector2 brushStart = new Vector2 (CenterOfMain.x - brushWidth / 2, CenterOfMain.y - brushHeight / 2);

		int pointX = 0;
		int pointY = 0;
		for (int y = 0; y < brushHeight; y++) {
			pointY = y + (int)brushStart.y;
			for (int x = 0; x < brushWidth; x++) {
				pointX = x + (int)brushStart.x;
				int mainColIndex = pointY * mainWith + pointX;
				int BrushColIndex = y * brushWidth + x;

				if (mainColIndex < 0 || mainColIndex>=storedMainTexture.Length)
					break;
				// Color Add
//				colorMix [mainColIndex].r = mainColorArray [mainColIndex].r + brushColorArray [BrushColIndex].r *intencity * addColor.r;
//				colorMix [mainColIndex].g = mainColorArray [mainColIndex].g + brushColorArray [BrushColIndex].g * intencity * addColor.g;
//				colorMix [mainColIndex].b = mainColorArray [mainColIndex].b + brushColorArray [BrushColIndex].b* intencity * addColor.b;
//				colorMix [mainColIndex].a = mainColorArray [mainColIndex].a + brushColorArray [BrushColIndex].r*intencity;

				// Color Replace
				colorMix[mainColIndex] = Color32.Lerp(storedMainTexture[mainColIndex],addColor,Convert.ToSingle(brushColorArray[BrushColIndex].a)/255);
			//	Debug.Log((float)brushColorArray[BrushColIndex].a/255);
			//	colorMix [mainColIndex].r = (byte)Mathf.Lerp((float)storedMainTexture[mainColIndex].r, (float)addColor.r ,(byte)((float)brushColorArray[BrushColIndex].a/255));
			//	colorMix [mainColIndex].g = (byte) Mathf.Lerp((float)storedMainTexture[mainColIndex].g,   (float)addColor.g ,(byte)((float)brushColorArray[BrushColIndex].a/255));
			//	colorMix [mainColIndex].b = (byte) Mathf.Lerp( (float)storedMainTexture[mainColIndex].b,  (float)addColor.b ,(byte)((float)brushColorArray[BrushColIndex].a/255));
				//colorMix[mainColIndex].a = (byte)255;
				//colorMix [mainColIndex].a = (byte)Mathf.Clamp((Convert.ToInt32( storedMainTexture [mainColIndex].a )+ Convert.ToInt32(brushColorArray [BrushColIndex].a)),0,255);// * addColor.a);
				//Debug.Log(colorMix[mainColIndex].a);
			}
		}
		storedMainTexture = colorMix;
		computeItiration++;
		if (computeItiration > 0) {
			ApplyToMainTexture (mainTexture);
		}
		//Debug.Log (computeItiration);
		//return returnTexture;
	}
}
