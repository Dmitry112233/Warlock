using Fusion;
using UnityEngine;

public class FreezeRotation : NetworkBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    public override void FixedUpdateNetwork()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }
}
