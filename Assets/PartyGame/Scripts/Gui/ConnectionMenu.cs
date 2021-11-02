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
        public MyNetworkDiscovery networkDiscovery;
        [Tooltip("drag the disabled button template ip inside the scrollview, as a template for new connection buttons")]
        [Space] [SerializeField] private Button buttonTemplateIP;
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
            if (networkDiscovery == null)
            {
                Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                networkDiscovery = GetComponent<MyNetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
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
            NetworkManager.singleton.StartHost();
            //if scene has changed to online here, so this line below doesn't get executed and server is not advertised
            // so we need to advertise also in the custom network manager OnStartHost
            networkDiscovery.AdvertiseServer();
        }
        
        /// <summary>
        /// when the start server button is clicked in the menu
        /// </summary>
        public void ButtonStartServer()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            NetworkManager.singleton.StartServer();
            //if scene has changed to online here, so this line below doesn't get executed and server is not advertised
            // so we need to advertise also in the custom network manager OnStartHost
            networkDiscovery.AdvertiseServer();
        }
        
        /// <summary>
        /// when the stop host button is clicked in the menu
        /// </summary>
        public void ButtonStopServer()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            NetworkManager.singleton.StopHost();
        }
        
        /// <summary>
        /// when the logoff button is clicked in the menu
        /// </summary>
        public void ButtonStopClient()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            NetworkManager.singleton.StopClient();
            NetworkManager.singleton.OnStartServer();
        }
        
        /// <summary>
        /// when the discover server button is clicked in the menu, also called at the start when this menu is displayed
        /// </summary>
        public void ButtonDiscoverServers()
        {
            Debug.Log($"Clicked {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
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
            networkDiscovery.StopDiscovery();
            NetworkManager.singleton.StartClient(info.uri);
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

        // Update is called once per frame
        void Update()
        {
            //PopulateServerList();
        }
    }
}
