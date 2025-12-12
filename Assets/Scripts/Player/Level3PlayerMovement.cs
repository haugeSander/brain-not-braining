using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level3PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody Rigidbody;
    public float thrust = 1;
    public AudioSource movementAudio;

    UnityEngine.Vector2 axis;
    public float JumpForce = 0.4f;
    private Coroutine fadeRoutine = null;
    float fadeOutDuration = 0.3f;

    private Camera MainCamera; 

    private Vector2 MouseScreenPosition;

    public GameObject ShockWavePrefab; 
    public Transform LaunchPointShockWave; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        MainCamera = Camera.main;
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

        bool isMoving = axis.sqrMagnitude > 0;

        if (isMoving)
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
                fadeRoutine = null;
            }
            
            movementAudio.volume = 1f; 

            if (!movementAudio.isPlaying)
            {
                movementAudio.Play();
            }
        }
        else
        {
            if (movementAudio.isPlaying && fadeRoutine == null)
            {
                fadeRoutine = StartCoroutine(FadeOut(movementAudio, fadeOutDuration));
            }
        }

        if(movementAudio.isPlaying)
            WaveLaunch();
     
    }

    public void OnLook(InputValue input)
    {
        MouseScreenPosition = input.Get<Vector2>();
        
    }

    IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float startTime = Time.time;
        
        while (Time.time < startTime + duration)
        {
            float elapsed = Time.time - startTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
            yield return null; 
        }

        audioSource.volume = 0;
        audioSource.Stop(); 
        audioSource.volume = startVolume; 
        
        fadeRoutine = null;
    }

    public void WaveLaunch()
    {
     

        Ray ray = MainCamera.ScreenPointToRay(MouseScreenPosition);
        RaycastHit hit;
        
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f)) 
        {
            targetPoint = hit.point;
            Debug.Log("Hit " + targetPoint);
        }
        else
        {
            targetPoint = ray.GetPoint(100f); 
        }
        //debug 
        Debug.DrawLine(ray.origin, targetPoint, Color.red, 5.0f);
        Debug.DrawLine(LaunchPointShockWave.position, targetPoint, Color.green, 6.0f);


        Vector3 launchDirection = (targetPoint - LaunchPointShockWave.position).normalized;

        Quaternion launchRotation = Quaternion.LookRotation(launchDirection);
        GameObject NewShockWave = Instantiate(ShockWavePrefab, LaunchPointShockWave.position, launchRotation);
        
        if (NewShockWave.TryGetComponent<ShockWave>(out var ShockWaveScript))
        {
            ShockWaveScript.Initialize(launchDirection); 
        }
        
    }


   


}
