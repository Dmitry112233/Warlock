using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHandler : NetworkBehaviour
{
    [Header("Magic settings")]
    public float fireBallCooldown = 2.0f;
    public float stompCooldown = 3.0f;
    public float stompDelay = 0.5f;
    public float aimVectorBooster = 2f;

    public float stompBooster = 3.0f;
    public float stompDuration = 3f;
    public float stompDamage = 10;
    public float ownStompDamage = 7;
    public float detectedColisionStompSphereRadius = 2.5f;
    public LayerMask collisionLayers;

    [Header("Prefabs")]
    public FireBallHandler fireBallPrefab;
    public Transform initProjectilePosition;

    private Vector3 fireVector;
    private TickTimer fireBallCooldownTimer = TickTimer.None;
    private TickTimer stompCooldownTimer = TickTimer.None;

    public bool IsFire { get; private set; }
    public bool IsStomp { get; private set; }

    private CharacterControllerCustom characterControllerCustom;
    public CharacterControllerCustom CharacterControllerCustom { get { return characterControllerCustom = characterControllerCustom ?? GetComponent<CharacterControllerCustom>(); } }

    private InputHandler inputHandler;
    public InputHandler InputHandler { get { return inputHandler = inputHandler ?? GetComponent<InputHandler>(); } }

    private RpcHandler rpcHandler;
    public RpcHandler RpcHandler { get { return rpcHandler = rpcHandler ?? GetComponent<RpcHandler>(); } }

    private NetworkObject networkObject;
    public NetworkObject NetworkObject { get { return networkObject = networkObject ?? GetComponent<NetworkObject>(); } }

    private AimLineRender aimLineRender;
    public AimLineRender AimLineRender { get { return aimLineRender = aimLineRender ?? GetComponent<AimLineRender>(); } }

    private Animator animator;
    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    private HpHandler hpHandler;
    public HpHandler HpHandler { get { return hpHandler = hpHandler ?? GetComponent<HpHandler>(); } }

    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    private CoolDownMagic coolDownFireBall;
    private CoolDownMagic coolDownStomp;

    private void Awake()
    {
        fireVector = Vector3.zero;
        IsFire = false;
        IsStomp = false;
    }

    private void Start()
    {
        coolDownFireBall = GameObject.FindGameObjectWithTag(GameData.JoystickTags.CoolFireBall).GetComponent<CoolDownMagic>();
        coolDownStomp = GameObject.FindGameObjectWithTag(GameData.JoystickTags.CoolDownStomp).GetComponent<CoolDownMagic>();
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

            if (networkInputData.isStompButtonPresed)
            {
                IsStomp = true;

                if (Object.HasInputAuthority) 
                {
                    coolDownStomp.ActivateCooldown();
                }
            }

            if (networkInputData.isFireBallButtonPresed && IsStomp != true)
            {
                IsFire = true;
                fireVector = networkInputData.fireInput;

                CharacterControllerCustom.RotateOnFire(fireVector);
                FireBallShot();

                if (Object.HasInputAuthority)
                {
                    coolDownFireBall.ActivateCooldown();
                }
            }
        }
    }

    public void FireBallShot()
    {
        if (fireBallCooldownTimer.ExpiredOrNotRunning(Runner))
        {
            fireVector.Normalize();

            Runner.Spawn(fireBallPrefab, initProjectilePosition.position, Quaternion.LookRotation(fireVector), Object.InputAuthority, (runner, spawnedFireBall) =>
                {
                    spawnedFireBall.GetComponent<FireBallHandler>().Fire(Object.InputAuthority, NetworkObject);
                });

            if (Object.HasInputAuthority)
            {
                RpcHandler.OnShot();
            }

            fireBallCooldownTimer = TickTimer.CreateFromSeconds(Runner, fireBallCooldown);
        }
    }

    private void Stomp()
    {
        if (stompCooldownTimer.ExpiredOrNotRunning(Runner))
        {
            int hitCounts = Runner.LagCompensation.OverlapSphere(transform.position, detectedColisionStompSphereRadius, Object.InputAuthority, hits, collisionLayers, HitOptions.IncludePhysX);

            if (hitCounts > 0)
            {
                Debug.Log("Inside is Stomp HITS");

                for (int i = 0; i < hitCounts; i++)
                {
                    HpHandler hPHandler = hits[i].Hitbox.transform.root.GetComponent<HpHandler>();
                    Transform playerTransform = hits[i].Hitbox.transform.root.GetComponent<Transform>();
                    CharacterControllerCustom characterController = hits[i].Hitbox.transform.root.GetComponent<CharacterControllerCustom>();
                    RpcHandler rpcHandler = hits[i].Hitbox.transform.root.GetComponent<RpcHandler>();

                    Vector3 pushVector = playerTransform.position - transform.position;
                    pushVector.y = 0;

                    if (hPHandler != null && (hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() != NetworkObject))
                    {
                        hPHandler.OnTakeDamage(stompDamage);
                        rpcHandler.OnTakenHit();

                        //customize final stomp vector depends on distance
                        var calculatedBoosterAndDuration = CalculateBoosterAndDurationDependsOnDistance(pushVector);
                        Debug.Log("STOOMP BOOSTER: " + calculatedBoosterAndDuration.booster);

                        pushVector.Normalize();

                        characterController.SetPushDestinationAndTime(pushVector * calculatedBoosterAndDuration.booster, calculatedBoosterAndDuration.duration, 0f);
                    }
                }

                stompCooldownTimer = TickTimer.CreateFromSeconds(Runner, stompCooldown);
            }

            if (Object.HasInputAuthority)
            {
                Debug.Log("PLAY RPC");
                RpcHandler.OnStomp();
            }

            HpHandler.OnTakeDamage(ownStompDamage);
        }
    }

    public override void Render()
    {
        if (IsStomp == true)
        {
            if (stompCooldownTimer.ExpiredOrNotRunning(Runner))
            {
                Debug.Log("Inside is ANIMATION STOMP");
                Animator.SetBool(GameData.Animator.StompBool, true);

                //Stomp take too much time if run before animation and it starts with a big delay 
                StartCoroutine(StompCora());
            }
        }

        if (IsFire == true)
        {
            Animator.SetBool(GameData.Animator.AttackBool, true);
        }
    }

    IEnumerator StompCora()
    {
        yield return new WaitForSeconds(stompDelay);
        Stomp();
    }

    public void FireBallAnimationEvent()
    {
        Animator.SetBool(GameData.Animator.AttackBool, false);
        IsFire = false;
    }

    public void StompAnimationEvent()
    {
        Animator.SetBool(GameData.Animator.StompBool, false);
        IsStomp = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectedColisionStompSphereRadius);
    }

    private (float booster, float duration) CalculateBoosterAndDurationDependsOnDistance(Vector3 pushVector)
    {
        if (pushVector.magnitude <= 1.6f)
        {
            return (stompBooster * 3f, 1f);
        }
        else if (pushVector.magnitude <= 2.3f)
        {
            return (stompBooster * 2f, 0.6f);
        }
        else
        {
            return (stompBooster * 1.5f, 0.5f);
        }
    }
}
