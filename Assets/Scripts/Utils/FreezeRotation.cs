using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (transform.rotation != Quaternion.identity) 
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
