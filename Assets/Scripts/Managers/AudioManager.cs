using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource mainMenu;
    public AudioSource thunder;

    public AudioSource shot;
    public AudioSource stomp;
    public AudioSource rocketExplosion;
    public AudioSource hit;

    private Dictionary<string, AudioSource> audioMap;
    List<GameObject> audioGameObjects;

    private void Awake()
    {
        audioMap = new Dictionary<string, AudioSource> 
        {
            { GameData.Sounds.Thunder, thunder}
        };

        audioGameObjects = new List<GameObject>();
    }

    private void Update()
    {
        if(audioGameObjects.Count > 0) 
        {
            for (int i = audioGameObjects.Count - 1; i >= 0; i--) 
            {
                var go = audioGameObjects[i];
                if (!go.GetComponent<AudioSource>().isPlaying)
                {
                    audioGameObjects.RemoveAt(i);
                    Destroy(go);
                }
            }
        }
    }

    public void PlayAudio(string audioSourceName)
    {
        var audioSource = audioMap[audioSourceName];
        audioSource.Play();
    }

    public void Play3DAudio(Transform transform, string initialAudioSourceName)
    {
        var go = new GameObject("Game Object for sound");
        go.transform.position = transform.position;

        go.transform.SetParent(this.transform);

        var audioSource = go.AddComponent<AudioSource>();
        //audioSource.GetCopyOf(audioMap[initialAudioSourceName]);
        audioSource.Play();

        audioGameObjects.Add(go);
    }
}