using TeddyToolKit.Core;
using UnityEngine;

namespace PartyGame.Scripts
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region for c# events
        private void OnEnable()
        {
            RegisterListeners();
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
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
