using UnityEngine;

public class StartGameController : MonoBehaviour
{
    public bool isGameStarted;

    void Start()
    {
        isGameStarted = false;
    }

    void Update()
    {
        if (!isGameStarted) 
        {
            var players = GameObject.FindGameObjectsWithTag(GameData.Tags.Player);
            
            if(players.Length == 2) 
            {
                foreach(GameObject player in players) 
                {
                    player.transform.position = new Vector3(Random.Range(0, 10), 0, 0);
                }

                isGameStarted = true;
            } 
        }
    }
}
