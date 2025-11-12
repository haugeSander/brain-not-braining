using System.Numerics;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    //distance from player
    UnityEngine.Vector3 Offset;

    UnityEngine.Vector3 NewplayerPosition;
    
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Offset = Player.transform.position - transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position - Offset;
    }
}
