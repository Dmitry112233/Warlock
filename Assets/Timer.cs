using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image uiFill;
    public TextMeshProUGUI uiText;

    public int Duration;
    private int remainingDuration;

    void Start()
    {
        remainingDuration = Duration;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while(remainingDuration >= 0) 
        {
            yield return new WaitForSeconds(1f);
            uiText.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";
            uiFill.fillAmount = Mathf.InverseLerp(0, Duration, remainingDuration);
            remainingDuration--;
        }
        gameObject.SetActive(false);
    }


}
