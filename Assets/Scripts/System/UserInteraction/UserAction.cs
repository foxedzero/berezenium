using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public abstract class UserAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected RectTransform RectTransform;
    [SerializeField] protected Text Label;
    protected bool MouseCaptured = false;

    public SimpleVoid OnSkip = null;

    private NeedCursorOrder NeedCursor = new NeedCursorOrder();

    protected virtual void OnDestroy()
    {
        CursorManager.SetNeedMouse(NeedCursor, true);
    }

    protected virtual void Start() 
    {
        CursorManager.SetNeedMouse(NeedCursor, false);
    }

    protected Vector2 CalculatePosition()
    {
        Vector2 viewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        return new Vector2(1920 * viewPort.x + 25, StaticTools.ScreenHeight * viewPort.y-25);
    }

    protected virtual void Update()
    {
        if (!MouseCaptured && Input.anyKeyDown )
        {
            foreach (string mouse in new string[] { "mouse 0", "mouse 1", "mouse 2", "mouse 3", "mouse 4", "mouse 5", "mouse 6" })
            {
                if (Input.GetKeyDown(mouse))
                {
                    if (OnSkip != null)
                    {
                        OnSkip.Invoke();
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseCaptured = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseCaptured = false;
    }
}
