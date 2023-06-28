using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage = 0.1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == GameData.Tags.Player)
        {
            if (other.GetComponent<HpHandler>().IsActive)
            {
                other.GetComponent<HpHandler>().OnTakeDamage(damage);
                other.GetComponent<CharacterControllerCustom>().SetSpeed(3f);

            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<HpHandler>().IsActive) 
        {
            if (other.tag == GameData.Tags.Player)
            {
                other.GetComponent<CharacterControllerCustom>().SetSpeed(4f);
            }
        }
    }
}
