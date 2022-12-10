using Fusion;
using System.Collections;
using UnityEngine;

public class CharacterMagicHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public FireBallHandler fireBallPrefab;
    public Transform initProjectilePosition;

    TickTimer fireBallDelay = TickTimer.None;

    NetworkObject networkObject;

    private NetworkMecanimAnimator animator;

    public NetworkMecanimAnimator Animator { get { return animator = animator ?? GetComponent<NetworkMecanimAnimator>(); } }

    private bool isFire = false;

    private void Awake()
    {
        isFire = false;
        networkObject = GetComponent<NetworkObject>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireBallButtonPresed)
            {
                isFire = true;
            }
        }
    }

    public void FireBallShot()
    {
        Runner.Spawn(fireBallPrefab, initProjectilePosition.position, Quaternion.LookRotation(transform.forward), Object.InputAuthority, (runner, spawnedFireBall) =>
            {
                spawnedFireBall.GetComponent<FireBallHandler>().Fire(Object.InputAuthority, networkObject, transform);
            });
    }

    public override void Render()
    {
        if (isFire == true)
        {
            isFire = false;

            if (fireBallDelay.ExpiredOrNotRunning(Runner))
            {
                Debug.Log("Inside is Attack");

                Animator.SetTrigger("Attack");

                StartCoroutine(FireCora());
                fireBallDelay = TickTimer.CreateFromSeconds(Runner, 2f);
            }
        }
    }

    IEnumerator FireCora()
    {
        yield return new WaitForSeconds(0.5f);
        FireBallShot();
    }

    public void FireBallAnimationEvent()
    {
        //FireBallShot();
    }
}
