using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using PartyGame.Scripts.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace PartyGame.Scripts
{
    /// <summary>
    /// handles the network connection menu, attached to the ui panel tab body for connection menu
    /// </summary>
    public class ConnectionMenu : MonoBehaviour
    {
        /// <summary>
        /// stores the list of discovered server, populated by the
        /// </summary>
        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        public MyNetworkDiscovery myNetworkDiscovery;
        
        [Space]
        [SerializeField] private Button btnStartHost;
        [SerializeField] private Button btnStartServer;
        [SerializeField] private Button btnConnectLocalhost;
        [SerializeField] private Button btnStopServer;
        [SerializeField] private Button btnStopClient;
        [SerializeField] private Button btnDiscoverServers;
        [SerializeField] private Button btnDebug;
        [Tooltip("drag the disabled button template ip inside the scrollview, as a template for new connection buttons")]
        [SerializeField] private Button buttonTemplateIP;
        [SerializeField] private InputField txtAddress;

        /// <summary>
        /// stores the list of buttons so we can clear it later on refresh
        /// </summary>
        private Dictionary<long, Button> buttonIPs = new Dictionary<long, Button>();
        
#if UNITY_EDITOR
        /// <summary>
        /// called only in unity editor scene, not in runtime
        /// </summary>
        void OnValidate()
        {
            if (myNetworkDiscovery == null)
            {
                Debug.Log($"Connection Menu Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                myNetworkDiscovery = GetComponent<MyNetworkDiscovery>();
                MyNetworkManager.Instance.myNetworkDiscovery = myNetworkDiscovery;
                UnityEditor.Events.UnityEventTools.AddPersistentListener(myNetworkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new Object[] { this, myNetworkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif
        /// <summary>
        /// when the start host button is clicked in the menu
        /// </summary>
        public void ButtonStartHost()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            MyNetworkManager.Instance.StartHost();
            //if scene has changed to online here, so this line below doesn't get executed and server is not advertised
            // so we need to advertise also in the custom network manager OnStartHost
            myNetworkDiscovery.AdvertiseServer();
        }
        
        /// <summary>
        /// when the start server button is clicked in the menu
        /// </summary>
        public void ButtonStartServer()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            MyNetworkManager.Instance.StartServer();
            //if scene has changed to online here, so this line below doesn't get executed and server is not advertised
            // so we need to advertise also in the custom network manager OnStartHost
            myNetworkDiscovery.AdvertiseServer();
        }
                
        public void ButtonConnectLocalhost()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            //make the default value localhost
            if (txtAddress.text == "") txtAddress.text = "localhost";
            MyNetworkManager.Instance.networkAddress = txtAddress.text;
            MyNetworkManager.Instance.StartClient();
        }
        
        /// <summary>
        /// when the stop host button is clicked in the menu
        /// </summary>
        public void ButtonStopServer()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            MyNetworkManager.Instance.StopHost();
        }
        
        /// <summary>
        /// when the logoff button is clicked in the menu
        /// </summary>
        public void ButtonStopClient()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            MyNetworkManager.Instance.StopClient();
            MyNetworkManager.Instance.OnStartServer();
        }
        
        /// <summary>
        /// when the discover server button is clicked in the menu, also called at the start when this menu is displayed
        /// </summary>
        public void ButtonDiscoverServers()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            myNetworkDiscovery.StartDiscovery();
        }
        
        public void ButtonDebug()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Debug.Log($"discoveredServers {discoveredServers}, count {discoveredServers.Count}");
            PopulateServerList();
        }

        /// <summary>
        /// clears then populates the server list (scroll view panel) when the event discovered server is fired
        /// </summary>
        public void PopulateServerList()
        {
            // Debug.Log($"Called {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            // servers
            // scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);
            ClearServerList();
            foreach (ServerResponse info in discoveredServers.Values)
            {
                var ipaddress = info.EndPoint.Address.ToString();
                Debug.Log(ipaddress);
                var button = Instantiate(buttonTemplateIP, buttonTemplateIP.transform.parent);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<Text>().text = ipaddress;
                buttonIPs.Add(info.serverId, button);
            }
        }

        /// <summary>
        /// clears the server/game list
        /// </summary>
        public void ClearServerList()
        {
            foreach (var pair in buttonIPs)
            {
                Destroy(pair.Value.gameObject);
            }
            buttonIPs.Clear();
        }
        
        public void Connect(ServerResponse info)
        {
            myNetworkDiscovery.StopDiscovery();
            MyNetworkManager.Instance.StartClient(info.uri);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
            PopulateServerList();
            Debug.Log($"Called {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        
        // Start is called before the first frame update
        void Start()
        {
            //automatically start to discover servers
            ButtonDiscoverServers();
        }

        private void OnEnable()
        {
            Debug.Log($"Called {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            RegisterListeners();
        }

        private void RegisterListeners()
        {
            //myNetworkDiscovery = MyNetworkManager.Instance.GetComponent<MyNetworkDiscovery>();
            MyNetworkManager.Instance.myNetworkDiscovery = myNetworkDiscovery;
            myNetworkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
            Debug.Log($"register listener networkDiscovery {myNetworkDiscovery} {myNetworkDiscovery.OnServerFound}");
            btnStartHost.onClick.AddListener(ButtonStartHost);
            btnStartServer.onClick.AddListener(ButtonStartServer);
            btnConnectLocalhost.onClick.AddListener(ButtonConnectLocalhost);
            btnStopServer.onClick.AddListener(ButtonStopServer);
            btnStopClient.onClick.AddListener(ButtonStopClient);
            btnDiscoverServers.onClick.AddListener(ButtonDiscoverServers);
            btnDebug.onClick.AddListener(ButtonDebug);
        }

        // Update is called once per frame
        void Update()
        {
            //PopulateServerList();
        }
    }
}
