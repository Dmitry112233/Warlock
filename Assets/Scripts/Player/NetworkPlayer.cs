using UnityEngine;
using Fusion;
using Cinemachine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    private RpcHandler rpcHandler;
    public RpcHandler RpcHandler { get { return rpcHandler = rpcHandler ?? GetComponent<RpcHandler>(); } }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Camera.main.gameObject.SetActive(false);

            Debug.Log("Spawned local player");

            GameObject.FindGameObjectWithTag(GameData.Tags.StartGameController).GetComponent<StartGameController>()?.RPC_CheckIfGameStarted();
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            CinemachineVirtualCamera cinemachineCamera = GetComponentInChildren<CinemachineVirtualCamera>();

            if (localCamera != null)
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
