using UnityEngine;
using System.Collections;

public class TouchGuide : MonoBehaviour
{

	public Transform indicator;

	public void UpdateIndicatorPosition (Vector2 newPos)
	{

		indicator.localPosition = newPos;
	}
}
