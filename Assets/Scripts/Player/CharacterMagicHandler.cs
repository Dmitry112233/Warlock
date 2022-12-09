using Fusion;
using UnityEngine;

public class CharacterMagicHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public FireBallHandler fireBallPrefab;
    public Transform initProjectilePosition;

    TickTimer fireBallDelay = TickTimer.None;

    NetworkObject networkObject;

    private Animator animator;

    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

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
            if (!Animator.GetBool("IsAttack")) 
            {
                Debug.Log("IsFire == true");
                if (fireBallDelay.ExpiredOrNotRunning(Runner))
                {
                    Debug.Log("Inside is Attack");
                    Animator.SetBool("IsAttack", true);
                    fireBallDelay = TickTimer.CreateFromSeconds(Runner, 2f);
                }
            }

            isFire = false;
        }
    }

    public void FireBallAnimationEvent()
    {
        FireBallShot();
        Animator.SetBool("IsAttack", false);
    }
}
