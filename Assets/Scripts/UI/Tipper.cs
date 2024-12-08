using UnityEngine;
using UnityEngine.EventSystems;

public class Tipper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField][Multiline] private string Info;

    public event SimpleVoid OnInfoChange = null;

    public string _Info
    {
        get
        {
            return Info;
        }
        set
        {
            Info = value;

            if (OnInfoChange != null)
            {
                OnInfoChange.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        TipManager.ShowTip(this, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TipManager.ShowTip(this, false);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TipManager.ShowTip(this, true);
    }
}
