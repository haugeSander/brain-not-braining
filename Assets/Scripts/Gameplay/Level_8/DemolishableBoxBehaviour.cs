using TMPro;
using UnityEngine;

public class DemolishableBoxBehaviour : MonoBehaviour
{
    public TextMeshProUGUI HintText;
    Rigidbody rigidbody;
    public AudioClip HitSound;
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
   


        if (collision.gameObject.tag == "DemolitionBall")
        {
            
            SoundManager.instance.PlaySound(HitSound);
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