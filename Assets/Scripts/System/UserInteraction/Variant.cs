using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class Variant : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private ChooseVariant Choose;
    [SerializeField] private Image Light;
    [SerializeField] private Text Name;
    [SerializeField] private int Index;
    [SerializeField] private int HierarchyIndex;

    public float _Width => Name.preferredWidth + 45;
    public int _Index => Index;

    public void SetInfo(ChooseVariant choose, string name, int index, int hierarchyIndex, float yPosition)
    {
        Choose = choose;
        Name.text = name;
        Index = index;
        HierarchyIndex = hierarchyIndex;

        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPosition);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Choose.SetCurrent(HierarchyIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Choose.Select(Index);
            SoundEffector.PlayUI(0);
        }
    }

    public void HighLight(bool state)
    {
        if (state)
        {
            Light.color = Design._UseColor;

            Name.rectTransform.anchoredPosition = new Vector2(8.5f, 0);
        }
        else
        {
            Light.color = Design._MainColor;

            Name.rectTransform.anchoredPosition = new Vector2(17.5f, 0);
        }
    }
}
