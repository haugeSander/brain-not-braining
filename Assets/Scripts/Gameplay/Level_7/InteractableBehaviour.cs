using TMPro;
using UnityEngine;

public class InteractableBehaviour : MonoBehaviour
{
    public TextMeshProUGUI HintText;
    void Start()
    {
        HintText.enabled = false;
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
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