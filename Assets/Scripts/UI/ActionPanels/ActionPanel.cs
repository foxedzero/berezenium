using UnityEngine;

public class ActionPanel : MonoBehaviour, ICancelable
{
    protected NeedCursorOrder NeedCursorOrder = new NeedCursorOrder();

    protected virtual void Start()
    {
        CursorManager.SetNeedMouse(NeedCursorOrder, false);
        CancelQueue.Register(this, false);
    }

    protected virtual void OnDestroy()
    {
        CursorManager.SetNeedMouse(NeedCursorOrder, true);
        CancelQueue.Register(this, true);
    }

    public virtual void Cancel()
    {
        Destroy(gameObject);
    }
}
