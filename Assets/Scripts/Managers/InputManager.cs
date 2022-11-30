using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private DynamicJoystick dynamicJoystick;

    public delegate void HorizontalHandler(float horizontal, float vertical);
    public event HorizontalHandler NotifyMovement;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    private void Start()
    {
        dynamicJoystick = GameObject.FindGameObjectWithTag("Dynamic Joystick").GetComponent<DynamicJoystick>();
    }

    void Update()
    {
        //Keyboard
        //Horizontal = Input.GetAxis("Horizontal");
        //Vertical = Input.GetAxis("Vertical");

        //Mobile Controll
        Horizontal = dynamicJoystick.Horizontal;
        Vertical = dynamicJoystick.Vertical;


        NotifyMovement?.Invoke(Horizontal, Vertical);
    }
}
