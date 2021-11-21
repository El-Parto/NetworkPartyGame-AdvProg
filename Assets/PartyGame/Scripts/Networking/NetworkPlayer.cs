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
    [SyncVar (hook = nameof(OnUpdatePlayerScore))] public int playerScore;
    // The player's health
    [SyncVar (hook = nameof(OnUpdatePlayerHealth))] public int playerHealth;
    [SyncVar (hook = nameof(OnUpdatePlayerColour))] public Color playerColour;
    [SyncVar (hook = nameof(onUpdatePlayerName))] public string playerName;
    [SyncVar(hook = nameof(OnTimerTick)), SerializeField] private int gameTimer;

    [Header("Game Settings")]
    [SyncVar(hook = nameof(OnUpdateMaxTime)), SerializeField] private int maxGameTime = 99;
    [SyncVar(hook = nameof(OnUpdateMaxHP)), SerializeField] private int maxPlayerHP = 30;
    [SyncVar(hook = nameof(OnUpdateGameModeID)), SerializeField] private int gameModeID = 1;
    [SyncVar(hook = nameof(OnUpdateMapID)), SerializeField] private int mapID = 1;
    
    public MeshRenderer rendererPlayer;
    public MeshRenderer rendererBumper;
    private Material materialPlayer;
    private Material materialBumper;

    private bool hasAddedToGui = false;
    private bool hasChangedColour = false;

    private Timer _gameTimerObject;
    public Timer GameTimerObject
    {
        get
        {
            if (_gameTimerObject == null)
            {
                _gameTimerObject = GetComponent<Timer>();
            }
            return _gameTimerObject;
        }
    }
    private Timer _ballTimer;
    public Timer ballTimer
    {
        get
        {
            if (_ballTimer == null)
            {
                _ballTimer = GameObject.Find("Network Ball Manager").GetComponent<Timer>();
            }

            return _ballTimer;
        }
    }
    
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
        base.OnStartClient();
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

    #region syncvar player score
    public void OnUpdatePlayerScore(int oldValue, int newValue)
    {
        UpdateGUIscore(netId, newValue);
    }

    [Command]
    public void CmdUpdatePlayerScore(int oldValue, int newValue)
    {
        RpcUpdateGUIscore(netId, newValue);
    }

    [ClientRpc]
    private void RpcUpdateGUIscore(uint key, int newValue)
    {
        UpdateGUIscore(netId, newValue);
    }
    
    private void UpdateGUIscore(uint key, int newValue)
    {
        //does nothing for now
    }
    #endregion
    
    #region syncvar player health
    public void OnUpdatePlayerHealth(int oldValue, int newValue)
    {
        UpdateGUIhealth(netId, newValue);
    }

    /// <summary>
    /// updates the player health and the update all clients gui
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    [Command]
    public void CmdUpdatePlayerHealth(int oldValue, int newValue)
    {
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
        UpdateGUIhealth(key, newValue);
    }
    
    private void UpdateGUIhealth(uint key, int newValue)
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
    #endregion

    #region syncvar player name
    public void onUpdatePlayerName(string oldValue, string newValue)
    {
        UpdateGUIplayerName(netId, newValue);
    }

    [Command]
    public void CmdUpdatePlayerName(string oldValue, string newValue)
    {
        RpcUpdatePlayerName(netId, newValue);
    }

    [ClientRpc]
    private void RpcUpdatePlayerName(uint key, string newValue)
    {
        UpdateGUIplayerName(key, newValue);
    }

    private void UpdateGUIplayerName(uint key, string newValue)
    {
        foreach (PlayerGUIRendering render in MyNetworkManager.Instance.MyUiManager.renders)
        {
            if (render.netId == key)
            {
                Debug.Log($"in update gui render {render.avatar.name} for {key} value is {newValue}");
                render.playerName.text = newValue;
            }
        }
    }
    #endregion
    
    #region syncvar player colour
    public void OnUpdatePlayerColour(Color oldValue, Color newValue)
    {
        UpdateGUIplayerColour(netId, newValue);
    }

    /// <summary>
    /// updates the player health and the update all clients gui
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    [Command]
    public void CmdUpdatePlayerColour(Color oldValue, Color newValue)
    {
        RpcUpdatePlayerColour(netId, newValue);
    }

    /// <summary>
    /// go through each gui renderer and update the gui for the player with the same net id and gui id
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newValue"></param>
    [ClientRpc]
    private void RpcUpdatePlayerColour(uint key, Color newValue)
    {
        UpdateGUIplayerColour(key, newValue);
    }
    
    private void UpdateGUIplayerColour(uint key, Color newValue)
    {
        if(materialPlayer == null)
            materialPlayer = rendererPlayer.material;
        if(materialBumper == null)
            materialBumper = rendererBumper.material;

        materialPlayer.color = playerColour;
        materialBumper.color = playerColour;
    }
    #endregion

    #region syncvar max time
    public void OnUpdateMaxTime(int oldValue, int newValue)
    {
        GameTimerObject.startingTime = newValue;
        ballTimer.startingTime = newValue;
        UpdateGUIMaxTime(netId, newValue);
    }

    [Command]
    public void CmdUpdateMaxTime(int oldValue, int newValue)
    {
        RpcUpdateGUIMaxTime(netId, newValue);
    }

    [ClientRpc]
    private void RpcUpdateGUIMaxTime(uint key, int newValue)
    {
        UpdateGUIMaxTime(netId, newValue);
    }
    
    private void UpdateGUIMaxTime(uint key, int newValue)
    {
        Debug.Log($"UpdateGUImaxTime netid {netId}");
        MyNetworkManager.Instance.MyUiManager.sliderMaxTime.value = (float) newValue;
    }
    #endregion

    #region syncvar max hp
    public void OnUpdateMaxHP(int oldValue, int newValue)
    {
        UpdateGUIMaxHP(netId, newValue);
    }

    [Command]
    public void CmdUpdateMaxHP(int oldValue, int newValue)
    {
        RpcUpdateGUIMaxHP(netId, newValue);
    }

    [ClientRpc]
    private void RpcUpdateGUIMaxHP(uint key, int newValue)
    {
        UpdateGUIMaxHP(netId, newValue);
    }
    
    private void UpdateGUIMaxHP(uint key, int newValue)
    {
        Debug.Log($"UpdateGUIMaxHP netid {netId}");
        MyNetworkManager.Instance.MyUiManager.sliderMaxHP.value = (float) newValue;
    }
    #endregion

    #region syncvar game mode id
    public void OnUpdateGameModeID(int oldValue, int newValue)
    {
        UpdateGUIGameModeID(netId, newValue);
    }

    [Command]
    public void CmdUpdateGameModeID(int oldValue, int newValue)
    {
        RpcUpdateGUIGameModeID(netId, newValue);
    }

    [ClientRpc]
    private void RpcUpdateGUIGameModeID(uint key, int newValue)
    {
        UpdateGUIGameModeID(netId, newValue);
    }
    
    private void UpdateGUIGameModeID(uint key, int newValue)
    {
        Debug.Log($"UpdateGUIGameModeID netid {netId}");
        if (newValue == 1)
        {
            MyNetworkManager.Instance.MyUiManager.toggleGameMode1.isOn = true;
            MyNetworkManager.Instance.MyUiManager.toggleGameMode2.isOn = false;
        }
    }
    #endregion
    
    #region syncvar map id
    public void OnUpdateMapID(int oldValue, int newValue)
    {
        UpdateGUIMapID(netId, newValue);
    }

    [Command]
    public void CmdUpdateMapID(int oldValue, int newValue)
    {
        RpcUpdateGUIMapID(netId, newValue);
    }

    [ClientRpc]
    private void RpcUpdateGUIMapID(uint key, int newValue)
    {
        UpdateGUIMapID(netId, newValue);
    }
    
    private void UpdateGUIMapID(uint key, int newValue)
    {
        Debug.Log($"UpdateGUIMapID netid {netId}");
        if (newValue == 1)
        {
            MyNetworkManager.Instance.MyUiManager.toggleMap1.isOn = true;
            MyNetworkManager.Instance.MyUiManager.toggleMap2.isOn = false;
        }
    }
    #endregion


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

    #region syncvar local hooks called from gui
    public void LocalPlayerColourChanged(Color value)
    {
        CmdUpdatePlayerColour(playerColour, value);
    }

    public void LocalPlayerNameChanged(string value)
    {
        CmdUpdatePlayerName(playerName, value);
    }

    public void LocalMapChanged(int value)
    {
        CmdUpdateMapID(mapID, value);
    }

    public void LocalGameModeChanged(int value)
    {
        CmdUpdateGameModeID(gameModeID, value);
    }

    public void LocalMaxHPChanged(int value)
    {
        CmdUpdateMaxHP(maxPlayerHP, value);
    }

    public void LocalMaxTimeChanged(int value)
    {
        CmdUpdateMaxTime(maxGameTime, value);
    }

    public void LocalGameStart(MatchSetting setting)
    {
        if (isLocalPlayer)
        {
            CmdGameStart(setting);
        }
    }
    #endregion

    #region match game start

    /// <summary>
    /// do game initialisation here, then start the timers
    /// </summary>
    [Command]
    public void CmdGameStart(MatchSetting setting)
    {
        //update settings
        maxGameTime = setting.maxTime;
        maxPlayerHP = setting.maxHP;
        gameModeID = setting.gameModeID;
        mapID = setting.mapID;
        playerName = setting.playerName;
        playerColour = setting.playerColour;
        playerHealth = maxPlayerHP;
        UpdateGUI(1);
        ServerGameStart();
    }

    private void UpdateGUI(uint key)
    {
        UpdateGUIhealth(key, playerHealth);
        UpdateGUIplayerColour(key, playerColour);
        UpdateGUIplayerName(key, playerName);
    }

    [SyncVar(hook = nameof(OnReceivedGameStarted))]
    public bool gameStarted = false;
        
    private void OnReceivedGameStarted(bool _old, bool _new)
    {
        // If you want a countdown or some sort of match starting
        // indicator, replace the contents of this function
        // with that and then call the Unload
			
        if(_new)
        {
            GameObject.FindObjectOfType<UiManager>().OnStartGame();
            CmdTimerStart();
        }
    }

    [Command]
    public void CmdTimerStart()
    {
        if (MyNetworkManager.Instance.IsHost)
        {
            GameTimerObject.Restart();
            ballTimer.Restart();
        }
    }
    
    [Server]
    public void ServerGameStart()
    {
        gameStarted = true;
    }

    private void OnTimerTick(int oldValue, int newValue)
    {
        MyNetworkManager.Instance.MyUiManager.UpdateTimerText(newValue);
    }
    
    [Command]
    public void CmdTimerTick(int value)
    {
        gameTimer = value;
    }
        
    [Command]
    public void CmdTimerDone()
    {
        Debug.Log("GAME OVER");
        //decides who wins by checking the hp of each player
        MyNetworkManager.LocalPlayer.ServerGameOver();
    }

    [Server]
    public void ServerGameOver()
    {
        string summary = "";
        foreach (var pair in MyNetworkManager.Instance.players)
        {
            summary += $"{pair.Value.playerName} remaining HP: {pair.Value.playerHealth}\n";
        }

        summary += "biggest HP wins!";
        RpcGameOver(summary);
    }

    [ClientRpc]
    public void RpcGameOver(string summmary)
    {
        MyNetworkManager.Instance.MyUiManager.OnGameOver();
        MyNetworkManager.Instance.MyUiManager.textGameOverSummary.text = summmary;
    }

    #endregion
}
