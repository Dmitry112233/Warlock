using Fusion;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (transform != null && _camera != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - _camera.transform.position), 0.2f);
        }
    }
}
