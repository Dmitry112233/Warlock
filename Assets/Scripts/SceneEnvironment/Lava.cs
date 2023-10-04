using UnityEngine;

public class Lava : MonoBehaviour
{
    public StartGameController startGameController;

    public float damage = 0.1f;
    public float speed = 2.5f;
    public float movementAnimationSpeed = 0.6f;

    private void OnTriggerStay(Collider other)
    {
        if (startGameController.isGameStarted && other.tag == GameData.Tags.Player)
        {
            if (other.GetComponent<HpHandler>().IsActive)
            {
                other.GetComponent<HpHandler>().OnTakeDamage(damage);
                other.GetComponent<CharacterControllerCustom>().SetSpeed(speed, movementAnimationSpeed);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (startGameController.isGameStarted && other.tag == GameData.Tags.Player)
        {
            other.GetComponent<CharacterControllerCustom>().ResetSpeed();
        }
    }
}
