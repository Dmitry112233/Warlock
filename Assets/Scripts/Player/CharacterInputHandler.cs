using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector3 movementInputVector = Vector3.zero;
    bool isFireBallButtonPresed = false;
    CharacterMovementHandler characterMovementHandler;
    HPHandler hPHandler;

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hPHandler = GetComponent<HPHandler>();
    }

    void Start()
    {
        InputManager.Instance.NotifyMovement += Read;
        InputManager.Instance.NotifyFire += Fire;
    }

    private void Read(float horizontal, float vertical)
    {
        if (!characterMovementHandler.Object.HasInputAuthority || hPHandler.IsDead)
            return;

        movementInputVector.x = horizontal;
        movementInputVector.z = vertical;
    }

    private void Fire()
    {
        if (!characterMovementHandler.Object.HasInputAuthority)
            return;

        isFireBallButtonPresed = true;
    }

    public NetworkInputData GetNetworkInput() 
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.mevementInput = movementInputVector;
        networkInputData.isFireBallButtonPresed = isFireBallButtonPresed;

        isFireBallButtonPresed = false;

        return networkInputData;
    }
    private void OnDestroy()
    {
        if(InputManager.Instance != null) 
        {
            InputManager.Instance.NotifyMovement -= Read;
            InputManager.Instance.NotifyFire -= Fire;
        }
    }
}
