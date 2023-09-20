using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameController : MonoBehaviour
{
    public bool isGameStarted;
    public Timer timer;

    private List<GameObject> playersList;

    void Start()
    {
        isGameStarted = false;
        playersList = playersList = new List<GameObject>();
    }

    void Update()
    {
        if (!isGameStarted) 
        {
            var players = GameObject.FindGameObjectsWithTag(GameData.Tags.Player);

            if (players.Length == 2) 
            {
                Debug.Log("PLAYERS COUNT IS 2");
                
                for(int i = 0; i < players.Length; i++) 
                {
                    players[i].GetComponent<CharacterControllerCustom>().Freeze();

                    if(i == 0) 
                    {
                        players[i].transform.position = new Vector3(-10.8f, 0.5f, 0f);
                        players[i].transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else 
                    {
                        players[i].transform.position = new Vector3(10.8f, 0.5f, 0f);
                        players[i].transform.rotation = Quaternion.LookRotation(Vector3.left);
                    }

                    playersList.Add(players[i]);
                    StartCoroutine(UnfreezePlayers(timer.Duration + 1));
                    timer.gameObject.SetActive(true);
                }

                isGameStarted = true;
            }
        }
    }

    private IEnumerator UnfreezePlayers(int Duration)
    {
       yield return new WaitForSeconds(Duration);
       playersList.ForEach(x => x.GetComponent<CharacterControllerCustom>().Unfreeze());
    }
}
