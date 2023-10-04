using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
    public float RotateSpeed = 1.2f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }
}
