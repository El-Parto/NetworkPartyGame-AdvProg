using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;
using Mirror;

using NetworkPartyGame.Physics;
using PartyGame.Scripts;
using PartyGame.Scripts.Networking;
using UnityEngine.SceneManagement;


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
    [SyncVar (hook = nameof(CmdUpdatePlayerScore))] public int playerScore;
    // The player's health
    [SyncVar (hook = nameof(CmdUpdatePlayerHealth))] public int playerHealth;
    [SyncVar (hook = nameof(CmdUpdatePlayerColour))] public Color playerColour;
    
    public MeshRenderer rendererPlayer;
    public MeshRenderer rendererBumper;
    private Material materialPlayer;
    private Material materialBumper;

    private bool hasAddedToGui = false;
    private bool hasChangedColour = false;
    
        //[SerializeField] private Camera mainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("net player start called");

    }
    
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
        //hacky way to avoid error when the ui is not ready, and keep calling from the update method.
        RegisterPlayerInGUI(netId);
        GetPlayerColourFromGUI(netId);
        
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
        MyNetworkManager.AddPlayer(this);
        //RegisterPlayerInGUI(netid) should be in the update because of some execution issue, should use sceneloadedevent next time.
        SetCameraPos();
        //base.OnStartClient();
    }
    
    public override void OnStopClient()
    {
        MyNetworkManager.RemovePlayer(this);
        UnRegisterPlayerInGUI(netId);
        base.OnStopClient();
    }

    /// <summary>
    /// registers the player in the gui slots. only take the first slot available and breaks out
    /// empty slot is when the netid is gui is 0
    /// </summary>
    /// <param name="key"></param>
    private void RegisterPlayerInGUI(uint key)
    {
        //hacky way to avoid error when the ui is not ready, and keep calling from the update method.
        if(MyNetworkManager.Instance.MyUiManager == null)
            return;

        if (hasAddedToGui)
            return;
        
        var i = 0;
        foreach (PlayerGUIRendering render in MyNetworkManager.Instance.MyUiManager.renders)
//        foreach (PlayerGUIRendering render in FindObjectOfType<UiManager>().renders)
        {
            if (render.netId == key)
            {
                Debug.Log($"RegisterPlayerInGUI trying to register the same ID {key}");
                return;
            }
            if (render.netId == 0) //0 means empty slot
            {
                Debug.Log($"registered player {key} in the gui slots {i}");
                render.netId = key;
                Debug.Log($"player colour should be from gui {render.avatar.color}");
                hasAddedToGui = true;
                break;
            }

            i++;
        }
    }
    
    /// <summary>
    /// registers the player in the gui slots. only take the first slot available and breaks out
    /// empty slot is when the netid is gui is 0
    /// </summary>
    /// <param name="key"></param>
    private void UnRegisterPlayerInGUI(uint key)
    {
        foreach (PlayerGUIRendering render in MyNetworkManager.Instance.MyUiManager.renders)
        {
            if (render.netId == key)
            {
                Debug.Log($"unregistered player {key} in the gui slots");
                render.netId = 0; //0 means empty slot
                break;
            }
        }
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

    [Command]
    public void CmdUpdatePlayerScore(int oldValue, int newValue)
    {
        //does nothing for now
    }

    [ClientRpc]
    private void RpcUpdateGUIscore(uint key, int newValue)
    {
        //does nothing for now
    }
    
    /// <summary>
    /// updates the player health and the update all clients gui
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    [Command]
    public void CmdUpdatePlayerHealth(int oldValue, int newValue)
    {
        playerHealth = newValue;
        RpcUpdateGUIhealth(netId, playerHealth);
    }

    /// <summary>
    /// go through each gui renderer and update the gui for the player with the same net id and gui id
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newValue"></param>
    [ClientRpc]
    private void RpcUpdateGUIhealth(uint key, int newValue)
    {
        foreach (PlayerGUIRendering render in MyNetworkManager.Instance.MyUiManager.renders)
        {
            if (render.netId == key)
            {
                Debug.Log($"in update gui render {render.avatar.name} for {key} value is {newValue}");
                render.hp.text = newValue.ToString("0000");
            }
        }
    }
    
    /// <summary>
    /// updates the player health and the update all clients gui
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    //[Command]
    public void CmdUpdatePlayerColour(Color oldValue, Color newValue)
    {
        if(materialPlayer == null)
            materialPlayer = rendererPlayer.material;
        if(materialBumper == null)
            materialBumper = rendererBumper.material;

        materialPlayer.color = newValue;
        materialBumper.color = newValue;
        RpcUpdatePlayerColour(netId);
    }

    /// <summary>
    /// go through each gui renderer and update the gui for the player with the same net id and gui id
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newValue"></param>
    //[ClientRpc]
    private void RpcUpdatePlayerColour(uint key)
    {
        if(materialPlayer == null)
            materialPlayer = rendererPlayer.material;
        if(materialBumper == null)
            materialBumper = rendererBumper.material;

        materialPlayer.color = playerColour;
        materialBumper.color = playerColour;
    }
    
    private void GetPlayerColourFromGUI(uint key)
    {
        //hacky way to avoid error when the ui is not ready, and keep calling from the update method.
        if(MyNetworkManager.Instance.MyUiManager == null)
            return;

        if (hasChangedColour)
            return;
        
        foreach (PlayerGUIRendering render in MyNetworkManager.Instance.MyUiManager.renders)
        {
            if (render.netId == key)
            {
                Debug.Log($"in get player colour gui render {render.avatar.name} for {key} value is {render.avatar.color}");
                playerColour = render.avatar.color;
                hasChangedColour = true;
            }
        }
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
