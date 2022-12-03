using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 10.0f;
    public float rotationSpeed = 5.0f;

    public float animationBlendSpeed = 0.1f;
    private float movementAnimationSpeed = 0.0f;

    private CharacterController controller;
    private Animator animator;

    public CharacterController Controller { get { return controller = controller ?? GetComponent<CharacterController>(); } }
    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }

    void Start()
    {
        InputManager.Instance.NotifyMovement += Move;
    }

    void Update()
    {

    }

    private void Move(float horizontal, float vertical)
    {
        Vector3 movementDirection = new Vector3(horizontal, 0, vertical);

        if (movementDirection != Vector3.zero) 
        {
            movementAnimationSpeed = movementDirection != Vector3.zero ? 1.0f : 0.0f;
            
            Vector3 velocity = movementDirection * movementSpeed;

            Controller.Move(velocity * Time.deltaTime);

            movementDirection.Normalize();
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            Controller.transform.rotation = Quaternion.RotateTowards(Controller.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            movementAnimationSpeed = 1.0f;
        }
        else 
        {
            movementAnimationSpeed = 0.0f;
        }

        Animator.SetFloat(GameData.Animator.Speed, Mathf.Lerp(Animator.GetFloat(GameData.Animator.Speed), movementAnimationSpeed, animationBlendSpeed));
    }
}
