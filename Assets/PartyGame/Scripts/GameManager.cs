using System;
using Game.Scripts;
using TeddyToolKit.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PartyGame.Scripts
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region constant definitions

        public const string MAP1_SCENE = "Map 1";
        public const string MAP2_SCENE = "Map 2";
        public const string OFFLINE_SCENE = "Offline";
        public const string ONLINE_SCENE = "Online";
        public const string GUI_SCENE = "GUI";
        
        #endregion

        /// <summary>
        /// this is to contain the main menu gui, but because this is in a different scene, it will be set in code using the public getter
        /// </summary>
        private UiManager _uiManager;
        /// <summary>
        /// this is to prevent null on the game object, because unity game object cannot accurately be compared with == null
        /// until a property is used, and thats why we need the try catch to check the count of renders element
        /// otherwise for other objects, using the object?.property syntax would be easier.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public UiManager MyUiManager
        {
            get
            {
                if (_uiManager != null)
                {
                    return _uiManager;
                }
                else
                {
                    //initialise the GUI
                    var guiObjects = SceneManager.GetSceneByName(GameManager.GUI_SCENE).GetRootGameObjects();
                    Debug.Log($"guiObjects to check {guiObjects.Length}");
                    bool hasGui = false;
                    foreach (var go in guiObjects)
                    {
                        _uiManager = go.GetComponent<UiManager>();
                        try
                        {
                            var testnull = _uiManager.renders.Count;
                            //if no exception after this line, then all good
                            hasGui = true;
                            break;
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }

                    //if after looping but still no gui then fatal error
                    if (!hasGui)
                    {
                        throw new NullReferenceException("need gui to continue, but not found");
                    }

                    return _uiManager;
                }
            }
        }
        
        #region for c# events
        private void OnEnable()
        {
            RegisterListeners();
            FlagAsPersistant();
            LoadLocalScene(GameManager.GUI_SCENE);
        }

        private void OnDisable()
        {
            DeregisterListeners();
        }
    
        /// <summary>
        /// listen to the event and call method when that happens
        /// </summary>
        private void RegisterListeners()
        {
            // EventManager.Instance.OnTimerDone += GameOver;
        }
        
        /// <summary>
        /// make sure to un-listen, always as a pair with the onenable
        /// </summary>
        private void DeregisterListeners()
        {
            // EventManager.Instance.OnTimerDone -= GameOver;
        }
	#endregion
        
        //julian added
	    public GameObject ballPrefab;
	    /// <summary>
	    /// julian added
	    /// </summary>
        public void SpawnBall()
	    {
	        Instantiate(ballPrefab);
	    }

        public void LoadLocalScene(string scenename)
        {
            SceneManager.LoadScene(scenename, LoadSceneMode.Additive);
        }

        /// <summary>
        /// what happens when the gameover event occurs
        /// </summary>
        public void GameOver()
        {
            Debug.Log("Game Over");
        }
    
        // Start is called before the first frame update
        void Start()
        {
            // var s = SceneManager.GetSceneByName(GUI_SCENE);
            // Debug.Log($"start loaded local gui scene with {s.GetRootGameObjects().Length} objects");
            // //s.GetRootGameObjects()
            // Debug.Log($"start loaded {MyUiManager.renders.Count}");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
