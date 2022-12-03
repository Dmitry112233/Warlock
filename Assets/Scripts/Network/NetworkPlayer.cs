using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority) 
        {
            Local = this;

            Camera.main.gameObject.SetActive(false);

            Debug.Log("Spawned local player");
        }
        else 
        {
            Camera localCamera = GetComponentInChildren<Camera>();

            if(localCamera != null) 
            {
                Debug.Log("SWITCHED OFF CAMERA 2 ");
                localCamera.gameObject.SetActive(false);
            }

            AudioListener audioListener = GetComponentInChildren<AudioListener>();

            if (audioListener != null)
            {
                Debug.Log("SWITCHED OFF AUDIO 2 ");
                audioListener.gameObject.SetActive(false);
            }
            
            Debug.Log("Spawned remote player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority) 
        {
            Runner.Despawn(Object);
        }
    }
}
