using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource mainMenu;
    public AudioSource thunder;

    public GameObject shot;
    public GameObject stomp;
    public GameObject rocketExplosion;
    public GameObject hit;

    private Dictionary<string, AudioSource> audioSourcesMap;
    private Dictionary<string, GameObject> audioPrefabsMap;

    List<GameObject> audioGameObjects;

    private void Awake()
    {
        audioSourcesMap = new Dictionary<string, AudioSource> 
        {
            { GameData.Sounds.Thunder, thunder}
        };

        audioPrefabsMap = new Dictionary<string, GameObject>
        {
            { GameData.Sounds.Shot, shot},
            { GameData.Sounds.Stomp, stomp},
            { GameData.Sounds.RocketExplosion, rocketExplosion},
            { GameData.Sounds.Hit, hit},
        };

        audioGameObjects = new List<GameObject>();
    }

    private void Update()
    {
        if(audioGameObjects.Count > 0) 
        {
            for (int i = audioGameObjects.Count - 1; i >= 0; i--) 
            {
                var audio = audioGameObjects[i];
                if (!audio.GetComponent<AudioSource>().isPlaying)
                {
                    audioGameObjects.RemoveAt(i);
                    Destroy(audio);
                }
            }
        }
    }

    public void PlayAudio(string audioSourceName)
    {
        var audioSource = audioSourcesMap[audioSourceName];
        audioSource.Play();
    }

    public void Play3DAudio(Vector3 position, string audioPrefabName)
    {
        var soundObject = Instantiate(audioPrefabsMap[audioPrefabName], position, Quaternion.identity);
        audioGameObjects.Add(soundObject);
    }
}
