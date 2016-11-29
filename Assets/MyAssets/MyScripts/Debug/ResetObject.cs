using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetObject : MonoBehaviour {

	public void ResetScene(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

}
