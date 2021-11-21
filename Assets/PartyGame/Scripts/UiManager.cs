using System;
using System.Collections.Generic;
using Mirror;
using PartyGame.Scripts;
using PartyGame.Scripts.Networking;
using TeddyToolKit.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace Game.Scripts
{
    /// <summary>
    /// used to update the values on the gui
    /// </summary>
    [Serializable]
    public class PlayerGUIRendering
    {
        public Image avatar;
        public Text playerName;
        public Text hp;

        /// <summary>
        /// used to check if this netid/player id is the same
        /// may not work if we have players disconnecting and reconnecting? as the index here is hard coded on inspector
        /// </summary>
        public uint netId;
    }

    /// <summary>
    /// used to store match settings and pass it to the server to use
    /// </summary>
    [Serializable]
    public class MatchSetting
    {
        public int maxTime;
        public int maxHP;
        public int gameModeID;
        public int mapID;
        public string playerName;
        public Color playerColour;
    }
    
    public class UiManager : MonoSingleton<UiManager>
    {
        [Tooltip("the Text UI element for Timer")]
        [SerializeField]
        private Text txtTimer;
        [SerializeField]
        private Text txtNetworkStatus;
        
        [Space]
        [Tooltip("the toggle group used for the tab heads")]
        [SerializeField]
        private ToggleGroup tabGroup;
        [SerializeField]
        private Toggle tabControl;
        [SerializeField]
        private Toggle tabConnection;
        [SerializeField]
        private Toggle tabSetting;
        [SerializeField]
        private Toggle tabGameOver;
        [Tooltip("the body for each tab")]
        [SerializeField]
        private List<GameObject> tabBody;
        
        [Space]
        [SerializeField] public List<PlayerGUIRendering> renders = new List<PlayerGUIRendering>(4);

        private int activeTabIndex; 
        
        /// <summary>
        /// Drag the Menu GUI GameObject here for the UIManager to manage
        /// </summary>
        [SerializeField] 
        [Tooltip("Drag the Menu GUI GameObject here for the UIManager to manage")]
        private GameObject mainPanelGUI;
        [SerializeField]
        private GameObject topPanel;
        [SerializeField]
        private GameObject topTimerBlock;
        
        [Space]
        [SerializeField]
        public Slider sliderMaxTime;
        [SerializeField]
        public Slider sliderMaxHP;
        [SerializeField]
        public Toggle toggleGameMode1;
        [SerializeField]
        public Toggle toggleGameMode2;
        [SerializeField]
        public Toggle toggleMap1;
        [SerializeField]
        public Toggle toggleMap2;
        [SerializeField]
        public Text txtPlayerNameSet;
        [SerializeField]
        private Button btnRandomName;
        [SerializeField]
        public Image imgPlayerColourSet;
        [SerializeField]
        private Button btnRandomColour;
        [SerializeField]
        private Button buttonStartGame;
        [SerializeField]
        public Text textGameOverSummary;
        [SerializeField]
        private Button buttonQuit;
        
        /// <summary>
        /// toggles the display of menu
        /// </summary>
        public void ToggleMenu(GameObject gameObject)
        {
            var current = gameObject.activeSelf;
            gameObject.SetActive(!current);
        }
        
        /// <summary>
        /// catches keypresses related for the UI, usually the main menu
        /// </summary>
        public void UIKeyPress()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu(mainPanelGUI);
            }
        }

        private void Start()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case GameManager.OFFLINE_SCENE:
                    OnStartOffline();
                    break;
                case GameManager.ONLINE_SCENE:
                    OnStartOnline();
                    break;
                default:
                    break;
            }
            //make sure only host can change this and start game
            sliderMaxTime.interactable = MyNetworkManager.Instance.IsHost;
            sliderMaxHP.interactable = MyNetworkManager.Instance.IsHost;
            buttonStartGame.interactable = MyNetworkManager.Instance.IsHost;
            toggleGameMode1.interactable = MyNetworkManager.Instance.IsHost;
            toggleGameMode2.interactable = MyNetworkManager.Instance.IsHost;
            toggleMap1.interactable = MyNetworkManager.Instance.IsHost;
            toggleMap2.interactable = MyNetworkManager.Instance.IsHost;
            buttonStartGame.interactable = MyNetworkManager.Instance.IsHost;
            //add listener to changed values
            sliderMaxTime.onValueChanged.AddListener(OnSliderMaxTimeChanged);
            sliderMaxHP.onValueChanged.AddListener(OnSliderMaxHpChanged);
            toggleGameMode1.onValueChanged.AddListener(OnToggleGameModeChanged);
            toggleMap1.onValueChanged.AddListener(OnToggleMapChanged);
            btnRandomName.onClick.AddListener(OnButtonRandomNameClicked);
            btnRandomColour.onClick.AddListener(OnButtonRandomColourClicked);
            buttonStartGame.onClick.AddListener(OnButtonStartGameClicked);
            buttonQuit.onClick.AddListener(OnButtonQuitClicked);
        }
        
        private void Update()
        {
            UIKeyPress();
            NetworkStatus();
        }

        private void NetworkStatus()
        {
            txtNetworkStatus.text = MyNetworkManager.Instance.mode.ToString();
        }

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void OnDisable()
        {
            DeregisterListeners();
        }

        /// <summary>
        /// set up the gui layout to show what is necessary for online mode after logging in
        /// main menu off, bottom bar on, to bar on
        /// </summary>
        public void OnStartOnline()
        {
            topPanel.SetActive(true);
            topTimerBlock.SetActive(true);
            mainPanelGUI.SetActive(true);
            tabSetting.isOn = true;
        }

        /// <summary>
        /// set up the gui layout to show what is necessary for online mode after starting a game
        /// main menu off, bottom bar on with players, to bar on, timer on
        /// </summary>
        public void OnStartOffline()
        {
            topPanel.SetActive(true);
            topTimerBlock.SetActive(false);
            mainPanelGUI.SetActive(true);
            tabConnection.isOn = true;
        }
        
        public void OnStartGame()
        {
            topPanel.SetActive(true);
            topTimerBlock.SetActive(true);
            tabControl.isOn = true;
            mainPanelGUI.SetActive(false);
        }
        
        public void OnGameOver()
        {
            topPanel.SetActive(true);
            topTimerBlock.SetActive(false);
            mainPanelGUI.SetActive(true);
            tabGameOver.isOn = true;
        }
        
        #region events related

        /// <summary>
        /// listen to the event and call method when that happens
        /// </summary>
        private void RegisterListeners()
        {
            // EventManager.Instance.OnTimer1SecondTick += UpdateTimerText;
        }
        
        /// <summary>
        /// make sure to un-listen, always as a pair with the onenable
        /// </summary>
        private void DeregisterListeners()
        {
            // EventManager.Instance.OnTimer1SecondTick -= UpdateTimerText;
        }

        #endregion

        /// <summary>
        /// updates the text on the ui element
        /// </summary>
        /// <param name="value"></param>
        public void UpdateTimerText(int value)
        {
            if (txtTimer) txtTimer.text = value.ToString("00");
        }

        /// <summary>
        /// called everytime the active tab head is changed and show the corresponding body
        /// the tabs are indexed in the object name using .int from 1 and above 
        /// </summary>
        public void showSelectedTabBody()
        {
            //get the index of active tab head from the tab group and break out of loop
            var t = tabGroup.GetFirstActiveToggle();
            //split by the delimiter . in the name
            var s = t.name.Split('.');
            //then get the integer in the last element
            int.TryParse(s[(s.Length - 1)], out activeTabIndex);
            // exit if nothing is active or invalid tab
            if (activeTabIndex < 0) return;
            
            //disables all tab bodies and enable the active one
            int bodyIndex;
            foreach (var body in tabBody)
            {
                s = body.name.Split('.');
                int.TryParse(s[(s.Length - 1)], out bodyIndex);
                body.SetActive(bodyIndex == activeTabIndex);
            }
        }

        public MatchSetting GetMatchSetting()
        {
            var setting = new MatchSetting();
            setting.maxTime = (int)sliderMaxTime.value;
            setting.maxHP = (int)sliderMaxHP.value;
            setting.gameModeID = toggleGameMode1.isOn ? 1 : 2;
            setting.mapID = toggleMap1.isOn ? 1 : 2;
            setting.playerName = txtPlayerNameSet.text;
            setting.playerColour = imgPlayerColourSet.color;
            return setting;
        }
        
        public void OnButtonStartGameClicked()
        {
            MyNetworkManager.LocalPlayer.LocalGameStart(GetMatchSetting());
        }
        
        public void OnButtonQuitClicked()
        {
            Application.Quit();
        }
        
        public void OnSliderMaxTimeChanged(float value)
        {
            MyNetworkManager.LocalPlayer.LocalMaxTimeChanged((int) value);
        }

        public void OnSliderMaxHpChanged(float value)
        {
            MyNetworkManager.LocalPlayer.LocalMaxHPChanged((int) value);
        }

        public void OnToggleGameModeChanged(bool value)
        {
            int gameModeID = toggleGameMode1.isOn ? 1 : 2;
            MyNetworkManager.LocalPlayer.LocalGameModeChanged(gameModeID);
        }
        
        public void OnToggleMapChanged(bool value)
        {
            int mapID = toggleMap1.isOn ? 1 : 2;
            MyNetworkManager.LocalPlayer.LocalMapChanged(mapID);
        }

        public void OnButtonRandomNameClicked()
        {
            var first = new string[5];
            var second = new string[5];
            first[0] = "Bright";
            first[1] = "Suss";
            first[2] = "Clever";
            first[3] = "Dupe";
            first[4] = "Express";
            second[0] = "Alien";
            second[1] = "Baby";
            second[2] = "Cat";
            second[3] = "Star";
            second[4] = "Killer";
            var i = UnityEngine.Random.Range(0, 4);
            var j = UnityEngine.Random.Range(0, 4);
            var value = $"{first[i]}{second[j]}";
            //make sure it is different from the previous one
            if (txtPlayerNameSet.text == value) value += "X";
            txtPlayerNameSet.text = value;
            MyNetworkManager.LocalPlayer.LocalPlayerNameChanged(value);
        }

        public void OnButtonRandomColourClicked()
        {
            var first = new Color[3];
            first[0] = Color.red;
            first[1] = Color.green;
            first[2] = Color.blue;
            var i = UnityEngine.Random.Range(0, 3);
            var j = UnityEngine.Random.Range(0, 3);
            var value = first[i] + first[j];
            //make sure it is different from the previous one
            if (imgPlayerColourSet.color == value) value += Color.gray;
            imgPlayerColourSet.color = value;
            MyNetworkManager.LocalPlayer.LocalPlayerColourChanged(value);
        }
    }
}