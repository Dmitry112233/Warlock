using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    Debug.Log($"Instatiate singletone {instance.GetType()}");
                    //DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }
}
