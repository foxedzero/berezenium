using UnityEngine;
using UnityEngine.UI;

public class Colorist : MonoBehaviour
{
    [SerializeField] private Graphic[] Main;
    [SerializeField] private Graphic[] Interactable;
    [SerializeField] private Graphic[] OnUse;
    [SerializeField] private Graphic[] Informationable;

    [SerializeField] private Graphic[] White;

    private void Start()
    {
        foreach (Graphic g in Main)
        {
            g.color = Design._MainColor;
        }
        foreach (Graphic g in Interactable)
        {
            g.color = Design._NoUseColor;
        }
        foreach (Graphic g in OnUse)
        {
            g.color = Design._UseColor;
        }
        foreach (Graphic g in Informationable)
        {
            g.color = Design._InformationalColor;
        }
        foreach (Graphic g in White)
        {
            g.color = new Color(1, 1, 1, g.color.a);
        }
    }
}
