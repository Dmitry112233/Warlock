using UnityEngine;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
{
    private DynamicJoystick dynamicJoystick;
    private FixedJoystickCustom fireJoystick;
    private Button stompButton;


    public delegate void HorizontalHandler(float horizontal, float vertical);
    public delegate void FireHandler(float horizontal, float vertical);
    public delegate void StompHandler();
    public delegate void LeaveHandler();
    public event HorizontalHandler NotifyMovement;
    public event HorizontalHandler NotifyAim;
    public event FireHandler NotifyFire;
    public event StompHandler NotifyStomp;

    public event LeaveHandler NotifyLeave;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public float HorizontalAimLine { get; private set; }
    public float VerticalAimLine { get; private set; }

    private bool isStompButtonClicked;

    private void Start()
    {
        dynamicJoystick = GameObject.FindGameObjectWithTag(GameData.JoystickTags.DynamicJoystick).GetComponent<DynamicJoystick>();
        fireJoystick = GameObject.FindGameObjectWithTag(GameData.JoystickTags.FireJoystick).GetComponent<FixedJoystickCustom>();
        stompButton = GameObject.FindGameObjectWithTag(GameData.JoystickTags.StompButton).GetComponent<Button>();

        isStompButtonClicked = false;

        if (stompButton != null)
        {
            stompButton.onClick.AddListener(() =>
            {
                NotifyStomp?.Invoke();
            });
        }
    }

    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Escape))
            NotifyLeave?.Invoke();
    }
}
