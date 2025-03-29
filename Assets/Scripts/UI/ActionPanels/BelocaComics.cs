using UnityEngine;
using UnityEngine.UI;

public class BelocaComics : ActionPanel
{
    [SerializeField] private Image Image;
    [SerializeField] private Sprite[] Pages;
    [SerializeField] private int CurrentPage;

    public void Move(bool back)
    {
        if (back)
        {
            CurrentPage = Mathf.Max(CurrentPage - 1, 0);
        }
        else
        {
            CurrentPage = Mathf.Min(CurrentPage + 1, Pages.Length - 1);
        }

        Image.sprite = Pages[CurrentPage];
    }
}
