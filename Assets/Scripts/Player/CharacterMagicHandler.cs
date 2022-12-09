using Fusion;
using UnityEngine;

public class CharacterMagicHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public FireBallHandler fireBallPrefab;
    public Transform initProjectilePosition;

    TickTimer fireBallDelay = TickTimer.None;

    NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireBallButtonPresed)
            {
                FireBallShot();
            }
        }
    }

    private void FireBallShot() 
    {
        if (fireBallDelay.ExpiredOrNotRunning(Runner)) 
        {
            Runner.Spawn(fireBallPrefab, initProjectilePosition.position, Quaternion.LookRotation(transform.forward), Object.InputAuthority, (runner, spawnedFireBall) =>
            {
                spawnedFireBall.GetComponent<FireBallHandler>().Fire(Object.InputAuthority, networkObject, transform);
            });
        }
    }
}
