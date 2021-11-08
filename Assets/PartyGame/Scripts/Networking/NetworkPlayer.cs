using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using NetworkPartyGame.Physics;


[RequireComponent((typeof(PlayerManager)))]
public class NetworkPlayer : NetworkBehaviour
{
    //the prefab being instantiated, in this case, the bumper. We want player to be spawning the bumper.
    [SerializeField] private GameObject bumper;
    [SerializeField] private Transform kickZone; // grabbing the transform for the bumper

    //[SerializeField] private Camera mainCamera;

    // Start is called before the first frame update
    void Start() { }
    
    // hoping that each player has their own camera view
    // [Command]
    //public void RpcSetCamera()
    //{
    //    mainCamera = Camera.main;
    //    mainCamera.transform.SetParent(this.transform, false);
   // }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdSpawnBumper();
        }
    }

    // Allows the client to move their player separately from remote players.
    public override void OnStartClient()
    {
        PlayerManager player = gameObject.GetComponent<PlayerManager>();
        player.enabled = isLocalPlayer;
    }

    //because a mono behaviour cannot spawn objects on the server
    //we require a command and a visualliser for the server.
    // visualliser = silly term for RpcClient
// actually pretty tempting to separate the bumper into it's own network script.
    [Command]
    public void CmdSpawnBumper()
    {
        // you would first need a variable for this to work.
        //Instantiate bumper at kickzone's transform as the parent.
        GameObject _bumper = Instantiate(bumper, kickZone); // We're instantiating the bumper GO, not to be confused with the behaviour of the bumper which is KickVisualiser
        NetworkServer.Spawn(_bumper);
        RpcSpawnBumper(_bumper);
    }

    // so that the client sees their own bumper spawning at the right place
    [ClientRpc]
    public void RpcSpawnBumper(GameObject bump)
    {
        bump.transform.SetParent(kickZone,false);
    }

}
