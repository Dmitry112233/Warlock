using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HPHandler : NetworkBehaviour
{
    public NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    private CharacterInputHandler characterInputHandler;
    public HitboxRoot hitboxRoot;

    private Animator animator;

    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool IsDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 100;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        characterInputHandler = GetComponent<CharacterInputHandler>();
    }

    void Start()
    {
        HP = startingHP;
        IsDead = false;

        isInitialized = true;
    }

    //Only called on the server
    public void OnTakeDamage(byte damage) 
    {
        if (IsDead)
            return;

        HP -= damage;

        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        if(HP <= 0) 
        {
            Debug.Log($"{Time.time} {transform.name} died");
            IsDead = true;
        }
    }

    static void OnHPChanged(Changed<HPHandler> changed) 
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.IsDead}");

        bool isDeadCurrent = changed.Behaviour.IsDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.IsDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
    }

    private void OnDeath() 
    {
        Debug.Log($"{Time.time} OnDeath");
        networkCharacterControllerPrototypeCustom.Controller.enabled = false; ;
        hitboxRoot.HitboxRootActive = false;
        characterInputHandler.enabled = false;
        Animator.SetBool("IsDead", true);
        StartCoroutine(ResetIsDead());

        //StartCoroutine(Leave());
    }

    IEnumerator ResetIsDead()
    {
        yield return new WaitForSeconds(1.5f);
        Animator.SetBool("IsDead", false);
    }

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(3);
        Debug.Log($"{Time.time} OnLeave");
        SceneManager.LoadScene(1);
    }
}
