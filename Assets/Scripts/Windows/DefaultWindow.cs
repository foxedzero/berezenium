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
