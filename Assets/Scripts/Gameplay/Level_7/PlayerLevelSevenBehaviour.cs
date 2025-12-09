using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelSevenBehaviour : MonoBehaviour
{

    BoxCollider PlayerBoxCollider;
   
    



    bool HasTouchedADoor=false;

    bool HasTouchedAnInteractable=false;
    private bool isTakingABlock = false;

    private Rigidbody BlockTaken= null;
    public void Start()
    {
        PlayerBoxCollider = GetComponent<BoxCollider>();
        
       
        
    }

    public void Update()
    {
        
    }

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

    void GrabBlock()
    {
        GameObject InteractableObstacle = FindClosestObjectByTag("Interactable");

        if (InteractableObstacle != null)
        {
            BlockTaken = InteractableObstacle.gameObject.GetComponent<Rigidbody>();
            // 1.Disable physics
            BlockTaken.isKinematic = true;
            Collider blockCollider = BlockTaken.GetComponent<Collider>();
            blockCollider.enabled = false;

            // 2. Connect the object to the player
            BlockTaken.transform.parent = transform;

            // 3. Pretend that the block is not entering the body of the player
            BlockTaken.transform.localPosition = new Vector3(0.0f, 1.0f, 1.5f);

            isTakingABlock = true;
            // SoundManager.instance.PlaySound(InteractableBoxGrabClip);

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
            Collider blockCollider = BlockTaken.GetComponent<Collider>();
            blockCollider.enabled = true;

            // 3. Cleaning the block taken
            BlockTaken = null;
            isTakingABlock = false;
            // SoundManager.instance.PlaySound(InteractableBoxGrabRelease);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected " + collision.gameObject );
        if(collision.gameObject.tag == "Door")
        {
            HasTouchedADoor = true;
        }

        if (collision.gameObject.tag == "Interactable")
        {
            HasTouchedAnInteractable=true;
        }

       
    }

    // void OnTriggerEnter(Collider other)
    // {
    //      if (other.gameObject.tag == "Checkpoint")
    //     {
    //         HintText.text="Turn Left";
    //         HintText.enabled= true;
            
    //     }        
    // }

    void OnCollisionExit(Collision collision)
    {
         if(collision.gameObject.tag == "Door")
        {
            HasTouchedADoor = false;
        }

        if (collision.gameObject.tag == "Interactable")
        {
            HasTouchedAnInteractable=false;
        }
    }
}