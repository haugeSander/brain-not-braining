using System.Numerics;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{


    public GameObject Player;
    public float turnSpeed = 2.0f;
   

	
    private float horizontal = 0;
  
	
	void Start () {
	}
	
	void LateUpdate()
	{
		horizontal = Input.GetAxis("Mouse X");  
        transform.position = Player.transform.position;
        transform.Rotate(0, horizontal * turnSpeed, 0);

	}
}
