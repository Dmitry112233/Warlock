using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector3 movementInputVector = Vector3.zero;

    void Start()
    {
        InputManager.Instance.NotifyMovement += Read;
    }

    private void Read(float horizontal, float vertical)
    {
        movementInputVector.x = horizontal;
        movementInputVector.z = vertical;
    }

    public NetworkInputData GetNetworkInput() 
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.mevementInput = movementInputVector;

        return networkInputData;
    }
}
