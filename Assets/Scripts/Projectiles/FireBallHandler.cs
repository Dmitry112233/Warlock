using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FireBallHandler : NetworkBehaviour, IPredictedSpawnBehaviour
{
    [Header("Prefab")]
    public List<GameObject> explosionsPrefabs;

    [Header("Collision detection")]
    public Transform checkForImpactPoint;
    public LayerMask collisionLayers;

    TickTimer maxLiveDurationTickTimer = TickTimer.None;

    public int rocketSpeed = 20;

    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    NetworkObject firedByNetworkObject;
    PlayerRef firedByPlayerRef;

    NetworkObject networkObject;

    public void Fire(PlayerRef firedByPlayerRef, NetworkObject firedByNetworkObject, Transform playerTrnsform) 
    {
        this.firedByPlayerRef = firedByPlayerRef;
        this.firedByNetworkObject = firedByNetworkObject;
        networkObject = GetComponent<NetworkObject>();

        maxLiveDurationTickTimer = TickTimer.CreateFromSeconds(Runner, 10);
    }

    public override void FixedUpdateNetwork() 
    {
        transform.position += transform.forward * Runner.DeltaTime * rocketSpeed;

        if (Object.HasStateAuthority) 
        {
            if (maxLiveDurationTickTimer.Expired(Runner)) 
            {
                Runner.Despawn(networkObject);
                return;
            }

            int hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 0.5f, firedByPlayerRef, hits, collisionLayers, HitOptions.IncludePhysX);

            bool isValidHit = false;

            if (hitCounts > 0)
                isValidHit = true;

            for(int i = 0; i < hitCounts; i++) 
            {
                if(hits[i].Hitbox != null) 
                {
                    if (hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() == firedByNetworkObject)
                        isValidHit = false;
                }
            }

            if (isValidHit) 
            {
                hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 1.5f, firedByPlayerRef, hits, collisionLayers, HitOptions.None);

                for (int i = 0; i < hitCounts; i++)
                {
                    HPHandler hPHandler = hits[i].Hitbox.transform.root.GetComponent<HPHandler>();

                    if(hPHandler != null) 
                    {
                        hPHandler.OnTakeDamage(10);
                    }
                }

                Runner.Despawn(networkObject);
            }
        }
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        foreach (GameObject particle in explosionsPrefabs) 
        {
            Instantiate(particle, checkForImpactPoint.position, Quaternion.identity);
        }
    }

    public void PredictedSpawnSpawned()
    {
      
    }

    public void PredictedSpawnUpdate()
    {
        transform.position += transform.forward * Runner.DeltaTime * rocketSpeed;

        if (Object.HasStateAuthority)
        {
            if (maxLiveDurationTickTimer.Expired(Runner))
            {
                Runner.Despawn(networkObject);
                return;
            }

            int hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 0.5f, firedByPlayerRef, hits, collisionLayers, HitOptions.IncludePhysX);

            bool isValidHit = false;

            if (hitCounts > 0)
                isValidHit = true;

            for (int i = 0; i < hitCounts; i++)
            {
                if (hits[i].Hitbox != null)
                {
                    if (hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() == firedByNetworkObject)
                        isValidHit = false;
                }
            }

            if (isValidHit)
            {
                hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 1.5f, firedByPlayerRef, hits, collisionLayers, HitOptions.None);

                for (int i = 0; i < hitCounts; i++)
                {
                    HPHandler hPHandler = hits[i].Hitbox.transform.root.GetComponent<HPHandler>();

                    if (hPHandler != null)
                    {
                        hPHandler.OnTakeDamage(10);
                    }
                }

                Runner.Despawn(networkObject);
            }
        }
    }

    public void PredictedSpawnRender()
    {
        throw new System.NotImplementedException();
    }

    public void PredictedSpawnFailed()
    {
        Runner.Despawn(Object, true);
    }

    public void PredictedSpawnSuccess()
    {
        throw new System.NotImplementedException();
    }
}