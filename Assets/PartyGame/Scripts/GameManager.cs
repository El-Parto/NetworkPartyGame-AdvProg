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

        public GameObject map1;
        public GameObject map2;

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
