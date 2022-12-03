using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInputData)) 
        {
            Vector3 movementDirection = new Vector3(networkInputData.mevementInput.x, 0, networkInputData.mevementInput.y);
            movementDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(movementDirection * 10);
        }
    }
}
