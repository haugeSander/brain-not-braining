using TMPro;
using UnityEngine;

public class InteractableBehaviour : MonoBehaviour
{
    public TextMeshProUGUI HintText;
    Rigidbody rigidbody;
    void Start()
    {
        HintText.enabled = false;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
       

        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Interactable")
        {
            rigidbody.isKinematic=true;
        }

    
        
            
    }

    void OnCollisionStay(Collision collision)
    {
         if (collision.gameObject.tag == "Player")
        {
            HintText.text = "Press F to Interact";
            HintText.enabled = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            
            HintText.enabled = false;
        }
    }
}