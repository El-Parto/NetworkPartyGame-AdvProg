using Mirror;

using NetworkPartyGame.Physics;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBump : NetworkBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // what should be the bumper mechanism, it was on ball previously but to make it work for networking
    // might be better to let the bumper handle it.
    // When the bumper colliders with a gameobject with the tag ball, get the collision's gameobject
    // then get the Ball component script and pass it as a temp var'
    // then increase the ball's speed by a multiplier
    [ServerCallback]
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if(ball.speed <= 20)
                ball.speed *= 1.4f;
        }
    }
}
