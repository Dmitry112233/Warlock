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
    public float pushInterpolationSpeed = 3.0f; 

    public float gravity = -9.8f;
    private float ySpeed;

    [Networked]
    public Vector3 Velocity { get; set; }

    public Vector3 PushDestinationPoint { get; set; }
    public TickTimer pushTimer = TickTimer.None;

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
    
    private float movementAnimationSpeed;
    private float maxMovementAnimationSpeed = 1f;


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

            Debug.Log("SPEED IS:" + speed);

            Controller.Move(direction * speed * deltaTime);
            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
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
            Debug.Log("IS GROUNDED");
            ySpeed = -0.5f;
        }

        var direction = new Vector3(0, ySpeed, 0);

        Controller.Move(direction * speed * deltaTime);
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
        if (!pushTimer.Expired(Runner))
        {
            transform.position = Vector3.Lerp(transform.position, PushDestinationPoint, pushInterpolationSpeed * Runner.DeltaTime);
        }
        else
        {
            PushDestinationPoint = Vector3.zero;
            SetSpeed(maxSpeed);
            maxMovementAnimationSpeed = 1f;
        }
    }

    public void SetPushDestinationAndTime(Vector3 pushDestinationPoint, float time, float speed)
    {
        PushDestinationPoint = pushDestinationPoint;
        PushDestinationPoint = transform.position + PushDestinationPoint;
        pushTimer = TickTimer.CreateFromSeconds(Runner, time);
        SetSpeed(speed);
        maxMovementAnimationSpeed = 0.6f;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void Freeze()
    {
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