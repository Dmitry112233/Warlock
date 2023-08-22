using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public StartGameController startGameController;

    public float damage = 0.1f;
    public float speed = 2.5f;

    private List<int> playersId;

    private void Start()
    {
        playersId = new List<int>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (startGameController.isGameStarted && other.tag == GameData.Tags.Player)
        {
            if (other.GetComponent<HpHandler>().IsActive)
            {
                if (!other.GetComponent<HpHandler>().IsLavaInfluence && playersId.FindAll(x => x.Equals(other.gameObject.GetInstanceID())).Count == 0)
                {
                    playersId.Add(other.gameObject.GetInstanceID());
                    other.GetComponent<HpHandler>().IsLavaInfluence = true;
                }

                if (playersId.FindAll(x => x.Equals(other.gameObject.GetInstanceID())).Count == 1) 
                {
                    other.GetComponent<HpHandler>().OnTakeDamage(damage);
                    other.GetComponent<CharacterControllerCustom>().SetSpeed(speed);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (startGameController.isGameStarted && other.tag == GameData.Tags.Player)
        {
            if (other.GetComponent<HpHandler>().IsActive && playersId.FindAll(x => x.Equals(other.gameObject.GetInstanceID())).Count == 1)
            {
                other.GetComponent<CharacterControllerCustom>().SetSpeed(other.GetComponent<CharacterControllerCustom>().maxSpeed);
                playersId.Remove(other.gameObject.GetInstanceID());
                other.GetComponent<HpHandler>().IsLavaInfluence = false;
            }
        }
    }
}
