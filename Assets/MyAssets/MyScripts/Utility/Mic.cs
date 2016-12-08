using UnityEngine;
using System.Collections;

public class Mic : Tool
{

	public Material activeMaterial;
	public Material idleMat;
	public Renderer micRend;

	public override void SetToolAbility (bool incBool)
	{
		if (incBool)
			micRend.material = activeMaterial;
		else
			micRend.material = idleMat;
		base.SetToolAbility (incBool);
	}
}
