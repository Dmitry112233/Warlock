using UnityEngine;
using Fusion;
using Cinemachine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

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
            CinemachineVirtualCamera cinemachineCamera = GetComponentInChildren<CinemachineVirtualCamera>();

            if(localCamera != null) 
            {
                Debug.Log("Destroy another player camera");

                Destroy(localCamera.gameObject);
                Destroy(cinemachineCamera.gameObject);
            }
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
