using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    public float pushInterpolation = 30f;

    private LineTest lineTest;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        lineTest = GetComponent<LineTest>();
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 movementDirection = Vector3.zero;

        if (GetInput(out NetworkInputData networkInputData))
        {
            movementDirection = new Vector3(networkInputData.mevementInput.x, 0, networkInputData.mevementInput.z);
            
            if (movementDirection != Vector3.zero)
            {

                lineTest.DrawLine(new Vector3[] { transform.position, transform.position + movementDirection * 2 });
                movementDirection.Normalize();
                networkCharacterControllerPrototypeCustom.Move(movementDirection);
            }
            else
            {
                networkCharacterControllerPrototypeCustom.ResetMovementAnimationSpeed();
            }
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
