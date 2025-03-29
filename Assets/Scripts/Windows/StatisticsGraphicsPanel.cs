using UnityEngine;

public class StatisticsGraphicsPanel : MonoBehaviour
{
    [SerializeField] private GraphicBar[] Electricity;
    [SerializeField] private GraphicBar[] Honey;
    [SerializeField] private GraphicBar[] EnergyHoney;
    [SerializeField] private GraphicBar[] Wood;
    [SerializeField] private GraphicBar[] Metal;
    [SerializeField] private GraphicBar[] Berezenium;

    private void OnEnable()
    {
        City._Time.HourPassed += UpdateInfo;
        UpdateInfo();
    }

    private void OnDisable()
    {
        City._Time.HourPassed -= UpdateInfo;
    }

    public void UpdateInfo()
    {
        SetGraphic(Electricity, CityStorage.ResourceType.Electricity);
        SetGraphic(Honey, CityStorage.ResourceType.Honey);
        SetGraphic(EnergyHoney, CityStorage.ResourceType.EnergyHoney);
        SetGraphic(Wood, CityStorage.ResourceType.Wood);
        SetGraphic(Metal, CityStorage.ResourceType.Metal);
        SetGraphic(Berezenium, CityStorage.ResourceType.Berezenium);
    }

    private void SetGraphic(GraphicBar[] bars, CityStorage.ResourceType resource)
    {
        int controlHour = City._CityStatistics._ControlHour;
        int hour = (int)(City._Time._WorldTime / 60);

        float[] values = new float[25];
        string[] tips = new string[25];
        foreach(CityStatistics.Statistic statistic in City._CityStatistics._Statistic)
        {
            if(statistic.Resource == resource)
            {
                int index = 24 - hour + statistic.Hour;
                if(index >= 0)
                {
                    values[index] += statistic.Value;
                    tips[index] += $"\n{statistic.Cause}: {statistic.Value}";
                }
            }
        }

        values[24 - hour + controlHour] += City._CityStatistics.GetControl(resource);
        for(int i = 25 -hour + controlHour; i < 25; i++)
        {
            values[i] += values[i - 1];
        }
        for (int i = 23 - hour + controlHour; i > -1; i--)
        {
            values[i] = values[i + 1] - values[i];
        }

        float max = Mathf.Max(values);
        float maximal = 0;
        while(maximal <= max)
        {
            maximal += 25;
        }


        for (int i = 0; i < bars.Length; i++)
        {
            bars[i]._Tipper._Info = $"{values[i]}\n" + tips[i];
            bars[i]._RectTransform.sizeDelta = new Vector2(33.52f, values[i] / maximal * 100);
            bars[i]._Hours.text = $"{ hour - controlHour + i - 25}";
        }
    }
}
