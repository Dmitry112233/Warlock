using Fusion;
using UnityEngine;

public class MovementHandler : NetworkBehaviour
{
    private CharacterControllerCustom characterControllerCustom;
    public CharacterControllerCustom CharacterControllerCustom { get { return characterControllerCustom = characterControllerCustom ?? GetComponent<CharacterControllerCustom>(); } }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            var movementDirection = new Vector3(networkInputData.mevementInput.x, 0, networkInputData.mevementInput.z);
            movementDirection.Normalize();
            CharacterControllerCustom.Move(movementDirection);
        }
        if (CharacterControllerCustom.PushDestinationPoint != Vector3.zero) 
        {
            CharacterControllerCustom.Push();
        }
    }

    public override void Render()
    {
        CharacterControllerCustom.PlayMoveAnimation();
    }
}
