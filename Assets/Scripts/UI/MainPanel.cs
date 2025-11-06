using UnityEngine;
using UnityEngine.UIElements;

public class MainPanel : MonoBehaviour
{
    public Color colorTop = Color.white;
    public Color colorBottom = Color.black;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IPanel panel = GetComponent<IPanel>();
       panel.visualTree.style.backgroundColor = new StyleColor(new Color(
           (colorTop.r + colorBottom.r) / 2,
           (colorTop.g + colorBottom.g) / 2,
           (colorTop.b + colorBottom.b) / 2
       ));
    }   

    // Update is called once per frame
    void Update()
    {
        
    }
}
