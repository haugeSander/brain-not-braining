using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public AudioClip JumpSound;
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
            if (AudioSource.isPlaying && AudioSource.clip != FastFootSteps)
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

    void OnJump()
    {
        AudioSource.PlayOneShot(JumpSound);
    }

    void OnUse(InputValue value)

    {
        if(!value.isPressed)return;

        Debug.Log($"OnUse chiamata su {gameObject.name} al frame {Time.frameCount} - HashCode Script: {this.GetHashCode()}");
        if (!isTakingABlock)
        {
            Debug.Log("object grabbed");
            GrabBlock();

        }
        else
        {
            Debug.Log("object released");
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
            BlockTaken.centerOfMass = Vector3.zero;
            AudioHelper.PlaySFX(BoxPickupClip, transform.position, 1f);

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
            // BlockTaken.linearVelocity = Vector3.zero;
            // BlockTaken.angularVelocity = Vector3.zero;
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
            AudioHelper.PlaySFX(BoxReleaseClip, transform.position, 1f);
        }

    }



    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Collision Detected " + other.gameObject);
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


}