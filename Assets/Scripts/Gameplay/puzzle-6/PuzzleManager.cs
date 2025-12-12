using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public ColorChangingHole[] holes;
    private int currentIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ActivateHole(0);
    }

    public void HoleCompleted(ColorChangingHole hole)
    {
        if (holes[currentIndex] != hole) return;

        currentIndex++;

        if (currentIndex < holes.Length)
        {
            ActivateHole(currentIndex);
        }
        else
        {
            Debug.Log("Puzzle completed!");
        }
    }

    private void ActivateHole(int index)
    {
        for (int i = 0; i < holes.Length; i++)
        {
            holes[i].gameObject.SetActive(i == index);
        }
    }
}

