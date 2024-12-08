using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserBear : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] protected RectTransform RectTransform;

    [SerializeField] private RectTransform Content;
    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private Text List;
    private Bear[] Bears = new Bear[0];

    [SerializeField] private Text Age0Text, Age1Text;
    [SerializeField] private Text KastaText;
    [SerializeField] private Text SortText;
    [SerializeField] private Text Label;
    [SerializeField] private GameObject NameField;
    [SerializeField] private GameObject IndexField;

    private int[] Age = new int[] { 0, 100};
    private int Kasta = -1;
    private int Sort = -1;
    private string BearName = "";
    private int BearIndex = -1;

    public delegate void BearReturn(Bear bear);
    private BearReturn ToReturn = null;

    private Vector2 StartPosition = Vector2.zero;

    private void OnDestroy()
    {
        City._DataBase.OnBearChanges -= UpdateInfo;
    }

    public void SetInfo(string label, BearReturn toReturn)
    {
        Label.text = label;
        ToReturn = toReturn;

        City._DataBase.OnBearChanges += UpdateInfo;
        UpdateInfo();

        IlusionHolders.SetInfo(null, Return);
    }

    public void EditBearFilter(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskInput("", SetBearAge0);
                break;
            case 1:
                UserInteract.AskInput("", SetBearAge1);
                break;
            case 2:
                UserInteract.AskVariants("", new string[] { "Любая", "Неопределёно", "Пасечник", "Конструктор", "Программист", "Биоинженер", "Первопроходец", "Творец" }, new int[] { -1, 0, 1, 2, 3, 4, 5, 6 }, SetBearKasta);
                break;
            case 3:
                UserInteract.AskVariants("", new string[] { "По номеру", "По имени", "По навыку", "Не имеет работы", "Не имеет жилья", "По здоровью", "По удовлетворенности", "По возрасту" }, new int[] { -1, 0, 1, 2, 3, 4, 5, 6 }, SetBearSorting);
                break;
            case 4:
                UserInteract.AskInput("", SetBearName);
                break;
        }
    }
    public void SetBearName(string info)
    {
        BearName = info;

        UpdateInfo();
    }
    public void SetBearIndex(string info)
    {
        if (info.Length <= 0)
        {
            BearIndex = -1;
        }
        else
        {
            BearIndex = StaticTools.StringToInt(info);
        }

        UpdateInfo();
    }
    public void SetBearAge0(string info)
    {
        Age[0] = Mathf.Min(Mathf.Clamp(StaticTools.StringToInt(info), 0, 100), Age[1]);
        Age0Text.text = $"от {Age[0]}";

        UpdateInfo();
    }
    public void SetBearAge1(string info)
    {
        Age[1] = Mathf.Max(Mathf.Clamp(StaticTools.StringToInt(info), 0, 100), Age[0]);
        Age1Text.text = $"до {Age[1]}";
        UpdateInfo();
    }
    public void SetBearKasta(int index)
    {
        Kasta = index;
        if (Kasta == -1)
        {
            KastaText.text = "Любая";
        }
        else
        {
            KastaText.text = ((Bear.Kasta)index).ToString();
        }

        UpdateInfo();
    }
    public void SetBearSorting(int index)
    {
        NameField.SetActive(index == 0);
        IndexField.SetActive(index == -1);

        Sort = index;
        switch (index)
        {
            case -1:
                SortText.text = "По номеру";
                break;
            case 0:
                SortText.text = "По имени";
                break;
            case 1:
                SortText.text = "По навыку";
                break;
            case 2:
                SortText.text = "Не имеет работы";
                break;
            case 3:
                SortText.text = "Не имеет жилья";
                break;
            case 4:
                SortText.text = "По здоровью";
                break;
            case 5:
                SortText.text = "По удовлетворенности";
                break;
            case 6:
                SortText.text = "По возрасту";
                break;
        }

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        Bear[] bears = new Bear[0];

        if (Sort == -1 && BearIndex != -1 && !(BearIndex < 0 || BearIndex >= City._DataBase._Bears.Length))
        {
            bears = new Bear[] { City._DataBase._Bears[BearIndex] };
        }
        else
        {
            foreach (Bear bear in City._DataBase._Bears)
            {
                if (bear._Age >= Age[0] && bear._Age <= Age[1])
                {
                    if (Kasta == -1 || bear._Kasta.GetHashCode() == Kasta)
                    {
                        bears = StaticTools.ExpandMassive(bears, bear);
                    }
                }
            }

            int[] bearSatisfactions = new int[0];
            int[] bearNamePriorities = new int[0];
            if (Sort == 5)
            {
                bearSatisfactions = new int[bears.Length];
                for (int i = 0; i < bears.Length; i++)
                {
                    bearSatisfactions[i] = bears[i]._Satisfaction;
                }
            }
            else if (Sort == 0 && BearName.Length > 0)
            {
                bearNamePriorities = new int[bears.Length];
                for (int i = 0; i < bears.Length; i++)
                {
                    bearNamePriorities[i] = StaticTools.Match(BearName, bears[i]._Name);
                }
            }

            for (int i = 0; i < bears.Length; i++)
            {
                int high = i;

                for (int ii = i + 1; ii < bears.Length; ii++)
                {
                    switch (Sort)
                    {
                        case 0:
                            if (BearName.Length <= 0)
                            {
                                if (bears[ii]._Name.CompareTo(bears[high]._Name) < 0)
                                {
                                    high = ii;
                                }
                            }
                            else
                            {
                                if (bearNamePriorities[ii] > bearNamePriorities[high])
                                {
                                    high = ii;
                                }
                            }
                            break;
                        case 1:
                            if (bears[ii]._Skill > bears[high]._Skill)
                            {
                                high = ii;
                            }
                            break;
                        case 2:
                            if ((bears[ii]._Facility == null).GetHashCode() > (bears[high]._Facility == null).GetHashCode())
                            {
                                high = ii;
                            }
                            break;
                        case 3:
                            if ((bears[ii]._Home == null).GetHashCode() > (bears[high]._Home == null).GetHashCode())
                            {
                                high = ii;
                            }
                            break;
                        case 4:
                            if (bears[ii]._Health < bears[high]._Health)
                            {
                                high = ii;
                            }
                            break;
                        case 5:
                            if (bearSatisfactions[ii] < bearSatisfactions[high])
                            {
                                high = ii;
                            }
                            break;
                        case 6:
                            if (bears[ii]._Age < bears[high]._Age)
                            {
                                high = ii;
                            }
                            break;
                    }
                }

                Bear buffer = bears[i];
                bears[i] = bears[high];
                bears[high] = buffer;

                if (Sort == 5)
                {
                    int sbuffer = bearSatisfactions[i];
                    bearSatisfactions[i] = bearSatisfactions[high];
                    bearSatisfactions[high] = sbuffer;
                }
                else if (Sort == 0 && BearName.Length > 0)
                {
                    int sbuffer = bearNamePriorities[i];
                    bearNamePriorities[i] = bearNamePriorities[high];
                    bearNamePriorities[high] = sbuffer;
                }
            }
        }

        Bears = bears;

        string info = "";

        for (int i = 0; i < bears.Length; i++)
        {
            switch (Sort)
            {
                case -1:
                    info += $"{bears[i]._Name}  #{StaticTools.IndexOf(City._DataBase._Bears, bears[i])}\n";
                    break;
                case 1:
                    info += $"{bears[i]._Name}  {bears[i]._Skill}\n";
                    break;
                case 2:
                    info += $"{bears[i]._Name}  {(bears[i]._Facility != null ? bears[i]._Facility._ConstructInfo.Name + $"#{StaticTools.IndexOf(City._DataBase._Facilities, bears[i]._Facility)}" : "нет")}\n";
                    break;
                case 3:
                    info += $"{bears[i]._Name}  {(bears[i]._Home != null ? bears[i]._Home._ConstructInfo.Name + $"#{StaticTools.IndexOf(City._DataBase._Facilities, bears[i]._Home)}" : "нет")}\n";
                    break;
                case 4:
                    info += $"{bears[i]._Name}  {bears[i]._Health}\n";
                    break;
                case 5:
                    info += $"{bears[i]._Name}  {bears[i]._Satisfaction}\n";
                    break;
                case 6:
                    info += $"{bears[i]._Name}  {bears[i]._Age}\n";
                    break;
                default:
                    info += $"{bears[i]._Name}\n";
                    break;
            }
        }

        List.text = info;
        IlusionHolders._MaxIndex = bears.Length - 1;

        Content.sizeDelta = new Vector2(0, 35 * Bears.Length);
    }

    public void Return(int index)
    {
        if(ToReturn != null)
        {
            ToReturn.Invoke(Bears[index]);
        }

        Destroy(gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    protected Vector2 CalculatePosition()
    {
        Vector2 viewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        return new Vector2(1920 * viewPort.x, StaticTools.ScreenHeight * viewPort.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartPosition = RectTransform.anchoredPosition;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform.anchoredPosition = StartPosition + eventData.position - eventData.pressPosition;
    }
}
