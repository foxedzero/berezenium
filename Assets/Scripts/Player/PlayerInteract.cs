using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    public void Interact();
    public void Indicate(bool state);
    public bool _AbstractUse { get; }
    public string _Info { get; }
}

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Animator Indicator;
    [SerializeField] private Text[] InteractInfo; 

    [SerializeField] private LayerMask LayerMask;
    private IInteractable Pointed = null;

    private void OnDisable()
    {
        if (Pointed != null)
        {
            Pointed.Indicate(false);
            Indicator.SetBool("Show", false);
            Pointed = null;
        }
    }

    private void Update()
    {
        if (CursorManager._UILocked)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.2f, transform.forward, out hit, 3, LayerMask))
        {
            IInteractable interactable = hit.transform.GetComponentInParent<IInteractable>();
            if (interactable != null && !(interactable is Facility))
            {
                if (Pointed != null  && Pointed != interactable)
                {
                    Pointed.Indicate(false);
                }

                Pointed = interactable;
                Pointed.Indicate(true);
                InteractInfo[0].text = $"{InputManager._Instance._Interact._Keys[0]}";
                InteractInfo[1].text = $"{Pointed._Info}";
                Indicator.SetBool("Show", true);
            }
            else if(Pointed != null)
            {
                Pointed.Indicate(false);
                Indicator.SetBool("Show", false);
                Pointed = null;
            }

            if (Pointed != null  && InputManager.GetButtonDown(InputManager.ButtonEnum.Interact))
            {
                Pointed.Interact();
            }
        }
        else
        {
            if(Pointed != null)
            {
                Pointed.Indicate(false);
                Indicator.SetBool("Show", false);
                Pointed = null;
            }
        }
    }
}
