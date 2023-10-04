using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FireBallHandler : NetworkBehaviour
{
    [Header("Fire ball settings")]
    public float liveTime = 10.0f;
    public float detectedColisionSphereRadius = 0.5f;
    public float explosionSphereRadius = 2f;
    public float rocketSpeed = 20f;
    public float pushSpeed = 12.0f;
    public float pushDuration = 1.5f;
    public float damage = 10;
    public float onDrawSpere = 0.5f;
    public float accelerateRocketSpeed = 100f;

    private float rocketSpeedInterpolate = 0f;

    [Header("Prefab")]
    public List<GameObject> explosionsParticles;

    [Header("Collision detection")]
    public Transform checkForImpactPoint;
    public LayerMask collisionLayers;

    private TickTimer maxLiveDurationTickTimer = TickTimer.None;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    private NetworkObject firedByNetworkObject;
    private PlayerRef firedByPlayerRef;

    private NetworkObject networkObject;
    public NetworkObject NetworkObject { get { return networkObject = networkObject ?? GetComponent<NetworkObject>(); } }

    public void Fire(PlayerRef firedByPlayerRef, NetworkObject firedByNetworkObject) 
    {
        this.firedByPlayerRef = firedByPlayerRef;
        this.firedByNetworkObject = firedByNetworkObject;
        maxLiveDurationTickTimer = TickTimer.CreateFromSeconds(Runner, liveTime);
    }

    public override void FixedUpdateNetwork() 
    {
        LerpRocketSpeed();

        transform.position += transform.forward * Runner.DeltaTime * rocketSpeedInterpolate;

        if (Object.HasStateAuthority) 
        {
            int hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, detectedColisionSphereRadius, firedByPlayerRef, hits, collisionLayers, HitOptions.IncludePhysX);

            if (maxLiveDurationTickTimer.Expired(Runner)) 
            {
                Runner.Despawn(NetworkObject);
                return;
            }

            if (hitCounts > 0) 
            {
                hitCounts = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, explosionSphereRadius, firedByPlayerRef, hits, collisionLayers, HitOptions.None);

                for (int i = 0; i < hitCounts; i++)
                {
                    HpHandler hPHandler = hits[i].Hitbox.transform.root.GetComponent<HpHandler>();
                    RpcHandler rpcHandler = hits[i].Hitbox.transform.root.GetComponent<RpcHandler>();
                    Transform playerTransform = hits[i].Hitbox.transform.root.GetComponent<Transform>();
                    CharacterControllerCustom characterController = hits[i].Hitbox.transform.root.GetComponent<CharacterControllerCustom>();

                    Vector3 pushVector = playerTransform.position - transform.position;
                    pushVector.Normalize();
                    pushVector.y = 0;

                    if (hPHandler != null && (hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() != firedByNetworkObject)) 
                    {
                        characterController.SetPushVectorTimeAndSpeed(pushVector, pushDuration, pushSpeed);

                        hPHandler.OnTakeDamage(damage);

                        //Fireball rpc is called from player becouse despawn is to fast to display effect on client. 
                        rpcHandler.RPC_FireBallExplosion();
                        rpcHandler.RPC_PlayHitSound();

                        Runner.Despawn(NetworkObject);
                    }
                }
            }
        }
    }

    private void LerpRocketSpeed() 
    {
        if (rocketSpeedInterpolate < rocketSpeed)
        {
            rocketSpeedInterpolate += accelerateRocketSpeed * Runner.DeltaTime;
        }
        else
        {
            rocketSpeedInterpolate = rocketSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkForImpactPoint.position, onDrawSpere);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
    }
}