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
		//[SerializeField] private Camera camera;
			//[SerializeField] private bool isLocalPlayer;
		[SerializeField] private Material playerMatcache;
		[SerializeField] private GameObject bumperObj;
		[SyncVar] public float moveSpeed = 16.91f;
		//[SyncVar(hook = nameof(OnSetColor))] public Color playerColor;
		private SyncList<Vector4> colors = new SyncList<Vector4>();
		private bool isMovingLeft = false;
		private bool isMovingRight = false;


		//[SyncVar(hook = nameof(MovePlayer)), SerializeField] private float speed = 16.91f; // THe speed of the individual player. All player's speeds are the same


		// Start is called before the first frame update
		void Start()
		{
			Vector3 R = new Vector3(1, 0, 0);
			Vector3 B = new Vector3(0, 0, 1);
			Vector3 G = new Vector3(0, 0, 1);
			Vector3 Y = new Vector3(1, 0, 1);
			colors.Add(R);
			colors.Add(G);
			colors.Add(B);
			colors.Add(Y);
			//camera = Camera.main;
		//		camera.gameObject.transform.SetParent(null);
		//	if(!isLocalPlayer)
		//	{
		//		camera.gameObject.SetActive(false);
			//}
		}
// does not work at the moment		
/*
		public void OnSetColor(Color _oldColor, Color _newColor)
		{
			if(playerMatcache == null)
			{
				playerMatcache = gameObject.GetComponentInChildren<MeshRenderer>().material;
			}

			playerMatcache.color = _newColor;
		}
// does not work at the moment
		[Command]
		public void CmdSetPlayerColor()
		{
			int randomColor = UnityEngine.Random.Range(0, 4); 
			Color _newColor = colors[randomColor];
			Material childColor = GetComponentInChildren<MeshRenderer>().material;
			childColor.color = _newColor;
		} 
// does not work at the moment		
		[ClientRpc]
		public void RpcOnSetColor(Color _newColor)
		{
			int randomColor = UnityEngine.Random.Range(0, 4); 
			_newColor = colors[randomColor];
			if(playerMatcache == null)
			{
				playerMatcache = gameObject.GetComponent<MeshRenderer>().material;
			}

			playerMatcache.color = _newColor;
		}
		*/

		// Update is called once per frame
		void Update() 
		{
			if(!isLocalPlayer)
			{
				return;
			}
			//CmdGenerateBumper(); 
		}

		//at the moment, you should be able to move the character upon leading it into the scene. However
		// I haven't figured out how to add GUI to the scene and whether or not that affects the player movement
		// server wide.

		public override void OnStartClient()
		{
			// * This will run REGARDLESS of if we are local player or remote.
			// * local player is true if this object is the clients local player otherwise false.
			PlayerController controller = gameObject.GetComponent<PlayerController>();
			BumperGenerator bumper = gameObject.GetComponent<BumperGenerator>();
			//controller.enabled = isLocalPlayer;
			//bumper.enabled = isLocalPlayer;

		}

		
		[ClientRpc]
		public void CmdGenerateBumper()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				GameObject bumperSpawn = Instantiate(bumperObj);
				NetworkServer.Spawn(bumperObj);	
			}
			
		}
		/*
		[ClientRpc]
		public void RpcGenerateBumper()
		{
			GameObject bumperSpawn = Instantiate(bumperObj, gameObject.transform);
			NetworkServer.Spawn(bumperSpawn);	
			
			
		}*/
		
		public override void OnStopClient()
		{
			Destroy(gameObject); // yes i know this isn't the correct way just yet.
		}
		

		




	}
}