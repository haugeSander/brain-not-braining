using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelEightBehaviour : MonoBehaviour
{

    BoxCollider PlayerBoxCollider;
    public TextMeshProUGUI HintText;





    bool HasTouchedADoor = false;

    bool HasTouchedAnInteractable = false;
    bool HasTouchedDemolitionBall = false;
    private bool isTakingABlock = false;

    private Rigidbody BlockTaken = null;

    public AudioClip BoxPickupClip;
    public AudioClip BoxReleaseClip;
    public AudioClip Footsteps;
    public AudioClip FastFootSteps;
    AudioSource AudioSource;


    float speed = 0;

    Vector3 lastPosition = Vector3.zero;
    public void Start()
    {
        PlayerBoxCollider = GetComponent<BoxCollider>();

        AudioSource = GetComponent<AudioSource>();

    }

    public void Update()
    {

    }
    void FixedUpdate()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
        speed *= 100;
        if (speed > 1.0f && speed < 8.0f)
        {
            if (!AudioSource.isPlaying)
            {
                AudioSource.clip = Footsteps;
                AudioSource.Play();
            }
        }
        else if (speed >= 8.0f)
        {

        // Debug.Log($"Speed:{speed}");
            if (AudioSource.isPlaying && AudioSource.clip!=FastFootSteps)
            {
                AudioSource.Stop();

            }
            
            if (!AudioSource.isPlaying)
            {
                AudioSource.clip = FastFootSteps;
                AudioSource.Play();
            }
            

        }
        else
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }
        }

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
        if (DemolitionBall != null)
        {
            Rigidbody rigidbody = DemolitionBall.GetComponent<Rigidbody>();
            if (rigidbody != null && !rigidbody.isKinematic)
            {
                Vector3 pushDir = new Vector3(1, 0, 0);
                rigidbody.linearVelocity = pushDir * 5.0f; // Applica una spinta
            }
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
            BlockTaken.transform.localPosition = new Vector3(0.0f, 0.5f, 1.5f);

            isTakingABlock = true;
            // SoundManager.instance.PlaySound(InteractableBoxGrabClip);
            BlockTaken.centerOfMass = Vector3.zero;
            SoundManager.instance.PlaySound(BoxPickupClip);
            HintText.text = "Press F to release";
            HintText.enabled = true;
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
        float GrabbingDistance = 2.0f;

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
        // Debug.Log("Collision Detected " + collision.gameObject);
        // if (collision.gameObject.tag == "Door")
        // {
        //     HasTouchedADoor = true;
        // }

        // if (collision.gameObject.tag == "Interactable")
        // {
        //     HasTouchedAnInteractable = true;
        // }

        // if (collision.gameObject.tag == "DemolitionBall")
        // {
        //     HasTouchedDemolitionBall = true;
        // }


    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Detected " + other.gameObject);
        if (other.gameObject.tag == "Interactable" && !isTakingABlock)
        {
            HintText.text = "Press F to Interact";
            HintText.enabled = true;
        }
        else if (other.gameObject.tag == "DemolitionBall")
        {
            HintText.text = "Press F to Push";
            HasTouchedDemolitionBall = true;
            HintText.enabled = true;
        }
        else
        {

        }
    }

    void OnTriggerStay(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {
        if (!isTakingABlock)
        {

            HintText.enabled = false;
            HasTouchedDemolitionBall = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // if (collision.gameObject.tag == "Door")
        // {
        //     HasTouchedADoor = false;
        // }

        // if (collision.gameObject.tag == "Interactable")
        // {
        //     HasTouchedAnInteractable = false;
        // }
        // if (collision.gameObject.tag == "DemolitionBall")
        // {
        //     HasTouchedDemolitionBall = false;
        // }
    }
}