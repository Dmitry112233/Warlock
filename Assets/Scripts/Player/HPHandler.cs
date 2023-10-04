using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HpHandler : NetworkBehaviour
{
    [Header("HP Settings")]
    public HitboxRoot hitboxRoot;
    public Slider healthSlider;

    public const byte startingHP = 100;
    public bool IsActive { get; set; }

    [Networked(OnChanged = nameof(OnHPChanged))]
    float HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool IsDead { get; set; }

    private CharacterControllerCustom characterControllerCustom;
    public CharacterControllerCustom CharacterControllerCustom { get { return characterControllerCustom = characterControllerCustom ?? GetComponent<CharacterControllerCustom>(); } }

    private InputHandler characterInputHandler;
    public InputHandler InputHandler { get { return characterInputHandler = characterInputHandler ?? GetComponent<InputHandler>(); } }

    private Animator animator;
    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    private NetworkObject networkObject;
    public NetworkObject NetworkObject { get { return networkObject = networkObject ?? GetComponent<NetworkObject>(); } }

    private RpcHandler rpcHandler;
    public RpcHandler RpcHandler { get { return rpcHandler = rpcHandler ?? GetComponent<RpcHandler>(); } }

    void Start()
    {
        IsActive = true;
        HP = startingHP;
        IsDead = false;
        healthSlider.value = HP / startingHP;
    }

    //Only called on the server
    public void OnTakeDamage(float damage)
    {
        if (IsActive) 
        {
            if (IsDead)
                return;

            HP -= damage;
            
            Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

            if (HP <= 0)
            {
                Debug.Log($"{Time.time} {transform.name} died");
                IsDead = true;
                StartCoroutine(LeaveGame());
            }
        }
    }

    static void OnHPChanged(Changed<HpHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");

        float hpCurrent = changed.Behaviour.HP;

        changed.LoadOld();

        float hpOld = changed.Behaviour.HP;

        if (hpCurrent < hpOld)
        {
            changed.Behaviour.healthSlider.value = (hpCurrent / (float)startingHP);
        }
    }

    static void OnStateChanged(Changed<HpHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.IsDead}");

        bool isDeadCurrent = changed.Behaviour.IsDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
    }

    private void OnDeath()
    {
        Debug.Log($"{Time.time} OnDeath");

        CharacterControllerCustom.Freeze();
        hitboxRoot.HitboxRootActive = false;
        Animator.SetTrigger(GameData.Animator.DeathTriger);
    }

    public void LeaveGameByEscape()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Shutdown();
        }
        Debug.Log("Player leave to main menu by escape");
    }

    public IEnumerator LeaveGame()
    {
        yield return new WaitForSeconds(1f);

        if (Object.HasStateAuthority) 
        {
            Runner.Shutdown();
        }
    }
}
