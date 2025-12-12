using UnityEngine;

public class BackgroundBehaviour : MonoBehaviour
{
    private float StartPosition;
    public GameObject Camera;

    public float ParallaxEffect;

    private float Distance;
    private float Length;
    private float Movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPosition = transform.position.x;
        Length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Calculating distance with camera movement
        Distance = Camera.transform.position.x * ParallaxEffect; 
        // Movement = Camera.transform.position.x * (1 - ParallaxEffect);

        transform.position= new Vector3(StartPosition + Distance,transform.position.y, transform.position.z);

        // if(Movement > StartPosition + Length)
        // {
        //     StartPosition+= Length;
        // }
        // else
        // {
        //     StartPosition -= Length;
        // }
    }
}
