using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkCharacterControllerPrototypeCustom : NetworkTransform
{
    [Header("Character Controller Settings")]
    public float maxSpeed = 4.0f;
    public float rotationSpeed = 15.0f;
    public float rotationOnFireSpeed = 2000f;
    public float animationBlendSpeed = 0.05f;
    public float pushInterpolationSpeed = 3.0f;

    [HideInInspector]
    public Vector3 PushDestinationPoint { get; set; }

    public TickTimer pushTimer = TickTimer.None;

    private Animator animator;

    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    /// <summary>
    /// Sets the default teleport interpolation velocity to be the CC's current velocity.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;
    /// <summary>
    /// Sets the default teleport interpolation angular velocity to be the CC's rotation speed on the Z axis.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToRotation"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

    [Networked]
    [HideInInspector]
    public Vector3 Velocity { get; set; }

    public float MovementAnimationSpeed { get; private set; }

    CharacterMagicHandler characterMagicHandler;

    public CharacterController Controller { get; private set; }

    protected override void Awake()
    {
        PushDestinationPoint = Vector3.zero;
        characterMagicHandler = GetComponent<CharacterMagicHandler>();
        Controller = GetComponent<CharacterController>();
    }

    protected override void CopyFromBufferToEngine()
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
        Controller = GetComponent<CharacterController>();
    }

    public void Move(Vector3 direction)
    {
        if(direction != Vector3.zero) 
        {
            var deltaTime = Runner.DeltaTime;
            var previousPos = transform.position;
            direction = direction.normalized;

            if (direction != default && characterMagicHandler.isFire != true)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * deltaTime);
                MovementAnimationSpeed = 1.0f;
            }

            Controller.Move(direction * maxSpeed * deltaTime);
            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        }
        else 
        {
            MovementAnimationSpeed = 0.0f;
        }
    }

    public void PlayMoveAnimation()
    {
        Animator.SetFloat(GameData.Animator.Speed,
            Mathf.Lerp(Animator.GetFloat(GameData.Animator.Speed), MovementAnimationSpeed,
            animationBlendSpeed));
    }

    public void RotateOnFire(Vector3 direction) 
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationOnFireSpeed * Runner.DeltaTime);
    }

    public void Push() 
    {
        if (!pushTimer.Expired(Runner))
        {
            transform.position = Vector3.Lerp(transform.position, PushDestinationPoint, pushInterpolationSpeed * Runner.DeltaTime);
        }
        else 
        {
            PushDestinationPoint = Vector3.zero;
        }
    }

    public void SetPushDestinationAndTime(Vector3 pushDestinationPoint, float time) 
    {
        PushDestinationPoint += pushDestinationPoint;
        PushDestinationPoint = transform.position + PushDestinationPoint;
        pushTimer = TickTimer.CreateFromSeconds(Runner, time);
    }
}