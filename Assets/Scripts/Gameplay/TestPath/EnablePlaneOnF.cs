using UnityEngine;

public class EnablePlaneOnF : MonoBehaviour
{
    // Drag your Plane GameObject here in the Inspector
    public GameObject plane;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (plane != null)
            {
                plane.SetActive(true);
            }
        }
    }
}

