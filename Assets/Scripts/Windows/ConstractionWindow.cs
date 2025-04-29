using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;
using static Constructor;

public class ConstractionWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private GameObject BuildSlotPrefab;
    [SerializeField] private RectTransform Content;

    private GameObject[] Slots = new GameObject[0];

    private Constructor Constructor = null;

    public override string _Label => "Панель строительства";

    private void Start()
    {
        Constructor = FindObjectOfType<Constructor>();

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
            if(City._Research.GetResearchLevel(info.Research) >= info.ResearchLevel)
            {
                constructions = StaticTools.ExpandMassive(constructions, info);
            }
        }

        Slots = new GameObject[constructions.Length];
        for(int i = 0; i < constructions.Length; i++)
        {
            BuildSlot slot = Instantiate(BuildSlotPrefab, Content).GetComponent<BuildSlot>();
            slot.SetInfo(this, constructions[i]);
            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(64.5f + i % 3 * 140, -64.5f - (i / 3) * 140);

            Slots[i] = slot.gameObject;
        }

        Content.sizeDelta = new Vector2(0, 140 + (constructions.Length / 3) * 140);
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
