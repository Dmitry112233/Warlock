using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class FireBallHandler : NetworkBehaviour
{
    [Header("Prefab")]
    public List<GameObject> explosionsParticles;

    [Header("Collision detection")]
    public Transform checkForImpactPoint;
    public LayerMask collisionLayers;

    TickTimer maxLiveDurationTickTimer = TickTimer.None;

    public int rocketSpeed = 20;
    public float pushBooster = 2f;

    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    NetworkObject firedByNetworkObject;
    PlayerRef firedByPlayerRef;

    NetworkObject networkObject;

    public float OnDrawSpere = 0.5f;

    public void Fire(PlayerRef firedByPlayerRef, NetworkObject firedByNetworkObject) 
    {
        this.firedByPlayerRef = firedByPlayerRef;
        this.firedByNetworkObject = firedByNetworkObject;
        networkObject = GetComponent<NetworkObject>();

        maxLiveDurationTickTimer = TickTimer.CreateFromSeconds(Runner, 10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkForImpactPoint.position, OnDrawSpere);
    }

    public override void FixedUpdateNetwork() 
    {
        transform.position += transform.forward * Runner.DeltaTime * rocketSpeed;

        if (Object.HasStateAuthority) 
        {
            int hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 0.5f, firedByPlayerRef, hits, collisionLayers, HitOptions.IncludePhysX);

            if (maxLiveDurationTickTimer.Expired(Runner)) 
            {
                Runner.Despawn(networkObject);
                return;
            }

            if (hitCounts > 0) 
            {
                hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 2f, firedByPlayerRef, hits, collisionLayers, HitOptions.None);

                for (int i = 0; i < hitCounts; i++)
                {
                    HpHandler hPHandler = hits[i].Hitbox.transform.root.GetComponent<HpHandler>();
                    Transform playerTransform = hits[i].Hitbox.transform.root.GetComponent<Transform>();
                    CharacterControllerCustom characterController = hits[i].Hitbox.transform.root.GetComponent<CharacterControllerCustom>();

                    Vector3 pushVector = playerTransform.position - transform.position;
                    pushVector.Normalize();
                    pushVector.y = 0;

                    if (hPHandler != null && (hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() != firedByNetworkObject)) 
                    {
                        hPHandler.OnTakeDamage(10);
                        characterController.SetPushDestinationAndTime(pushVector * pushBooster, 1.5f);
                    }
                }

                Runner.Despawn(networkObject);
            }
        }
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {

    }
}