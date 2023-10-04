using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerCustom : NetworkTransform
{
    [Header("Character Controller Settings")]
    public float speed = 4.0f;
    public float maxSpeed = 4.0f;
    public float rotationSpeed = 15.0f;
    public float rotationOnFireSpeed = 2000f;
    public float animationBlendSpeed = 0.05f;
    public float pushSlowDownBooster = 12f;
    public float gravity = -9.8f;

    private float ySpeed;
    private float movementAnimationSpeed;
    private float maxMovementAnimationSpeed = 1f;

    [Networked]
    public Vector3 Velocity { get; set; }

    public bool IsPushed { get; set; }
    private Vector3 PushDestinationPoint { get; set; }
    private float PushSpeed { get; set; }
    private TickTimer PushTimer = TickTimer.None;

    private MagicHandler magicHandler;
    private MagicHandler MagicHandler { get { return magicHandler = magicHandler ?? GetComponent<MagicHandler>(); } }

    private CharacterController controller;
    public CharacterController Controller { get { return controller = controller ?? GetComponent<CharacterController>(); } }

    private InputHandler characterInputHandler;
    public InputHandler InputHandler { get { return characterInputHandler = characterInputHandler ?? GetComponent<InputHandler>(); } }

    private MovementHandler movementHandler;
    public MovementHandler MovementHandler { get { return movementHandler = movementHandler ?? GetComponent<MovementHandler>(); } }

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

    protected override void Awake()
    {
        PushDestinationPoint = Vector3.zero;
    }

    protected override void CopyFromBufferToEngine()
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
    }

    public void Move(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            var deltaTime = Runner.DeltaTime;
            var previousPos = transform.position;

            direction = direction.normalized;

            if (direction != default && MagicHandler.IsFire != true)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * deltaTime);
                movementAnimationSpeed = maxMovementAnimationSpeed;
            }

            if (!IsPushed) 
            {
                Controller.Move(direction * speed * deltaTime);
                Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
            }
        }
        else
        {
            movementAnimationSpeed = 0.0f;
        }
    }

    public void Gravity()
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;

        ySpeed += gravity * deltaTime;

        if (Controller.isGrounded)
        {
            ySpeed = -0.5f;
        }

        var direction = new Vector3(0, ySpeed, 0);

        Controller.Move(direction * speed * deltaTime);
        Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
    }

    public void PlayMoveAnimation()
    {
        Animator.SetFloat(GameData.Animator.Speed,
            Mathf.Lerp(Animator.GetFloat(GameData.Animator.Speed), movementAnimationSpeed,
            animationBlendSpeed));
    }

    public void RotateOnFire(Vector3 direction)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationOnFireSpeed * Runner.DeltaTime);
    }

    public void Push()
    {
        if (!PushTimer.ExpiredOrNotRunning(Runner))
        {
            var previousPos = transform.position;

            Controller.Move(PushDestinationPoint * Runner.DeltaTime * PushSpeed);

            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;

            if (PushSpeed > 0f) 
            {
                PushSpeed -= pushSlowDownBooster * Runner.DeltaTime;
            }
        }
        else
        {
            IsPushed = false;
            PushSpeed = 0f;
            maxMovementAnimationSpeed = 1f;
        }
    }

    public void SetPushVectorTimeAndSpeed(Vector3 pushVector, float time, float speed)
    {
        IsPushed = true;

        PushDestinationPoint = pushVector;
        PushDestinationPoint.Normalize();
        
        PushTimer = TickTimer.CreateFromSeconds(Runner, time);
        
        PushSpeed = speed;
        maxMovementAnimationSpeed = 0.5f;
    }

    public void SetSpeed(float speed, float maxMovementAnimationSpeed)
    {
        this.speed = speed;
        this.maxMovementAnimationSpeed = maxMovementAnimationSpeed;
    }

    public void ResetSpeed()
    {
        this.speed = maxSpeed;
        this.maxMovementAnimationSpeed = 1f;
    }

    public void Freeze()
    {
        Animator.SetFloat(GameData.Animator.Speed, 0);

        InputHandler.UnsubscribeInputManager();
        InputHandler.enabled = false;
        Controller.enabled = false;
        MagicHandler.enabled = false;
        MovementHandler.enabled = false;
    }

    public void Unfreeze()
    {
        InputHandler.Subscribe();
        InputHandler.enabled = true;
        Controller.enabled = true;
        MagicHandler.enabled = true;
        MovementHandler.enabled = true;
    }
}