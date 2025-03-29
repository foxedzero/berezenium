using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCheckUI : MonoBehaviour
{
    public static bool OnUI = false;

    private void Update()
    {
        OnUI = MouseOnUI();
    }

    static public bool MouseOnUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>(0);
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count == 0)
        {
            return false;
        }

        return true;
    }

}
