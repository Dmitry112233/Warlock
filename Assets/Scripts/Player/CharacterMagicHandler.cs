using Fusion;
using System.Collections;
using UnityEngine;

public class CharacterMagicHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public FireBallHandler fireBallPrefab;
    public Transform initProjectilePosition;

    TickTimer fireBallDelay = TickTimer.None;

    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    NetworkObject networkObject;

    private Animator animator;

    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    public bool isFire = false;
    private Vector3 fireVector;

    private LineTest lineTest;

    private void Awake()
    {
        fireVector = Vector3.zero;
        isFire = false;
        networkObject = GetComponent<NetworkObject>();
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        lineTest = GetComponent<LineTest>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            var aimVector = new Vector3(networkInputData.aimInput.x, 0, networkInputData.aimInput.z);

            lineTest.DrawLine(new Vector3[] { transform.position, transform.position + aimVector * 2 });

            if (networkInputData.isFireBallButtonPresed)
            {
                isFire = true;
                fireVector = networkInputData.fireInput;
            }
        }
    }

    public void FireBallShot()
    {
        fireVector.Normalize();

        Runner.Spawn(fireBallPrefab, transform.position + fireVector, Quaternion.LookRotation(fireVector), Object.InputAuthority, (runner, spawnedFireBall) =>
            {
                spawnedFireBall.GetComponent<FireBallHandler>().Fire(Object.InputAuthority, networkObject);
            });
    }

    public override void Render()
    {
        if (isFire == true)
        {
            if (fireBallDelay.ExpiredOrNotRunning(Runner))
            {
                networkCharacterControllerPrototypeCustom.RotateOnFire(fireVector);
                Debug.Log("Inside is Attack");

                Animator.SetBool("Attack", true);

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
        Animator.SetBool("Attack", false);
        isFire = false;
    }
}
