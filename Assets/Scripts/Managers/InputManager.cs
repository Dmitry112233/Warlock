using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private DynamicJoystick dynamicJoystick;
    private FixedJoystickCustom fireJoystick;

    public delegate void HorizontalHandler(float horizontal, float vertical);
    public delegate void FireHandler(float horizontal, float vertical);
    public event HorizontalHandler NotifyMovement;
    public event HorizontalHandler NotifyAim;
    public event FireHandler NotifyFire;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public float HorizontalAimLine { get; private set; }
    public float VerticalAimLine { get; private set; }

    private void Start()
    {
        dynamicJoystick = GameObject.FindGameObjectWithTag("Dynamic Joystick").GetComponent<DynamicJoystick>();
        fireJoystick = GameObject.FindGameObjectWithTag("Fire Joystick").GetComponent<FixedJoystickCustom>();
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

        HorizontalAimLine = fireJoystick.Horizontal;
        VerticalAimLine = fireJoystick.Vertical;

        NotifyAim?.Invoke(HorizontalAimLine, VerticalAimLine);

        if (fireJoystick.isFire == true)
        {
            NotifyFire?.Invoke(fireJoystick.HorizontalOnUp, fireJoystick.VrticallOnUp);
            fireJoystick.isFire = false;
        }

        /*if (Input.GetKeyDown(KeyCode.F))
            NotifyFire?.Invoke(HorizontalFire, VerticalFire);*/
    }
}
