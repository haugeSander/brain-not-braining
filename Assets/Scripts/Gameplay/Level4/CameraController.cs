using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform PlayerTransform;
    public Vector3 offset;

    public AudioClip MainTheme;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.instance.PlaySound(MainTheme);
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(PlayerTransform.position.x +offset.x, PlayerTransform.position.y+offset.y,offset.z);
    }
}
