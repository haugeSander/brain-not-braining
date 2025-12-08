using UnityEngine;

public class PressurePlank : MonoBehaviour
{
    public AudioClip HitClip;
    public AudioClip UsingClip;

    public Vector3 InitialPosition;

    public SlidingDoorBehaviour slidingDoor;

    private void Start()
    {
        InitialPosition = transform.position;
        
    }

    private void Update()
    {

    

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Player")
        {

            // collision.transform.parent = transform;
            SoundManager.instance.PlaySound(HitClip);
           
            slidingDoor.SlideDoor();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {


            transform.Translate(0, -0.5f, 0);
            Collider2D pressurePlateCollider = GetComponent<Collider2D>();
            pressurePlateCollider.enabled = false;


        }
    }


}