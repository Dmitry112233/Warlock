using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            Vector3 movementDirection = new Vector3(networkInputData.mevementInput.x, 0, networkInputData.mevementInput.z);

            if (movementDirection != Vector3.zero)
            {
                movementDirection.Normalize();
                networkCharacterControllerPrototypeCustom.Move(movementDirection);
            }
            else
            {
                networkCharacterControllerPrototypeCustom.ResetMovementAnimationSpeed();
            }
        }
    }

    public override void Render()
    {
        networkCharacterControllerPrototypeCustom.PlayMoveAnimation();
    }
}
