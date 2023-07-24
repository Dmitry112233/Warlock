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
        RPC_PlayRockerExplosionSound(transform.position);
    }

    public void OnTakenHit()
    {
        RPC_PlayHitSound(transform.position);
    }

    public void OnStomp()
    {
        RPC_PlayStomp(transform.position);
        RPC_PlayStompSound(transform.position);
    }

    public void OnShot()
    {
        RPC_PlayShotSound(transform.position);
    }

    [Rpc]
    public void RPC_PlayShotSound(Vector3 playPosition)
    {
        AudioManager.Instance.Play3DAudio(playPosition, GameData.Sounds.Shot);
    }

    [Rpc]
    public void RPC_PlayStompSound(Vector3 playPosition)
    {
        AudioManager.Instance.Play3DAudio(playPosition, GameData.Sounds.Stomp);
    }

    [Rpc]
    public void RPC_PlayRockerExplosionSound(Vector3 playPosition)
    {
        AudioManager.Instance.Play3DAudio(playPosition, GameData.Sounds.RocketExplosion);
    }

    [Rpc]
    public void RPC_PlayHitSound(Vector3 playPosition)
    {
        AudioManager.Instance.Play3DAudio(playPosition, GameData.Sounds.Hit);
    }

    [Rpc]
    public void RPC_PlayFireballParticles(Vector3 playPosition)
    {
        foreach (GameObject particle in explosionsParticles)
        {
            Instantiate(particle, playPosition, Quaternion.identity);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_PlayStomp(Vector3 playPosition)
    {
        Debug.Log("STOMP EFFECT INSTATIATED");
        Instantiate(stompPrefab, playPosition, Quaternion.identity);
    }
}
