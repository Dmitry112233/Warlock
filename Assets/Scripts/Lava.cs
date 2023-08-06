using UnityEngine;

public class Lava : MonoBehaviour
{
    public StartGameController startGameController;

    public float damage = 0.1f;
    public float speed = 2.5f;

    private void OnTriggerStay(Collider other)
    {
        if (startGameController.isGameStarted && other.tag == GameData.Tags.Player)
        {

            Debug.Log("STAY ON LAVA");
            if (other.GetComponent<HpHandler>().IsActive)
            {
                other.GetComponent<HpHandler>().OnTakeDamage(damage);
                other.GetComponent<CharacterControllerCustom>().SetSpeed(speed);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (startGameController.isGameStarted && other.tag == GameData.Tags.Player)
        {

            Debug.Log("EXIT ON LAVA");
            if (other.GetComponent<HpHandler>().IsActive)
            {
                other.GetComponent<CharacterControllerCustom>().SetSpeed(other.GetComponent<CharacterControllerCustom>().maxSpeed);
            }
        }
    }
}
