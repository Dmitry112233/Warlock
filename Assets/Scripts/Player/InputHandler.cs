using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Vector3 movementInputVector = Vector3.zero;
    private Vector3 aimInputVector = Vector3.zero;
    private Vector3 fireInputVector = Vector3.zero;
    private bool isFireBallButtonPresed = false;

    private MovementHandler movementHandler;
    public MovementHandler MovementHandler { get { return movementHandler = movementHandler ?? GetComponent<MovementHandler>(); } }

    private HpHandler hPHandler;
    public HpHandler HpHandler { get { return hPHandler = hPHandler ?? GetComponent<HpHandler>(); } }

    void Start()
    {
        InputManager.Instance.NotifyMovement += Read;
        InputManager.Instance.NotifyAim += ReadAim;
        InputManager.Instance.NotifyFire += Fire;
    }

    private void Read(float horizontal, float vertical)
    {
        if (!MovementHandler.Object.HasInputAuthority || HpHandler.IsDead)
            return;

        movementInputVector.x = horizontal;
        movementInputVector.z = vertical;
    }

    private void ReadAim(float horizontal, float vertical)
    {
        if (!MovementHandler.Object.HasInputAuthority || HpHandler.IsDead)
            return;

        aimInputVector.x = horizontal;
        aimInputVector.z = vertical;
    }

    private void Fire(float horizontal, float vertical)
    {
        if (!MovementHandler.Object.HasInputAuthority)
            return;

        isFireBallButtonPresed = true;
        fireInputVector.x = horizontal;
        fireInputVector.z = vertical;
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.mevementInput = movementInputVector;
        networkInputData.aimInput = aimInputVector;
        networkInputData.fireInput = fireInputVector;
        networkInputData.isFireBallButtonPresed = isFireBallButtonPresed;

        isFireBallButtonPresed = false;

        return networkInputData;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.NotifyMovement -= Read;
            InputManager.Instance.NotifyFire -= Fire;
        }
    }
}
