using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SimpleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [System.Serializable] public class MyClickEvent : UnityEvent { }

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    protected MyClickEvent m_OnClick = new MyClickEvent();

    [SerializeField ] private Animator Animator;
    private bool Pressed = false;
    private bool MouseCaptured = false;

    private void OnDisable()
    {
        Animator.SetInteger("state", 0);
        Pressed = false;
        MouseCaptured = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;

        Animator.SetInteger("state", 2);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;

        Animator.SetInteger("state", MouseCaptured ? 1 : 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_OnClick.Invoke();

        SoundEffector.PlayUI(0);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseCaptured = true;

        Animator.SetInteger("state", Pressed ? 2 : 1);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseCaptured = false;
        Pressed = false;

        Animator.SetInteger("state", 0);
    }
}
