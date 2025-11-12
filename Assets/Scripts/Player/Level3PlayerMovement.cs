using UnityEngine;
using UnityEngine.InputSystem;

public class Level3PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  private Rigidbody Rigidbody;
    public float thrust = 1;

    UnityEngine.Vector2 axis;
    public float JumpForce = 0.4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Rigidbody.AddForce(
            UnityEngine.Vector3.Normalize(new UnityEngine.Vector3(axis.x, 0.0f, axis.y))
            * thrust
            );

    }

    void OnMove(InputValue input)
    {
        axis = input.Get<UnityEngine.Vector2>();
    }
     
     void OnJump()
    {
        //ball on the ground
        if(Rigidbody.linearVelocity.y == 0)
        {
            Rigidbody.AddForce(UnityEngine.Vector3.up * JumpForce,ForceMode.Impulse);
        }
    }
}
