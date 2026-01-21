using UnityEngine;

public class CameraIntroTransition : MonoBehaviour
{
    public Transform firstPersonTarget;
    public float delayBeforeMove = 3f;
    public float transitionDuration = 1.5f;

    private Vector3 startPos;
    private Quaternion startRot;
    private float timer;
    private bool transitioning = false;

    void Start()
    {
        
    }

    public void GameStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        Invoke(nameof(StartTransition), delayBeforeMove);
    }

    void StartTransition()
    {
        transitioning = true;
        timer = 0f;
    }

    void Update()
    {
        if (!transitioning) return;

        timer += Time.deltaTime;
        float t = timer / transitionDuration;

        transform.position = Vector3.Lerp(startPos, firstPersonTarget.position, t);
        transform.rotation = Quaternion.Slerp(startRot, firstPersonTarget.rotation, t);

        if (t >= 1f)
        {
            transitioning = false;
            transform.SetParent(firstPersonTarget);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
