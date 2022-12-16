using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class HpHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> explosionsParticles;
    public HitboxRoot hitboxRoot;

    private bool isInitialized = false;
    private const byte startingHP = 100;

    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool IsDead { get; set; }

    private CharacterControllerCustom characterControllerCustom;
    public CharacterControllerCustom CharacterControllerCustom { get { return characterControllerCustom = characterControllerCustom ?? GetComponent<CharacterControllerCustom>(); } }

    private InputHandler characterInputHandler;
    public InputHandler InputHandler { get { return characterInputHandler = characterInputHandler ?? GetComponent<InputHandler>(); } }

    private Animator animator;
    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

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

        if (HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} died");
            IsDead = true;
        }
    }

    static void OnHPChanged(Changed<HpHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");

        byte hpCurrent = changed.Behaviour.HP;

        changed.LoadOld();

        byte hpOld = changed.Behaviour.HP;

        if (hpCurrent < hpOld)
            changed.Behaviour.PlayFireBallParticles();
    }

    private void PlayFireBallParticles()
    {
        foreach (GameObject particle in explosionsParticles)
        {
            Instantiate(particle, transform.position, Quaternion.identity);
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

        CharacterControllerCustom.Controller.enabled = false; ;
        hitboxRoot.HitboxRootActive = false;
        InputHandler.enabled = false;
        Animator.SetTrigger(GameData.Animator.DeathTriger);
    }
}
