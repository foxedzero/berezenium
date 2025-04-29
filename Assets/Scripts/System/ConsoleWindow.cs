using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsoleWindow : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private RectTransform Content;
    [SerializeField] private InputField InputField;
    private NeedCursorOrder NeedCursorOrder = new NeedCursorOrder();

    private Vector2 FirstPosition = Vector2.zero;

    private void Start()
    {
        CursorManager.SetNeedMouse(NeedCursorOrder, false);

        Console._Instance.OnLogUpdate += UpdateInfo;
        UpdateInfo();
    }

    private void OnDestroy()
    {
        CursorManager.SetNeedMouse(NeedCursorOrder, true);

        Console._Instance.OnLogUpdate -= UpdateInfo;
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    public void UpdateInfo()
    {
        InputField.text = Console._Log;

        Content.sizeDelta = new Vector2(0, InputField.preferredHeight + 100);
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        FirstPosition = RectTransform.anchoredPosition;

        OnDrag(pointerEventData);
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        Vector2 newPosition = FirstPosition + pointerEventData.position - pointerEventData.pressPosition;
        if (newPosition.x + RectTransform.sizeDelta.x / 2 > 960)
        {
            newPosition.x = 960 - RectTransform.sizeDelta.x / 2;
        }
        else if (newPosition.x - RectTransform.sizeDelta.x / 2  < -960)
        {
            newPosition.x = -960 + RectTransform.sizeDelta.x / 2 ;
        }

        float screenHeight = StaticTools.ScreenHeight;
        if (newPosition.y + RectTransform.sizeDelta.y / 2 > screenHeight / 2)
        {
            newPosition.y = screenHeight / 2 - RectTransform.sizeDelta.y / 2 ;
        }
        else if (newPosition.y - RectTransform.sizeDelta.y / 2 < -screenHeight / 2)
        {
            newPosition.y = -screenHeight / 2 + RectTransform.sizeDelta.y / 2;
        }

        RectTransform.anchoredPosition = newPosition;
    }
}
