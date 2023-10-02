using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHandler : NetworkBehaviour
{
    [Header("Magic settings")]
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
        coolDownFireBall = GameObject.FindGameObjectWithTag(GameData.JoystickTags.CoolDownFireBall).GetComponent<CoolDownMagic>();
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

                //Stomp take too much time if run before animation and it starts with a big delay 
                StartCoroutine(StompCora());

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
        fireVector.Normalize();

        Runner.Spawn(fireBallPrefab, initProjectilePosition.position, Quaternion.LookRotation(fireVector), Object.InputAuthority, (runner, spawnedFireBall) =>
            {
                spawnedFireBall.GetComponent<FireBallHandler>().Fire(Object.InputAuthority, NetworkObject);
            });

        if (Object.HasInputAuthority)
        {
            RpcHandler.RPC_PlayShotSound();
        }
    }

    private void Stomp()
    {
        if (Object.HasStateAuthority)
        {
            RpcHandler.RPC_OnStomp();

            int hitCounts = Runner.LagCompensation.OverlapSphere(transform.position, detectedColisionStompSphereRadius, Object.InputAuthority, hits, collisionLayers, HitOptions.IncludePhysX);

            if (hitCounts > 0)
            {
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
                        rpcHandler.RPC_PlayHitSound();

                        //customize final stomp vector depends on distance
                        var calculatedBoosterAndDuration = CalculateSpeedAndDurationDependsOnDistance(pushVector);

                        pushVector.Normalize();
                        characterController.SetPushVectorTimeAndSpeed(pushVector, calculatedBoosterAndDuration.duration, calculatedBoosterAndDuration.speed);
                    }
                }
            }

            HpHandler.OnTakeDamage(ownStompDamage);
        }
    }

    public override void Render()
    {
        if (IsStomp == true)
        {
            Animator.SetBool(GameData.Animator.StompBool, true);
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

    private (float speed, float duration) CalculateSpeedAndDurationDependsOnDistance(Vector3 pushVector)
    {
        if (pushVector.magnitude <= 1.5f)
        {
            return (15f, 1f);
        }
        else
        {
            return (10f, 1f);
        }
    }
}
