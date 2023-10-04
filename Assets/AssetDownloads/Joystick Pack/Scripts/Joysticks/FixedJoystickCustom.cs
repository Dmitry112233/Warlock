using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystickCustom : Joystick
{
    public float HorizontalOnUp = 0.0f;
    public float VrticallOnUp = 0.0f;
    public bool isFire = false;

    [SerializeField] private CoolDownMagic coolDownMagic;

    public override void OnPointerUp(PointerEventData eventData)
    {
        HorizontalOnUp = Horizontal;
        VrticallOnUp = Vertical;
        if (Math.Abs(HorizontalOnUp) >= 0.1 || Math.Abs(VrticallOnUp) >= 0.1 && coolDownMagic.isReady) 
        {
            isFire = true;
        }
        base.OnPointerUp(eventData);
    }
}