using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    private Animator animator;

    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

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

            if (movementDirection != Vector3.zero)
            {
                movementDirection.Normalize();
                networkCharacterControllerPrototypeCustom.Move(movementDirection * 10);
                networkCharacterControllerPrototypeCustom.MovementAnimationSpeed = 1.0f;
            }
            else 
            {
                networkCharacterControllerPrototypeCustom.MovementAnimationSpeed = 0.0f;
            }
            //Animator.SetFloat(GameData.Animator.Speed, Mathf.Lerp(Animator.GetFloat(GameData.Animator.Speed), movementAnimationSpeed, animationBlendSpeed));
        }
    }

    public override void Render()
    {
        Animator.SetFloat(GameData.Animator.Speed,
            Mathf.Lerp(Animator.GetFloat(GameData.Animator.Speed), networkCharacterControllerPrototypeCustom.MovementAnimationSpeed,
            networkCharacterControllerPrototypeCustom.AnimationBlendSpeed));
    }
}
