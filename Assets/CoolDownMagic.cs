using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownMagic : MonoBehaviour
{
    public float lerpDuration = 2f;
    public float coolDownTime = 2f;

    private float remainingTime;

    private float timeElapsed;
    private Image image;
    public Image Image { get { return image = image ?? GetComponent<Image>(); } set { } }
    public bool isReady;

    private void Start()
    {
        Image.fillAmount = 0;
        isReady = false;
        timeElapsed = 2f;
        remainingTime = 0f;
    }

    void Update()
    {
        if (timeElapsed <= lerpDuration)
        {
            var fillAmount = Mathf.Lerp(1, 0, timeElapsed / lerpDuration);
            if (fillAmount <= 0.01) 
            {
                fillAmount = 0;
            }
            Image.fillAmount = fillAmount;
            timeElapsed += Time.deltaTime; 
        }
        
        if(remainingTime > 0) 
        {
            isReady = false;
            remainingTime -= Time.deltaTime;
        }
        else 
        {
            isReady = true;
        }
    }

    public void ActivateCooldown()
    {
        timeElapsed = 0f;
        remainingTime = coolDownTime;
    }
}
