using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelSevenBehaviour : MonoBehaviour
{

    BoxCollider PlayerBoxCollider;





    bool HasTouchedADoor = false;

    bool HasTouchedAnInteractable = false;
    bool HasTouchedDemolitionBall = false;
    private bool isTakingABlock = false;

    private Rigidbody BlockTaken = null;

    public AudioClip BoxPickupClip;
    public AudioClip BoxReleaseClip;
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

        if (HasTouchedDemolitionBall)
        {
            PushDemolitionBall();
        }
    }

    void PushDemolitionBall()
    {
        GameObject DemolitionBall = FindClosestObjectByTag("DemolitionBall");
        Rigidbody rigidbody = DemolitionBall.GetComponent<Rigidbody>();
        if (rigidbody != null && !rigidbody.isKinematic)
        {
            Vector3 pushDir = new Vector3(1, 0, 0);
            rigidbody.linearVelocity = pushDir * 5.0f; // Applica una spinta
        }
    }

    // private void OnControllerColliderHit(ControllerColliderHit hit)
    // {
    //     // Verifica se l'oggetto colpito è la Sfera (puoi usare un Tag o il nome)
    //     if (hit.gameObject.CompareTag("DemolitionBall")) // Assicurati di taggare la Sfera
    //     {
    //         // Debug.Log("Player: La Sfera è stata colpita!");

    //         // Se la Sfera deve essere spinta, puoi applicare una forza qui:
    //         Rigidbody body = hit.collider.attachedRigidbody;

    //         // Esempio per spingere un oggetto (richiede Rigidbody sulla Sfera):
    //         // if (body != null && !body.isKinematic)
    //         // {
    //         //     Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
    //         //     body.linearVelocity = pushDir * 5.0f; // Applica una spinta
    //         // }
    //     }
    // }

    void GrabBlock()
    {
        GameObject InteractableObstacle = FindClosestObjectByTag("Interactable");

        if (InteractableObstacle != null)
        {
            BlockTaken = InteractableObstacle.gameObject.GetComponent<Rigidbody>();
            // 1.Disable physics
            BlockTaken.transform.rotation = Quaternion.identity;
            Collider blockCollider = BlockTaken.GetComponent<Collider>();
            blockCollider.enabled = false;
            BlockTaken.isKinematic = true;

            // 2. Connect the object to the player
            BlockTaken.transform.parent = transform;

            // 3. Pretend that the block is not entering the body of the player
            BlockTaken.transform.localPosition = new Vector3(0.0f, 1.0f, 1.5f);

            isTakingABlock = true;
            // SoundManager.instance.PlaySound(InteractableBoxGrabClip);
            BlockTaken.centerOfMass = Vector3.zero;
            SoundManager.instance.PlaySound(BoxPickupClip);
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
            // 1. Detach and Reset Velocities
            BlockTaken.transform.parent = null;
            BlockTaken.linearVelocity = Vector3.zero;
            BlockTaken.angularVelocity = Vector3.zero;
            BlockTaken.transform.rotation = Quaternion.identity;

            // 2. Reactivate Collider  (Best Practice)
            Collider blockCollider = BlockTaken.GetComponent<Collider>();
            blockCollider.enabled = true;

            // 3. Set Constraints 
            BlockTaken.constraints = RigidbodyConstraints.FreezeRotation;

            // 4. Reactivate Physics 
            BlockTaken.isKinematic = false;

            BlockTaken = null;
            isTakingABlock = false;
             SoundManager.instance.PlaySound(BoxReleaseClip);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected " + collision.gameObject);
        if (collision.gameObject.tag == "Door")
        {
            HasTouchedADoor = true;
        }

        if (collision.gameObject.tag == "Interactable")
        {
            HasTouchedAnInteractable = true;
        }

        if (collision.gameObject.tag == "DemolitionBall")
        {
            HasTouchedDemolitionBall = true;
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
        if (collision.gameObject.tag == "Door")
        {
            HasTouchedADoor = false;
        }

        if (collision.gameObject.tag == "Interactable")
        {
            HasTouchedAnInteractable = false;
        }
        if (collision.gameObject.tag == "DemolitionBall")
        {
            HasTouchedDemolitionBall = false;
        }
    }
}