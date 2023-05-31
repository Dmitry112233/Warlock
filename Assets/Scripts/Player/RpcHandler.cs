using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> explosionsParticles;
    public GameObject stompPrefab;

    public void OnTakeFireBall()
    {
        RPC_PlayFireballParticles(transform.position);
    }

    public void OnStomp()
    {
        RPC_PlayStomp(transform.position);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayFireballParticles(Vector3 playPosition)
    {
        foreach (GameObject particle in explosionsParticles)
        {
            Instantiate(particle, playPosition, Quaternion.identity);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayStomp(Vector3 playPosition)
    {
        Debug.Log("STOMP EFFECT INSTATIATED");
        Instantiate(stompPrefab, playPosition, Quaternion.identity);
    }
}
