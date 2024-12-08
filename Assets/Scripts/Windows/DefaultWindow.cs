using WindowInterfaces;
using UnityEngine;

public abstract class DefaultWindow : Window, IMoveable
{
    [SerializeField] protected WindowDrag Dragger;
    [SerializeField] protected bool Pinned = false;

    public WindowDrag _Dragger => Dragger;
    public Window _Window => this;
    public override string _Label => $"пустое окно ({Index})";

    public virtual void Move(Vector2 newPosition)
    {
        if (Pinned)
        {
            return;
        }

        if(newPosition.x + RectTransform.sizeDelta.x / 2 + 12.5f > 960)
        {
            newPosition.x = 960 - RectTransform.sizeDelta.x / 2 - 12.5f;
        }
        else if(newPosition.x - RectTransform.sizeDelta.x / 2 - 12.5f < -960)
        {
            newPosition.x = -960 + RectTransform.sizeDelta.x / 2 + 12.5f;
        }

        float screenHeight = StaticTools.ScreenHeight;
        if (newPosition.y + RectTransform.sizeDelta.y / 2 + 12.5f > screenHeight / 2)
        {
            newPosition.y = screenHeight / 2 - RectTransform.sizeDelta.y / 2 - 12.5f;
        }
        else if (newPosition.y - RectTransform.sizeDelta.y / 2 - 12.5f < -screenHeight / 2)
        {
            newPosition.y = -screenHeight / 2 + RectTransform.sizeDelta.y / 2 + 12.5f;
        }

        RectTransform.anchoredPosition = newPosition;
    }

    public override void RightMouse()
    {
        UserInteract.AskVariants(_Label, new string[] { $"{(Pinned ? "открепить" : "закрепить")} окно", "закрыть окно" }, new int[] { -1, 0 }, RightMouseActions);
    }

    public override void RightMouseActions(int index)
    {
        switch (index)
        {
            case -1:
                Pinned = !Pinned;
                break;
            case 0:
                Close();
                break;
        }
    }
}
