using Unity.VisualScripting;
using UnityEngine;

public class MinimapBehaviour : MonoBehaviour
{
    public Transform PlayerTransform;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        Vector3 newPosition = PlayerTransform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation =  Quaternion.Euler(90.0f, PlayerTransform.eulerAngles.y,0);
    }
}