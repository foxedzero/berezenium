
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SettingParameter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator Animator;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Header("Параметр")]
    public BoolEvent Event;

    private void OnDisable()
    {
        Animator.SetBool("over", false);
    }

    public void Click(bool right)
    {
        Event?.Invoke(right);
        SoundEffector.PlayUI(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animator.SetBool("over", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animator.SetBool("over", false);
    }
}
