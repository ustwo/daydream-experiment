using UnityEngine;
using System.Collections;

public class NetworkSmoother : Photon.MonoBehaviour{

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = transform.position;
			stream.Serialize(ref pos);
			Quaternion rot = transform.rotation;
			stream.Serialize (ref rot);
		}
		else
		{
			Vector3 pos = Vector3.zero;
			stream.Serialize(ref pos);  // pos gets filled-in. must be used somewhere
			transform.position = pos;
			Quaternion rot = Quaternion.identity;
			stream.Serialize (ref rot);
			transform.rotation = rot;
		}
	}
}
