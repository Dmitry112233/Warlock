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

    // Update is called once per frame
    void Update()
    {

        //movementInputVector.x = dynamicJoystick.Horizontal;
        //movementInputVector.y = dynamicJoystick.Vertical;

        /*movementInputVector.x = Input.GetAxis("Horizontal");
        movementInputVector.y = Input.GetAxis("Vertical");*/
    }

    private void Read(float horizontal, float vertical)
    {
        movementInputVector.x = horizontal;
        movementInputVector.y = vertical;
    }

    public NetworkInputData GetNetworkInput() 
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.mevementInput = movementInputVector;

        return networkInputData;
    }
}
