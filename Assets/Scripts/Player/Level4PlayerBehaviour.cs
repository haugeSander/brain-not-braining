using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    public AudioClip InteractableBoxGrabClip;
    public AudioClip InteractableBoxGrabRelease;
    private Rigidbody2D BlockTaken = null;
    private bool isTakingABlock = false;

    public float GrabDistance = 2.0f;
    bool IsGrounded = true;


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
        Animator.SetBool("Move", axis.x != 0.0f);
        Animator.SetBool("Use", false);
        if (axis.x > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (axis.x < 0.00f)
        {
            transform.localScale = new Vector3(-1, 1, 1);


        }

        if (IsGrounded)
        {
            SoundManager.instance.PlaySound(StepsClip);
        }

    }

    void OnJump()
    {

        if (IsGrounded == true)
        {

            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, JumpForce);
            SoundManager.instance.PlaySound(JumpClip);
        }
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

    void OnCollisionEnter2D(Collision2D theCollision)
    {
        if (theCollision.gameObject.tag == "Ground" ||
            theCollision.gameObject.tag == "Obstacle" ||
            theCollision.gameObject.tag == "InteractableObstacle")
        {
            IsGrounded = true;
        }
    }

    //consider when character is jumping .. it will exit collision.
    void OnCollisionExit2D(Collision2D theCollision)
    {
        if (theCollision.gameObject.tag == "Ground" ||
            theCollision.gameObject.tag == "Obstacle" ||
            theCollision.gameObject.tag == "InteractableObstacle")
        {
            IsGrounded = false;
        }
    }


    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     BlockTaken = collision.GetComponent<Rigidbody2D>();
    // }

    // void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.GetComponent<Rigidbody>() == BlockTaken && !isTakingABlock)
    //         BlockTaken = null;
    // }
    void GrabBlock()
    {
        GameObject InteractableObstacle = FindClosestObjectByTag("InteractableObstacle");

        if (InteractableObstacle != null)
        {
            BlockTaken = InteractableObstacle.gameObject.GetComponent<Rigidbody2D>();
            // 1.Disable physics
            BlockTaken.isKinematic = true;
            Collider2D blockCollider = BlockTaken.GetComponent<Collider2D>();
            blockCollider.enabled = false;

            // 2. Connect the object to the player
            BlockTaken.transform.parent = transform;

            // 3. Pretend that the block is not entering the body of the player
            BlockTaken.transform.localPosition = new Vector3(2.0f, 0, 0);

            isTakingABlock = true;
            SoundManager.instance.PlaySound(InteractableBoxGrabClip);

        }

    }

    GameObject FindClosestObjectByTag(string type)
    {

        // Find all game objects with tag Enemy
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(type);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        Transform target;
        float GrabbingDistance = 7.5f;

        // Iterate through them and find the closest one

        foreach (GameObject go in gos)
        {
            var diff = (go.transform.position - position);
            var curDistance = diff.sqrMagnitude;

            if ((curDistance < distance) && (curDistance <= GrabbingDistance))
            {
                closest = go;
                distance = curDistance;

                target = closest.transform;
            }
        }

        return closest;

    }

    void ReleaseBlock()
    {

        if (BlockTaken != null)
        {
            // 1. Detach the block from the player
            BlockTaken.transform.parent = null;

            // 2. Restart physics
            BlockTaken.isKinematic = false;
            Collider2D blockCollider = BlockTaken.GetComponent<Collider2D>();
            blockCollider.enabled = true;

            // 3. Cleaning the block taken
            BlockTaken = null;
            isTakingABlock = false;
            SoundManager.instance.PlaySound(InteractableBoxGrabRelease);
        }

    }


}
