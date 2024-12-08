using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreePlayerInteract : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private CursorIconInfo Cursor;
    private IInteractable Pointed;

    private void Update()
    {
        if (CheckClick())
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask))
            {
                IInteractable interactable = hit.transform.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    Pointed = interactable;
                    CursorManager.EditIcon(Cursor, false);
                }
                else
                {
                    Pointed = null;
                    CursorManager.EditIcon(Cursor, true);
                }

                if (Pointed != null && InputManager.GetButtonDown(InputManager.ButtonEnum.Interact))
                {
                    Pointed.Interact();
                }
            }
            else
            {
                if (Pointed != null)
                {
                    Pointed = null;
                    CursorManager.EditIcon(Cursor, true);
                }
            }
        }
        else
        {
            if (Pointed != null)
            {
                Pointed = null;
                CursorManager.EditIcon(Cursor, true);
            }
        }
    }

    private bool CheckClick()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>(0);
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count == 0)
        {
            return true;
        }

        return false;
    }
}
