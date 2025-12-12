using UnityEngine;
using UnityEngine.UI;

public class ChecKPointBehaviour : MonoBehaviour
{
    public RawImage HintDirection;
    public float rotationAngle;

    
    void Start()
    {
        HintDirection.enabled=false;
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HintDirection.transform.Rotate(0.0f,0.0f,rotationAngle);
            HintDirection.enabled=true;

            Debug.Log("Player Entered CheckPoint");
        }      
    }

    void OnTriggerExit(Collider other)
    {
        HintDirection.transform.Rotate(0.0f,0.0f,-rotationAngle);
        HintDirection.enabled = false;        
    }
}