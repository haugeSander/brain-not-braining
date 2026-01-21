using TMPro;
using UnityEngine;

public class InteractableBehaviour : MonoBehaviour
{
    public TextMeshProUGUI HintText;
    Rigidbody rigidbody;
    public AudioClip HitSound;
    void Start()
    {
        if (HintText != null)
        {
            HintText.enabled = false;
        }
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && HintText != null)
        {
            HintText.text = "Press F to Interact";
            HintText.enabled = true;
        }

        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Interactable")
        {
            rigidbody.isKinematic=true;
        }

        if (collision.gameObject.tag == "DemolitionBall")
        {
            
            AudioHelper.PlaySFX(HitSound, transform.position, 1f);
        }
        
            
    }

    void OnCollisionStay(Collision collision)
    {
         if (collision.gameObject.tag == "Player" && HintText != null)
        {
            HintText.text = "Press F to Interact";
            HintText.enabled = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && HintText != null)
        {
            
            HintText.enabled = false;
        }
    }
}