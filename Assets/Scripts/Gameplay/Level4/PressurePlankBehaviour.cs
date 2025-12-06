using UnityEngine;

public class PressurePlank : MonoBehaviour
{
    public AudioClip HitClip;
    public AudioClip UsingClip;

    public Vector3 InitialPosition;

    private Rigidbody2D rigidbody2D;

    private bool MoveBack=false;

    private void Start()
    {
        InitialPosition = transform.position;
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (MoveBack)
        {
            if(transform.position.y < InitialPosition.y)
            {

                transform.Translate(0,0.01f,0);
            }
            else
            {
                MoveBack=false;
            }
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Player")
        {
            
            // collision.transform.parent = transform;
            SoundManager.instance.PlaySound(HitClip);  
            MoveBack=false;
        }  
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
   
                
            transform.Translate(0,-0.01f,0);
            
        } 
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        MoveBack=true;
        // collision.transform.parent = null;
    }
}