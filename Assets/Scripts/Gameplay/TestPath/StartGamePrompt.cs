using UnityEngine;

public class StartGamePrompt : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CameraIntroTransition cameraIntroTransition;
    [SerializeField] private KeyCode startKey = KeyCode.Z;

    private bool started = false;

    void Start()
    {
        startText.SetActive(true);
    }



    void Update()
    {
        if (!started && Input.anyKeyDown)
        {
            started = true;
            //Debug.Log("Game Starts");
            startText.SetActive(false);
            gridManager.StartGame();
            cameraIntroTransition.GameStart();
        }
    }
}

