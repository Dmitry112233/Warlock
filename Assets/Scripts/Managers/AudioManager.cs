using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource mainMenu;
    public AudioSource thunder;

    public GameObject shot;
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
        var audioSource = audioMap[audioSourceName];
        audioSource.Play();
    }

    public void Play3DAudio(Vector3 position, string initialAudioSourceName)
    {
        var soundObject = Instantiate(shot, position, Quaternion.identity);
        //soundObject.GetComponent<AudioSource>().Play();
        audioGameObjects.Add(soundObject);
    }
}
