using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanManager : Singleton<CleanManager>
{
    private List<GameObject> spawnedStompObjects;

    void Awake()
    {
        spawnedStompObjects = new List<GameObject>();
    }

    void Update()
    {
        if (spawnedStompObjects.Count > 0)
        {
            for (int i = spawnedStompObjects.Count - 1; i >= 0; i--)
            {
                var obj = spawnedStompObjects[i];
                if (!obj.GetComponentInChildren<ParticleSystem>().IsAlive())
                {
                    spawnedStompObjects.Remove(obj);
                    Destroy(obj);
                }
            }
        }
    }

    public void AddObjectToDestroy(GameObject obj) 
    {
        spawnedStompObjects.Add(obj);
    }

    public void Init() 
    {
        //Get null reference exception in case of calling CleanManager.Instance.AddObjectToDestroy(obj) from RPCHandler after stomp.
        //Need to get the instance in Start() somewhere -> RPCHandler for now. 
    }
}
