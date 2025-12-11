using Unity.VisualScripting;
using UnityEngine;

public class BackgroundBehaviour : MonoBehaviour
{
    public GameObject MainArea;
    private void Start()
    {
        
    }

    private void Update()
    {
        

        transform.RotateAround(
            
               MainArea.transform.position,Vector3.up,2.5f * Time.deltaTime);
        // transform.rotation= new Quaternion(0,3,0,0);
    }
}