using System.Collections;
using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;

    public float speed = 1;
    public Transform trailTransform;

    public Material skyBoxMaterialDefault;
    public Material skyBoxMaterialFlash;

    private float defaultIntensity;
    private Color defaultLightColor;
    private Color defaultSkyColor;
    private Color defaultFroundColor;

    public void Start()
    {
        transform.position = startPosition;

        defaultIntensity = RenderSettings.ambientIntensity;
        defaultLightColor = RenderSettings.ambientLight = Color.white;
        defaultSkyColor = RenderSettings.ambientSkyColor = Color.white;
        defaultFroundColor = RenderSettings.ambientGroundColor = Color.white;
    }

    void Update()
    {
        if((endPosition - transform.position).magnitude > 0.5f) 
        {
            var resultVector = endPosition - transform.position;
            transform.position += resultVector.normalized * speed * Time.deltaTime;
            trailTransform.rotation = Quaternion.LookRotation(resultVector * -1);
        }
        else 
        {
            Debug.Log("RESET METEOR POSITION");

            RenderSettings.skybox = skyBoxMaterialFlash;
            RenderSettings.ambientIntensity = 2f;/*
            RenderSettings.ambientLight = Color.white;
            RenderSettings.ambientSkyColor = Color.white;
            RenderSettings.ambientGroundColor = Color.white;*/

            transform.position = startPosition;

            StartCoroutine(ResetLight(0.1f));

            RenderSettings.skybox = skyBoxMaterialFlash;
            RenderSettings.ambientIntensity = 2f;


            StartCoroutine(ResetLight(0.3f));
        }
    }

    IEnumerator ResetLight(float time)
    {
        yield return new WaitForSeconds(time);

        RenderSettings.skybox = skyBoxMaterialDefault;
        RenderSettings.ambientIntensity = defaultIntensity;
        RenderSettings.ambientLight = defaultLightColor;
        RenderSettings.ambientSkyColor = defaultSkyColor;
        RenderSettings.ambientGroundColor = defaultFroundColor;
    }
}
