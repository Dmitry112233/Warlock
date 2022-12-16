using Fusion;
using System.Collections;
using UnityEngine;

public class MagicHandler : NetworkBehaviour
{
    [Header("Magic settings")]
    public float fireBallCooldown = 2.0f;
    public float fireBallTimeBeforAppearence = 0.5f;
    public float aimVectorBooster = 2f;

    [Header("Prefabs")]
    public FireBallHandler fireBallPrefab;
    public Transform initProjectilePosition;

    private Vector3 fireVector;
    private TickTimer fireBallDelay = TickTimer.None;

    public bool IsFire { get; private set; }

    private CharacterControllerCustom characterControllerCustom; 
    public CharacterControllerCustom CharacterControllerCustom { get { return characterControllerCustom = characterControllerCustom ?? GetComponent<CharacterControllerCustom>(); } }

    private NetworkObject networkObject;
    public NetworkObject NetworkObject { get { return networkObject = networkObject ?? GetComponent<NetworkObject>(); } }

    private AimLineRender aimLineRender;
    public AimLineRender AimLineRender { get { return aimLineRender = aimLineRender ?? GetComponent<AimLineRender>(); } }

    private Animator animator;
    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    private void Awake()
    {
        fireVector = Vector3.zero;
        IsFire = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            var aimVector = new Vector3(networkInputData.aimInput.x, 0, networkInputData.aimInput.z);
            aimVector.Normalize();

            if (Object.HasInputAuthority) 
            {
                AimLineRender.SetUpLine(new Vector3[] { transform.position, transform.position + aimVector * aimVectorBooster });
            }

            if (networkInputData.isFireBallButtonPresed)
            {
                IsFire = true;
                fireVector = networkInputData.fireInput;
            }
        }
    }

    public void FireBallShot()
    {
        fireVector.Normalize();

        Runner.Spawn(fireBallPrefab, transform.position + fireVector, Quaternion.LookRotation(fireVector), Object.InputAuthority, (runner, spawnedFireBall) =>
            {
                spawnedFireBall.GetComponent<FireBallHandler>().Fire(Object.InputAuthority, NetworkObject);
            });
    }

    public override void Render()
    {
        if (IsFire == true)
        {
            if (fireBallDelay.ExpiredOrNotRunning(Runner))
            {
                CharacterControllerCustom.RotateOnFire(fireVector);

                Debug.Log("Inside is Attack");

                Animator.SetBool(GameData.Animator.AttackBool, true);

                StartCoroutine(FireCora());
                fireBallDelay = TickTimer.CreateFromSeconds(Runner, fireBallCooldown);
            }
        }
    }

    IEnumerator FireCora()
    {
        yield return new WaitForSeconds(fireBallTimeBeforAppearence);
        FireBallShot();
    }

    public void FireBallAnimationEvent()
    {
        Animator.SetBool(GameData.Animator.AttackBool, false);
        IsFire = false;
    }
}
