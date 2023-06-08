using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public float timeRemaining = 0;
    private bool isLastFlashIntensityLow;
    public new Light light;

    public float minIntensity = 0f;
    public float maxIntensity = 0f;

    void Start()
    {
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
