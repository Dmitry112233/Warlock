using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

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
                        players[i].transform.position = new Vector3(-15.79f, 0.11f, -13.25f);
                        players[i].transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else 
                    {
                        players[i].transform.position = new Vector3(16.53f, 0.11f, -13.25f);
                        players[i].transform.rotation = Quaternion.LookRotation(Vector3.left);
                    }

                    playersList.Add(players[i]);
                    StartCoroutine(UnfreezePlayers(timer.Duration + 1));
                    timer.gameObject.SetActive(true);
                }

               /* foreach(GameObject player in players) 
                {
                    player.GetComponent<CharacterControllerCustom>().Freeze();
                    player.transform.position = new Vector3(-0.05f, 0.11f, -15.37f);
                    playersList.Add(player);
                    StartCoroutine(UnfreezePlayers(timer.Duration));
                    timer.gameObject.SetActive(true);
                }*/

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
