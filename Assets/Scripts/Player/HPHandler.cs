using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> explosionsParticles;

    [Header("HP Settings")]
    public HitboxRoot hitboxRoot;
    public Slider healthSlider;

    private bool isInitialized = false;
    private const byte startingHP = 10;

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

    private NetworkObject networkObject;
    public NetworkObject NetworkObject { get { return networkObject = networkObject ?? GetComponent<NetworkObject>(); } }

    void Start()
    {
        HP = startingHP;
        IsDead = false;
        isInitialized = true;
        healthSlider.value = HP / startingHP;
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
            StartCoroutine(LeaveGame());
        }
    }

    static void OnHPChanged(Changed<HpHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");

        byte hpCurrent = changed.Behaviour.HP;

        changed.LoadOld();

        byte hpOld = changed.Behaviour.HP;

        if (hpCurrent < hpOld)
        {
            changed.Behaviour.PlayFireBallParticles();
            changed.Behaviour.healthSlider.value = (hpCurrent / (float)startingHP);
        }
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

        InputHandler.UnsubscribeInputManager();
        InputHandler.enabled = false;
        CharacterControllerCustom.Controller.enabled = false; ;
        hitboxRoot.HitboxRootActive = false;
        Animator.SetTrigger(GameData.Animator.DeathTriger);
    }

    public void LeaveGameByEscape()
    {
        //Destroy(FindObjectOfType<InputManager>());
        Runner.Despawn(NetworkObject);
        Debug.Log("PLAYER LEAVE TO MAIN MENU");
    }

    public IEnumerator LeaveGame()
    {
        yield return new WaitForSeconds(3f);

        if (Object.HasStateAuthority) 
        {
            Runner.Shutdown();
        }

        /*var networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        Destroy(networkRunnerHandler);*/
        //networkRunnerHandler.InitializeNetworkRunner(networkRunnerHandler.networkRunner, GameMode.AutoHostOrClient, "TestSession", NetAddress.Any(), SceneManager.GetSceneByName("MainMenu").buildIndex, null);
        //SceneManager.LoadScene(1);
        //var networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        //Runner.SetActiveScene(1);
        //Debug.Log("PLAYER LEAVE TO MAIN MENU");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
       // Runner.SetActiveScene(1);
    }
}
