using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    public float pushInterpolation = 30f;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            var movementDirection = new Vector3(networkInputData.mevementInput.x, 0, networkInputData.mevementInput.z);
            movementDirection.Normalize();
            networkCharacterControllerPrototypeCustom.Move(movementDirection);
        }
        if (networkCharacterControllerPrototypeCustom.PushDestinationPoint != Vector3.zero) 
        {
            networkCharacterControllerPrototypeCustom.Push();
        }
    }

    public override void Render()
    {
        networkCharacterControllerPrototypeCustom.PlayMoveAnimation();
    }
}
