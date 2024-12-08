using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private GameObject UseIndicator;
    private IInteractable Pointed = null;

    private void Update()
    {
        if (CursorManager._UILocked)
        {
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 3, LayerMask))
        {
            IInteractable interactable = hit.transform.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                Pointed = interactable;
                UseIndicator.SetActive(true);
            }
            else
            {
                Pointed = null;
                UseIndicator.SetActive(false);
            }

            if (Pointed != null && InputManager.GetButtonDown(InputManager.ButtonEnum.Interact))
            {
                Pointed.Interact();
            }
        }
        else
        {
            if(Pointed != null)
            {
                Pointed = null;
                UseIndicator.SetActive(false);
            }
        }
    }
}
