using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystickCustom : Joystick
{
    public float HorizontalOnUp = 0.0f;
    public float VerticalOnUp = 0.0f;

    public override void OnPointerUp(PointerEventData eventData)
    {
        HorizontalOnUp = Horizontal;
        VerticalOnUp = Vertical;

        StartCoroutine(ResetValues());
        base.OnPointerUp(eventData);
    }

    IEnumerator ResetValues()
    {
        yield return new WaitForSeconds(0.2f);
        HorizontalOnUp = 0.0f;
        VerticalOnUp = 0.0f;
    }
}