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
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraSpawnPos;

    [SyncVar] public float cDtimer = 1.2f;
    [SyncVar] public bool canKick = true;

    [SyncVar] public bool hit = false;
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
        if(canKick)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                CmdSpawnBumper();
                canKick = false;
            } 
        }
        BumperCooldown();
    }

    /// <summary>
    /// THe cooldown timer for the bumper Will check if timer variable is at 0 or below before resetting and changing
    /// cankick to true.
    /// </summary>
    public void BumperCooldown()
    {
        // if you have used your bumper thus setting can kick to false
        if(!canKick)
        {
            // also if you have hit something, reduce cooldown by half.
            if(hit)
            {
                cDtimer *= 0.1f;
                cDtimer -= 1 * Time.deltaTime;
                hit = false;
            }
            else
            {
                cDtimer -= 1 * Time.deltaTime; //otherwise just decrease by one over time.    
            }
            
            Debug.Log($"ready in : {cDtimer.ToString("F0")}");

        }

        // if cooldown timer reaches or goes negatively past 0
        if(cDtimer <= 0)
        {
            canKick = true; // become able to kick again
            cDtimer = 1.2f; // refresh timer.
            Debug.Log("You may kick again");
        }
    }

    // Allows the client to move their player separately from remote players.
    public override void OnStartClient()
    {
        PlayerManager player = gameObject.GetComponent<PlayerManager>();
        player.enabled = isLocalPlayer;
        SetCameraPos();

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
        NetworkServer.Spawn(_bumper); // spawn on the server.
        RpcSpawnBumper(_bumper);
    }

    
    // so that the client sees their own bumper spawning at the right place
    [ClientRpc]
    public void RpcSpawnBumper(GameObject bump)
    {
        bump.transform.SetParent(kickZone,false);
    }
    
    //as we don't need to sync the camera movement, we will set camera rotation based on local player
    private void SetCameraPos()
    {
        camera = FindObjectOfType<Camera>();
        if(isLocalPlayer)
        {
            camera.transform.localRotation = cameraSpawnPos.transform.rotation;
        }
    }

}
