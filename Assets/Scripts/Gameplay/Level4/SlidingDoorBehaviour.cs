using UnityEngine;

public class SlidingDoorBehaviour:MonoBehaviour
{
    public Vector3 InitialPosition;
    public AudioClip SlideDoorClip;

    private void Start()
    {
        InitialPosition = transform.position;
    }

    private void Update()
    {
        
    }

    public void SlideDoor()
    {
        SoundManager.instance.PlaySound(SlideDoorClip);
        transform.Translate(0.0f,5f,0.0f);
    }
}