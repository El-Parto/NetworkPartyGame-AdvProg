using System.Collections;
using System.Collections.Generic;
using Mirror;

using NetworkPartyGame.Physics;

using System;

using UnityEngine;

namespace NetworkPartyGame.Networking
{
    //[RequireComponent(typeof(Ball))]
    // I'm not sure, but i don't think this one is CNTROLLING the ball so to speak, but handles the spawning of the ball.
    
    //spawn manager for the ball 
    public class NetworkBall : NetworkBehaviour
    {
        // need a synced timer so it periodically spawns balls.
        [SyncVar] public float timer = 5;
        // need a synced bool so that it knows when to spawn the ball.
        [SyncVar] public bool canSpawn;
        
        
        // The ball object
        [SerializeField] private GameObject ball;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void Update()
        {
            //timer is now in a separate script
            //Timer();
        }

        [Server]
        public void Timer()
        {
            timer -= 1 * Time.deltaTime;
            
            if(timer <= 0) // if the timer has expired, reset and canspawn ball
            {

                timer = 5;
                canSpawn = true;


            }
            

            if(canSpawn)
            {
                CmdSpawnBall();
                canSpawn = false;

            }

        }
        
        
        [Server]
        private void CmdSpawnBall()
        {
            // Instantiates the ball prefab and spawns it on the server
            GameObject newBall = Instantiate(ball);
            NetworkServer.Spawn(newBall);
        }
        
        //add your ontriggers and stuff here
        
        
        // for now, just using an on collision to destroy the ball 
        
        
    }
}