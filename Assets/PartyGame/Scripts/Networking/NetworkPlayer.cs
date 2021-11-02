using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using NetworkPartyGame.Physics;

using System.Security.Cryptography;

using TMPro;

namespace NetworkPartyGame.Player
{
	/// <summary>
	/// Networking still does my head in, I moved a few things from Player controller to here,
	/// it should work? probably not but it's better than what i had before. 
	/// </summary>
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(BumperGenerator))]
	public class NetworkPlayer : NetworkBehaviour
	{
		[SerializeField] private bool isLocalPlayer;

		[SyncVar] public float moveSpeed = 16.91f;

		private bool isMovingLeft = false;
		private bool isMovingRight = false;


		//[SyncVar(hook = nameof(MovePlayer)), SerializeField] private float speed = 16.91f; // THe speed of the individual player. All player's speeds are the same


		// Start is called before the first frame update
		void Start() { }

		// Update is called once per frame
		void Update() { }

		//at the moment, you should be able to move the character upon leading it into the scene. However
		// I haven't figured out how to add GUI to the scene and whether or not that affects the player movement
		// server wide.

		public override void OnStartClient()
		{
			// * This will run REGARDLESS of if we are local player or remote.
			// * local player is true if this object is the clients local player otherwise false.
			PlayerController controller = gameObject.GetComponent<PlayerController>();
			BumperGenerator bumper = gameObject.GetComponent<BumperGenerator>();
			controller.enabled = isLocalPlayer;
			controller.enabled = isLocalPlayer;

		}
		public override void OnStopClient()
		{
			Destroy(gameObject); // yes i know this isn't the correct way just yet.
		}
		

		




	}
}