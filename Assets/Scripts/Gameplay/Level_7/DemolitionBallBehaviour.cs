using System.Collections;
using TMPro;
using UnityEngine;

public class DemolitionBallBehaviour : MonoBehaviour
{
    Rigidbody rigidbody;
    Vector3 oldEulerAngles;
    // public TextMeshProUGUI HintText;
    public AudioSource AudioSource;
    float VelocityThreshold=5.0f;
    bool isRolling = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        AudioSource =GetComponent<AudioSource>();
        // rigidbody.isKinematic=true;
        oldEulerAngles = transform.rotation.eulerAngles;
      
    }

    void FixedUpdate()
    {
        if (rigidbody.angularVelocity.magnitude > VelocityThreshold)
        {
          if (!AudioSource.isPlaying)
            {
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
}

  

   


 

