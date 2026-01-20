using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isCorrectTile;

    private Renderer rend;
    private Collider col;

   

    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color previewColor = Color.yellow;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        rend.material.color = defaultColor;
    }

    public void SetPreview(bool state)
    {
        rend.material.color = state ? previewColor : defaultColor;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (isCorrectTile)
        {
            rend.material.color = correctColor;
        }
        else
        {
            // Wrong tile → disappear
            col.enabled = false;
            rend.enabled = false;
        }
    }

    void Update()
    {
         
    }

    
}
