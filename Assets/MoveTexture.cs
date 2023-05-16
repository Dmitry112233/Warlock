using UnityEngine;

public class MoveTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;
    public float directionX = -1f;
    public float directionY = -1f;

    private new Renderer renderer;
    public Renderer Renderer { get { return renderer = renderer ?? GetComponent<Renderer>(); } }

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX * directionX;
        float offsetY = Time.time * scrollSpeedY * directionY;
        Renderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }
}
