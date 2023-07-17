using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameData;

public class MeteorMovement : MonoBehaviour
{
    public List<Vector3> startPositions;
    public List<Vector3> endPositions;

    public Vector3 endPosition;

    public float speed = 1;
    public Transform trailTransform;

    public FlashLight flashLight;

    public float time = 0.25f;
    public float minFlashIntension = 1.8f;
    public float maxFLashIntension = 2f;

    public void Start()
    {
        transform.position = startPositions[Random.Range(0, startPositions.Count)];
        endPosition = endPositions[Random.Range(0, endPositions.Count)];
    }

    void Update()
    {
        if((endPosition - transform.position).magnitude > 1f) 
        {
            var resultVector = endPosition - transform.position;
            transform.position += resultVector.normalized * speed * Time.deltaTime;
            trailTransform.rotation = Quaternion.LookRotation(resultVector * -1);
        }
        else 
        {
            AudioManager.Instance.PlayAudio(GameData.Sounds.Thunder);
            flashLight.DoFlashLight(time, minFlashIntension, maxFLashIntension);
            transform.position = startPositions[Random.Range(0, startPositions.Count)];
            endPosition = endPositions[Random.Range(0, endPositions.Count)];
        }
    }
}
