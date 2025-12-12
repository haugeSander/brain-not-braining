using System.Collections;
using TMPro;
using UnityEngine;

public class DoorBeahviour : MonoBehaviour
{

    public AudioClip DoorOpeningClip;
    public TextMeshProUGUI HintText;

    public GameObject Fulcrum;

    public bool isPlayerTouchingDoor=false;
    private void Start()
    {
        HintText.enabled = false;
    }

    void FixedUpdate()
    {
        if (isPlayerTouchingDoor && Input.GetKeyDown(KeyCode.F))
        {
             Debug.Log("Collision with player");
                StartCoroutine(DoorOpeningCoroutine());
        }
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            HintText.enabled = true;
            isPlayerTouchingDoor= true;
            
            
        }
    }
    


    IEnumerator DoorOpeningCoroutine()
    {
         SoundManager.instance.PlaySound(DoorOpeningClip);
            
            transform.RotateAround(
            
            Fulcrum.transform.position,Vector3.up,200f * Time.deltaTime);
        yield return new WaitForSeconds(5);

    }



    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            HintText.enabled = false;
            isPlayerTouchingDoor=false;
        }
    }
    }
