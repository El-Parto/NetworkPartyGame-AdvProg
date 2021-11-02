using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;


[RequireComponent((typeof(PlayerManager)))]
public class NetworkPlayer : NetworkBehaviour
{
    public void Awake()
    {
        
        
    }

    // public override void OnStartLocalPlayer()
    // {
    //     PlayerManager player = gameObject.GetComponentInChildren<PlayerManager>();
    //     player.enabled = true;
    // }

    
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
