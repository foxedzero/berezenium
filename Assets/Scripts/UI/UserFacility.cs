using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserFacility : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] protected RectTransform RectTransform;
    [SerializeField] private RectTransform Content;
    [SerializeField] private Text Label;

    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private Text List;
    private Facility[] Facilities = new Facility[0];

    [SerializeField] private Text[] FacilityFilters;
    [SerializeField] private GameObject FacilityNameButton;
    [SerializeField] private GameObject FacilityIndexButton;

    private int FacilityType = -1;
    private int SortFacilityBy = -1;
    private string FacilityName = "";
    private int FacilityIndex = -1;

    public delegate void FacilityReturn(Facility facility);
    private FacilityReturn ToReturn = null;

    private Vector2 StartPosition = Vector2.zero;

    private void OnDestroy()
    {
        City._DataBase.OnFacilityChanges -= UpdateList;
    }

    public void SetInfo(string label, FacilityReturn ask)
    {
        Label.text = label;
        ToReturn = ask;

        City._DataBase.OnFacilityChanges += UpdateList;
        UpdateList();

        IlusionHolders.SetInfo(null, Return);
    }

    public void EditFacilityFilter(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskVariants("", new string[] { "Любое", "Жильё", "Электричество", "Промышленность", "Пища", "Развлечение", "Хранилище", "Образование и наука", "Медицина", "Прочее" }, new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8 }, SetFacilityType);
                break;
            case 1:
                UserInteract.AskVariants("", new string[] { "По номеру", "По названию", "По укомплектованности", "По потреблению/производству электричества" }, new int[] { -1, 0, 1, 2 }, SetFacilitySort);
                break;
        }
    }
    public void SetFacilityName(string info)
    {
        FacilityName = info;

        UpdateList();
    }
    public void SetFacilityIndex(string info)
    {
        if (info.Length <= 0)
        {
            FacilityIndex = -1;
        }
        else
        {
            FacilityIndex = StaticTools.StringToInt(info);
        }

        UpdateList();
    }
    public void SetFacilityType(int index)
    {
        FacilityType = index;

        switch (index)
        {
            case -1:
                FacilityFilters[0].text = "Любое";
                break;
            case 0:
                FacilityFilters[0].text = "Жильё";
                break;
            case 1:
                FacilityFilters[0].text = "Электричество";
                break;
            case 2:
                FacilityFilters[0].text = "Промышленность";
                break;
            case 3:
                FacilityFilters[0].text = "Пища";
                break;
            case 4:
                FacilityFilters[0].text = "Развлечение";
                break;
            case 5:
                FacilityFilters[0].text = "Хранилище";
                break;
            case 6:
                FacilityFilters[0].text = "Образование и наука";
                break;
            case 7:
                FacilityFilters[0].text = "Медицина";
                break;
            case 8:
                FacilityFilters[0].text = "Прочее";
                break;
        }

        UpdateList();
    }
    public void SetFacilitySort(int index)
    {
        SortFacilityBy = index;

        FacilityIndexButton.SetActive(index == -1);
        FacilityNameButton.SetActive(index == 0);

        switch (index)
        {
            case -1:
                FacilityFilters[1].text = "По номеру";
                break;
            case 0:
                FacilityFilters[1].text = "По названию";
                break;
            case 1:
                FacilityFilters[1].text = "По укомплектованности";
                break;
            case 2:
                FacilityFilters[1].text = "По электричеству";
                break;
        }

        UpdateList();
    }

    public void Return(int index)
    {
        if(ToReturn != null)
        {
            ToReturn.Invoke(Facilities[ index]);
        }

        Destroy(gameObject);
    }

    public void UpdateList()
    {
        Facility[] facilities = new Facility[0];

        if (SortFacilityBy == -1 && FacilityIndex != -1 && !(FacilityIndex < 0 || FacilityIndex >= City._DataBase._Facilities.Length))
        {
            facilities = new Facility[] { City._DataBase._Facilities[FacilityIndex] };
        }
        else
        {
            foreach (Facility facility in City._DataBase._Facilities)
            {
                switch (FacilityType)
                {
                    default:
                        facilities = StaticTools.ExpandMassive(facilities, facility);
                        break;
                    case 0:
                        if (facility is Home)
                        {
                            facilities = StaticTools.ExpandMassive(facilities, facility);
                        }
                        break;
                    case 1:
                        if (facility is EnergyProcuder)
                        {
                            facilities = StaticTools.ExpandMassive(facilities, facility);
                        }
                        break;
                    case 3:
                        if (facility is FoodProducer)
                        {
                            facilities = StaticTools.ExpandMassive(facilities, facility);
                        }
                        break;
                }
            }

            for (int i = 0; i < facilities.Length; i++)
            {
                int high = i;
                for (int ii = i + 1; ii < facilities.Length; ii++)
                {
                    switch (SortFacilityBy)
                    {
                        case 0:
                            if (FacilityName.Length <= 0)
                            {
                                if (facilities[ii]._ConstructInfo.Name.CompareTo(facilities[high]._ConstructInfo.Name) < 0)
                                {
                                    high = ii;
                                }
                            }
                            else
                            {
                                if (StaticTools.Match(FacilityName, facilities[ii]._ConstructInfo.Name) > StaticTools.Match(FacilityName, facilities[high]._ConstructInfo.Name))
                                {
                                    high = ii;
                                }
                            }
                            break;
                        case 1:
                            if (facilities[ii]._MaxBearCount - facilities[ii]._AssignedBears.Length > facilities[high]._MaxBearCount - facilities[high]._AssignedBears.Length)
                            {
                                high = ii;
                            }
                            break;
                        case 2:
                            if (((facilities[ii] is EnergyProcuder) ? -(facilities[ii] as EnergyProcuder)._Produce : facilities[ii]._EnergyConsume) < ((facilities[ii] is EnergyProcuder) ? -(facilities[ii] as EnergyProcuder)._Produce : facilities[ii]._EnergyConsume))
                            {
                                high = ii;
                            }
                            break;
                    }
                }

                Facility buffer = facilities[i];
                facilities[i] = facilities[high];
                facilities[high] = buffer;
            }
        }

        Facilities = facilities;

        string info = "";

        for (int i = 0; i < facilities.Length; i++)
        {
            switch (SortFacilityBy)
            {
                default:
                    info += $"{facilities[i]._ConstructInfo.Name}\n";
                    break;
                case -1:
                    info += $"{facilities[i]._ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, facilities[i])}\n";
                    break;
                case 1:
                    info += $"{facilities[i]._ConstructInfo.Name} {facilities[i]._AssignedBears.Length}/{facilities[i]._MaxBearCount}\n";
                    break;
                case 2:
                    info += $"{facilities[i]._ConstructInfo.Name} {(facilities[i] is EnergyProcuder ? $"+{(facilities[i] as EnergyProcuder)._Produce}" : $"-{facilities[i]._EnergyConsume}")}\n";
                    break;
            }
        }

        List.text = info;
        IlusionHolders._MaxIndex = facilities.Length - 1;

        Content.sizeDelta = new Vector2(0, 35 * Facilities.Length);
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
