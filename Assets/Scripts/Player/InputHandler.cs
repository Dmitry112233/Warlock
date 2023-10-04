using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Vector3 movementInputVector = Vector3.zero;
    private Vector3 aimInputVector = Vector3.zero;
    private Vector3 fireInputVector = Vector3.zero;
    private bool isFireBallButtonPresed = false;
    private bool isStompButtonPresed = false;

    private MovementHandler movementHandler;
    public MovementHandler MovementHandler { get { return movementHandler = movementHandler ?? GetComponent<MovementHandler>(); } set { } }

    private HpHandler hPHandler;
    public HpHandler HpHandler { get { return hPHandler = hPHandler ?? GetComponent<HpHandler>(); } set { } }

    void Start()
    {
        InputManager.Instance.NotifyMovement += Read;
        InputManager.Instance.NotifyAim += ReadAim;
        InputManager.Instance.NotifyFire += Fire;
        InputManager.Instance.NotifyLeave += Leave;
        InputManager.Instance.NotifyStomp += Stomp;
    }

    private void Read(float horizontal, float vertical)
    {
        if (MovementHandler.Object != null && HpHandler != null)
        {
            if (!MovementHandler.Object.HasInputAuthority || HpHandler.IsDead)
                return;
            movementInputVector.x = horizontal;
            movementInputVector.z = vertical;
        }
    }

    private void ReadAim(float horizontal, float vertical)
    {
        if (MovementHandler.Object != null && HpHandler != null)
        {
            if (!MovementHandler.Object.HasInputAuthority || HpHandler.IsDead)
                return;
            aimInputVector.x = horizontal;
            aimInputVector.z = vertical;
        }
    }

    private void Fire(float horizontal, float vertical)
    {
        if (MovementHandler.Object != null)
        {
            if (!MovementHandler.Object.HasInputAuthority)
                return;
            isFireBallButtonPresed = true;
            fireInputVector.x = horizontal;
            fireInputVector.z = vertical;
        }
    }

    private void Stomp()
    {
        if (MovementHandler.Object != null)
        {
            if (!MovementHandler.Object.HasInputAuthority)
                return;
            isStompButtonPresed = true;
        }
    }

    private void Leave() 
    {
        HpHandler.LeaveGameByEscape();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.movementInput = movementInputVector;
        networkInputData.aimInput = aimInputVector;
        networkInputData.fireInput = fireInputVector;
        networkInputData.isFireBallButtonPresed = isFireBallButtonPresed;
        networkInputData.isStompButtonPresed = isStompButtonPresed;

        isFireBallButtonPresed = false;
        isStompButtonPresed = false;

        return networkInputData;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.NotifyMovement -= Read;
            InputManager.Instance.NotifyFire -= Fire;
            InputManager.Instance.NotifyAim -= ReadAim;
            InputManager.Instance.NotifyStomp -= Stomp;
            InputManager.Instance.NotifyLeave -= Leave;
        }
    }

    public void UnsubscribeInputManager() 
    {
        InputManager.Instance.NotifyMovement -= Read;
        InputManager.Instance.NotifyFire -= Fire;
        InputManager.Instance.NotifyAim -= ReadAim;
        InputManager.Instance.NotifyLeave -= Leave;
        InputManager.Instance.NotifyStomp -= Stomp;
    }

    public void Subscribe()
    {
        InputManager.Instance.NotifyMovement += Read;
        InputManager.Instance.NotifyAim += ReadAim;
        InputManager.Instance.NotifyFire += Fire;
        InputManager.Instance.NotifyLeave += Leave;
        InputManager.Instance.NotifyStomp += Stomp;
    }
}
