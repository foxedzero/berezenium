using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Variant : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private ChooseVariant Choose;
    [SerializeField] private Text Name;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private int Index;
    [SerializeField] private int HierarchyIndex;

    public float _Width => Mathf.Min(Name.preferredWidth + 55, 600);
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
        Arrow.SetActive(state);

        if (state)
        {
            Name.color = new Color(1, 1, 1, 0.8f);
            Name.rectTransform.anchoredPosition = new Vector2(27.5f, 0);
        }
        else
        {
            Name.color = new Color(1, 1, 1, 0.25f);
            Name.rectTransform.anchoredPosition = new Vector2(17.5f, 0);
        }
    }
}
