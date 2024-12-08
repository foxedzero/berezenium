using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;
using static Constructor;

public class ConstractionWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private GameObject BuildSlotPrefab;
    [SerializeField] private RectTransform Content;
    [SerializeField] private Text CategoryInfo;

    private static ConstructCategory Category = ConstructCategory.Жилище;
    private GameObject[] Slots = new GameObject[0];

    private Constructor Constructor = null;

    public override string _Label => "Панель строительства";

    private void Start()
    {
        Constructor = FindObjectOfType<Constructor>();

        UpdateList();

        CategoryInfo.text = Category.ToString();
    }

    public void SetCategory()
    {
        UserInteract.AskVariants("", new string[] { "Электроэнергия", "Жилище", "Производство", "Добыча", "Пища", "Медицина", "Прочее" }, new int[] { 0, 1, 2, 3, 4, 5, 6  }, SetCategory);
    }

    public void SetCategory(int index)
    {
        Category = (ConstructCategory)index;

        CategoryInfo.text = Category.ToString();

        UpdateList();
    }

    public void UpdateList()
    {
        foreach(GameObject slot in Slots)
        {
            Destroy(slot);
        }

        ConstructInfo[] constructions = new ConstructInfo[0];
        foreach(ConstructInfo info in Constructor._Constructions)
        {
            if(info.Category == Category && City._Research.GetResearchLevel(info.Research) >= info.ResearchLevel)
            {
                constructions = StaticTools.ExpandMassive(constructions, info);
            }
        }

        float x = 62.5f;

        Slots = new GameObject[constructions.Length];
        for(int i = 0; i < constructions.Length; i++)
        {
            BuildSlot slot = Instantiate(BuildSlotPrefab, Content).GetComponent<BuildSlot>();
            slot.SetInfo(this, constructions[i]);
            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);

            Slots[i] = slot.gameObject;

            x += 150;
        }

        Content.sizeDelta = new Vector2(x, 0);
    }

    public void Construct(ConstructInfo info)
    {
        if(Constructor._Constructing == info)
        {
            Constructor._Constructing = null;
        }
        else
        {
            Constructor._Constructing = info;
        }
    }
}
