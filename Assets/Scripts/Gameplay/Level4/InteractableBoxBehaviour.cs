using TMPro;
using UnityEngine;

public class InteractableBoxBehaviour : MonoBehaviour
{
   
    public TextMeshProUGUI text;
     Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        text.text="Press F to Interact";
        text.enabled=false;
    }

    private void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
             Debug.Log("Collision detected with Player");
            text.enabled=true;
            // SoundManager.instance.PlaySound(HitClip);
        
        }
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Interactable")
        {
            rigidbody.isKinematic=true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        text.enabled = false;
    }
}