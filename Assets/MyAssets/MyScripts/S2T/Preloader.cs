using UnityEngine;
using System.Collections;

public class Preloader : MonoBehaviour {

	public float speed = 10F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.back * Time.deltaTime * speed);
	}
}
