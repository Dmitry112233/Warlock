using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> explosionsParticles;
    public GameObject stompPrefab;

    [Rpc]
    public void RPC_PlayShotSound()
    {
        AudioManager.Instance.Play3DAudio(transform.position, GameData.Sounds.Shot);
    }

    [Rpc]
    public void RPC_PlayHitSound()
    {
        AudioManager.Instance.Play3DAudio(transform.position, GameData.Sounds.Hit);
    }

    [Rpc]
    public void RPC_FireBallExplosion()
    {
        foreach (GameObject particle in explosionsParticles)
        {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
        AudioManager.Instance.Play3DAudio(transform.position, GameData.Sounds.RocketExplosion);
    }

    [Rpc]
    public void RPC_OnStomp()
    {
        var obj = Instantiate(stompPrefab, transform.position, Quaternion.identity);

        if(CleanManager.Instance != null) 
        {
            CleanManager.Instance.AddObjectToDestroy(obj);
        }
        AudioManager.Instance.Play3DAudio(transform.position, GameData.Sounds.Stomp);
    }
}
