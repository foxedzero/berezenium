using System.Collections.Generic;
using UnityEngine;
using static CitySally;

public class CityFoodstream : MonoBehaviour
{
    [SerializeField] private CityTime CityTime;
    [SerializeField] private CityDataBase CityData;

    [SerializeField] private float Food;
    [SerializeField] private float SaturateCoefficient;

    public event SimpleVoid OnChanges = null;

    public string _SaveInfo
    {
        get
        {
            return $"Food({Food})Saturation({SaturateCoefficient})";
        }
        set
        {
            if(value == null || value.Length <= 0)
            {
                return;
            }

            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            Food = StaticTools.StringToFloat(parameters["Food"]);
            SaturateCoefficient = StaticTools.StringToFloat(parameters["Saturation"]);
        }
    }
    public float _Saturation => SaturateCoefficient;
    public float _StoredFood
    {
        get
        {
            return Food;
        }
        set
        {
            Food = Mathf.Max(value, 0);
            OnChanges?.Invoke();
        }
    }

    public float CurrentProduce()
    {
        float value = 0;
        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility is FoodProducer)
            {
                value += (facility as FoodProducer)._BaseProduce * facility._Effectivity ;
            }
        }

        return value;
    }
    public float CurrentConsume()
    {
        float consume = 0;
        foreach(Bear bear in City._DataBase._Bears)
        {
            consume += bear._Sally == null ? GlobalVariables._BearFoodCost : 0;
        }

        return consume;
    }

    public void HourPassed()
    {
        int hour = ((int)City._Time._WorldTime % 1500) / 60;
        if(hour == 8 || hour == 16 || hour == 24)
        {
            if(Food <= 0)
            {
                SaturateCoefficient = 0;
                return;
            }

            float consume = CurrentConsume();

            if (Food < consume)
            {
                SaturateCoefficient = Mathf.Min((Food / consume) * 1, 1);
               
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"приём пищи", (int)(City._Time._WorldTime / 60), -Food, CityStorage.ResourceType.Honey));
                Food = 0;
            }
            else
            {
                SaturateCoefficient = 1;
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"приём пищи", (int)(City._Time._WorldTime / 60), -consume, CityStorage.ResourceType.Honey));
                Food -= consume;
            }
        }
    }
}
