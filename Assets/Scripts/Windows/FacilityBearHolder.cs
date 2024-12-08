using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FacilityBearHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text Info;
    [SerializeField] private Image Background;
    private Bear Bear = null;
    private FacilityWindow Window = null;

    public void SetInfo(Bear bear, FacilityWindow window)
    {
        Bear = bear;
        Window = window;
        Info.text = Bear._Name;
    }

    public void RightMouseAction(int index)
    {
        switch (index)
        {
            case 0:
                FindObjectOfType<WindowCreator>().CreateWindow<BearWindow>().SetInfo(Bear);
                break;
            case 1:
                Window._Facility.AssignBear(Bear, true);
                break;
            case 2:
                UserInteract.AskBear("Назначить вместо него", Resign);
                break;
        }
    }

    public void Resign(Bear bear)
    {
        Facility facility = Window._Facility;

        facility.AssignBear(Bear, true);
        facility.AssignBear(bear, facility);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundEffector.PlayUI(0);
        UserInteract.AskVariants(Bear._Name, new string[] {"Выбрать", "Снять его с назначения", "Назначить вместо него"}, new int[] {0, 1, 2}, RightMouseAction);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Background.color = Design._UseColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Background.color = Design._NoUseColor;
    }
}
