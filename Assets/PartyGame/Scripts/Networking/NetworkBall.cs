using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkPartyGame.Networking
{
    public class NetworkBall : NetworkBehaviour
    {
        // The ball object
        [SerializeField] private GameObject ball;
        // Start is called before the first frame update
        void Start()
        {
            CmdSpawnEnemy();
        }
        
        [Command]
        public void CmdSpawnEnemy()
        {
            // Instantiates the ball prefab and spawns it on the server
            GameObject newBall = Instantiate(ball);
            NetworkServer.Spawn(newBall);
        }
    }
}