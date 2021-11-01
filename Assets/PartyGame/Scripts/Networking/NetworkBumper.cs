using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(BumperMechanic))]
public class NetworkBumper : NetworkBehaviour
{

	public void Update()
	{
		if(!isLocalPlayer)
		{
			return;
		}
	}
	//allows the bumper to be enabled to all clients.
	public override void OnStartClient()
	{
		BumperMechanic bump = gameObject.GetComponent<BumperMechanic>();
		//bump.enabled = isLocalPlayer;
	}
	
	
	
	
}
