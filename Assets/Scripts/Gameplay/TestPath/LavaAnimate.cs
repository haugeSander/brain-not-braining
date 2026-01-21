using UnityEngine;

public class LavaAnimate : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed, Time.time * scrollSpeed);
        rend.material.mainTextureOffset = offset;
    }
}
