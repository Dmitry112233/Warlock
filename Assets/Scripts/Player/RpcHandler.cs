using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> explosionsParticles;

    public void OnTakeFireBall()
    {
        RPC_PlayFireballParticles(transform.position);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayFireballParticles(Vector3 playPosition)
    {
        foreach (GameObject particle in explosionsParticles)
        {
            Instantiate(particle, playPosition, Quaternion.identity);
        }
    }
}
