using UnityEngine;

public class FlashLight : MonoBehaviour
{
    private bool isLastFlashIntensityLow;
    public new Light light;


    private float timeRemaining;
    private float minIntensity;
    private float maxIntensity;

    void Start()
    {
        timeRemaining = 0f;
        minIntensity = 0f;
        maxIntensity = 0f;

        isLastFlashIntensityLow = false;
        light = GetComponent<Light>();
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (!isLastFlashIntensityLow)
            {
                light.intensity = Random.Range(minIntensity, maxIntensity);
                isLastFlashIntensityLow = true;
            }
            else
            {
                light.intensity = 0.9f;
                isLastFlashIntensityLow = false;
            }
        }
        else
        {
            light.intensity = 1;
        }
    }

    public void DoFlashLight(float time, float minIntensity, float maxIntensity) 
    {
        timeRemaining += time;
        this.minIntensity = minIntensity;
        this.maxIntensity = maxIntensity;
    }
}
