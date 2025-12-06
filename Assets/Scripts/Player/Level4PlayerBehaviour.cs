using UnityEngine;
using UnityEngine.InputSystem;

public class Level4PlayerBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D Rigidbody;
    private BoxCollider2D BoxCollider;
    public LayerMask GroundLayer;

    private Animator Animator;
    public float thrust = 1;

    UnityEngine.Vector2 axis;
    public float JumpForce = 20.4f;

    public AudioClip JumpClip;
    public AudioClip StepsClip;
    private Rigidbody2D BlockTaken = null;
    private bool isTakingABlock=false;

    public float GrabDistance = 2.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        BoxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Rigidbody.AddForce(
            UnityEngine.Vector3.Normalize(new UnityEngine.Vector2(axis.x, axis.y * JumpForce))
            * thrust
            );

    }

    void OnMove(InputValue input)
    {
        // Debug.Log("OnMoce");
        axis = input.Get<UnityEngine.Vector2>();
        Animator.SetBool("Move",axis.x != 0.0f);
        Animator.SetBool("Use",false);
        if (axis.x > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (axis.x < 0.00f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (isGrounded())
        {
            SoundManager.instance.PlaySound(StepsClip);
        }
       
    }

    void OnJump()
    {
        if (isGrounded())
        {
            
            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, JumpForce);
            SoundManager.instance.PlaySound(JumpClip);
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center,BoxCollider.bounds.size,0,Vector2.down,0.1f,GroundLayer);
        return raycastHit.collider != null;
    }

    // Funzione per cercare un blocco vicino
    void OnUse()
    {
        if (!isTakingABlock)
        {
            
           GrabBlock();
        }
        else
        {
            ReleaseBlock();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
       BlockTaken = collision.GetComponent<Rigidbody2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Rigidbody>() == BlockTaken && !isTakingABlock)
            BlockTaken= null;
    }
    void GrabBlock()
    {
        // 1. Disabilita la fisica del blocco per non farlo reagire alle collisioni
        BlockTaken.isKinematic = true; 
        
        // 2. Collega il blocco al Giocatore: il blocco seguirà il Giocatore ovunque vada
        BlockTaken.transform.parent = transform; 
        
        // 3. (Opzionale) Sposta leggermente il blocco davanti al giocatore per non farlo "entrare" nel corpo
        BlockTaken.transform.localPosition = new Vector3(0, 0, 1.5f); 
        
        isTakingABlock = true;
    }

    void ReleaseBlock()
    {
        // 1. Scollega il blocco dal Giocatore
        BlockTaken.transform.parent = null; 
        
        // 2. Riabilita la fisica
        BlockTaken.isKinematic = false;
        
        // 3. Pulisci il riferimento
        BlockTaken = null;
        isTakingABlock = false;
    }


}
