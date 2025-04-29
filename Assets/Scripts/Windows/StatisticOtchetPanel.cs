using UnityEngine;
using UnityEngine.UI;
using static CityStatistics;

public class StatisticOtchetPanel : MonoBehaviour
{
    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private Tipper Tipper;

    [SerializeField] private Text Was;
    [SerializeField] private Text Produced;
    [SerializeField] private Text Consumed;
    [SerializeField] private Text Left;

    private string[] Tips = new string[14];

    private void OnEnable()
    {
        City._Time.DayChanged += UpdateInfo;
        UpdateInfo();
    }

    private void OnDisable()
    {
        City._Time.DayChanged -= UpdateInfo;
        Tipper.OnPointerExit(null);
    }

    private void Start()
    {
        IlusionHolders.SetInfo(Informate, null);
    }

    public void UpdateInfo()
    {
        CityStatistics city = City._CityStatistics;
        string was = $"<color=#AAAAAA>Запас</color>";
        string produced = "<color=#AAAAAA>Произв.</color>";
        string consumed = "<color=#AAAAAA>Израсх.</color>";

        string left = $"<color=#AAAAAA>Остаток</color>";

        Tips[0] = $"Отчёт на {City._CityStatistics._ControlHour / 25} день\n\nБыло\nПроизведено\nИзраходовано\nОсталось";

        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Electricity, 1);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Honey, 2);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.EnergyHoney, 3);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Wood, 4);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Metal, 5);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Berezenium, 6);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Robots, 7);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Robots, 8);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Snowrunners, 9);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Astressin, 10);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Ratonik, 11);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Steamulator, 12);
        WasProdCons(ref was, ref produced, ref consumed, ref left, CityStorage.ResourceType.Antisleep, 13);

        Was.text = was;
        Produced.text = produced;
        Consumed.text = consumed;
        Left.text = left;
    }

    private void WasProdCons(ref string was, ref string produced, ref string consumed, ref string left, CityStorage.ResourceType resource, int tipIndex)
    {
        float[] values = new float[4];
        int controlHour = City._CityStatistics._ControlHour;

        values[3] = City._CityStatistics.GetControl(resource);
        values[0] = values[3];

        string tip = $"{values[3]}\n^ ^ ^ ^ ^ ^ ^ ^";

        int[] exceptionCause = new int[0];
        
        foreach (Statistic statistic in City._CityStatistics._Statistic)
        {
            if(statistic.Resource == resource && statistic.Hour < controlHour && statistic.Hour >= controlHour - 25)
            {
                int cause = statistic.Cause.GetHashCode();
                if(!StaticTools.Contains(exceptionCause, cause))
                {
                    float value = 0;
                    foreach (Statistic statisticInCause in City._CityStatistics._Statistic)
                    {
                        if(statisticInCause.Resource == resource && statisticInCause.Hour <= controlHour && statistic.Hour >= controlHour - 25 && statisticInCause.Cause.GetHashCode() == cause)
                        {
                            value += statisticInCause.Value;
                        }
                    }

                    tip += $"\n{statistic.Cause}: {(value > 0 ? "+" : "")}{value}";
                    values[0] -= value;
                    if (value > 0)
                    {
                        values[1] += value;
                    }
                    else if (value < 0)
                    {
                        values[2] += -value;
                    }

                    exceptionCause = StaticTools.ExpandMassive(exceptionCause, cause);
                }
            }
        }
        tip += $"\n ^ ^ ^ ^ ^ ^ ^ ^\n<color=yellow>{values[0]}</color>";

        was += $"\n{values[0]}";
        produced += $"\n{values[1]}";
        consumed += $"\n{values[2]}";
        left += $"\n{values[3]}";

        Tips[tipIndex] = tip;
    }

    public void Informate(int index)
    {
        Tipper.OnPointerExit(null);
        Tipper._Info = Tips[index];
        Tipper.OnPointerEnter(null);
    }
}
