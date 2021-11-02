using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent((typeof(PlayerManager)))]
public class NetworkPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnStartClient()
    {
        PlayerManager player = gameObject.GetComponent<PlayerManager>();
        player.enabled = isLocalPlayer;
    }
    
}
