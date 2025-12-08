using UnityEngine;

public class PressurePlate3DBehaviour : MonoBehaviour
{
    public AudioClip PressureClip;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collsion detected");
        if (collision.gameObject.tag=="Player")
        {
            SoundManager.instance.PlaySound(PressureClip);
        }
    }
}