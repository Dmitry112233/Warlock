using Fusion;
using UnityEngine;

public class MovementHandler : NetworkBehaviour
{
    private CharacterControllerCustom characterControllerCustom;
    public CharacterControllerCustom CharacterControllerCustom { get { return characterControllerCustom = characterControllerCustom ?? GetComponent<CharacterControllerCustom>(); } }

    private MagicHandler magicHandler;
    public MagicHandler MagicHandler { get { return magicHandler = magicHandler ?? GetComponent<MagicHandler>(); } }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if(MagicHandler.IsStomp != true && MagicHandler.IsFire != true) 
            {
                var movementDirection = new Vector3(networkInputData.movementInput.x, 0, networkInputData.movementInput.z);
                movementDirection.Normalize();
                CharacterControllerCustom.Move(movementDirection);
            }
            CharacterControllerCustom.Gravity();
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
