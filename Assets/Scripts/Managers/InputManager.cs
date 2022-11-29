using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public delegate void HorizontalHandler(float horizontal, float vertical);

    public event HorizontalHandler NotifyMovement;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        NotifyMovement?.Invoke(Horizontal, Vertical);

        /*if (Horizontal != 0 || Vertical != 0)
        {
            NotifyMovement?.Invoke(Horizontal, Vertical);
        }*/
    }
}
