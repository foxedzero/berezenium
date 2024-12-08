
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;

public class Pokazateli : MonoBehaviour
{
    [SerializeField] private CityStorage CityStorage;
    [SerializeField] private CityDataBase CityDataBase;
    [SerializeField] private CityEnergosystem CityEnergosystem;
    [SerializeField] private CityFoodstream CityFoodstream;
    [SerializeField] private CityTime CityTime;
    [SerializeField] private TimeEditor TimeEditor;

    [SerializeField] private Tipper StorageTip;
    [SerializeField] private Tipper BearsTip;
    [SerializeField] private Tipper FoodTip;
    [SerializeField] private Tipper ElectricityTip;
    [SerializeField] private Tipper TimeTip;

    [SerializeField] private Text EnergyHoney;
    [SerializeField] private Text BearsCount;
    [SerializeField] private Text FoodCount;
    [SerializeField] private Text Electricity;
    [SerializeField] private Text WeatherInfo;
    [SerializeField] private Text TimeInfo;
    [SerializeField] private Text DateInfo;

    private bool UpdateEnergyInfo = false;
    private bool UpdateFoodInfo = false;
    private bool UpdateSatisfactionInfo = false;

    private string StringDateInfo = "";

    private void Start()
    {
        CityStorage.OnChanges += UpdateStorage;
        CityDataBase.OnBearChanges += UpdateDataBase;
        CityDataBase.OnFacilityChanges += UpdateDataBase;
        CityTime.DayChanged += DayPassed;

        UpdateDataBase();
        UpdateStorage();
        DayPassed();
    }

    public void UpdateStorage()
    {
        EnergyHoney.text = $"{CityStorage._EnergyHoney}";
        StorageTip._Info = $"В данный момент вы имеете:\n{CityStorage._EnergyHoney} мёда\n{CityStorage._Berezenium} березениума\n{City._Storage._Metal} металла\n{City._Storage._Wood} древесины\n{City._Storage._Robots} роботов";
    }

    public void DayPassed()
    {
        FoodCount.text = $"{(CityFoodstream._Produced - CityFoodstream._Consumed > 0 ? "+" : "")}{Mathf.RoundToInt(CityFoodstream._Produced - CityFoodstream._Consumed)}";
        FoodTip._Info = $"В хранилище: {CityFoodstream._StoredFood}/{CityFoodstream._FoodCapacity}\nПроизвели: {CityFoodstream._Produced}\nИзрасходовали: {CityFoodstream._Consumed}\nСытость медведей: {(int)(CityFoodstream._Saturation * 100)}%";

        Electricity.text = $"{(CityEnergosystem._Produced - CityEnergosystem._Consumed > 0 ? "+" : "")}{Mathf.RoundToInt(CityEnergosystem._Produced - CityEnergosystem._Consumed)}";
        ElectricityTip._Info = $"Аккумулировано: {CityEnergosystem._StoredEnergy}/{CityEnergosystem._EnergyCapacity}\nПроизвели: {CityEnergosystem._Produced}\nИзрасходовали: {CityEnergosystem._Consumed}\nЭнергосистема: {(int)(CityEnergosystem._Effectivity * 100)}%";

        StringDateInfo = $"{CityTime.GetDate()}  {CityTime.GetYear()} года";
        DateInfo.text = CityTime.GetIntDate();

        WeatherInfo.text = $"{-City._Weather._Cold}";
    }

    public void UpdateDataBase()
    {
        int capacity = 0;
        foreach(Facility facility in CityDataBase._Facilities)
        {
            if(facility is Home)
            {
                capacity += (facility as Home)._MaxBearCount;
            }
        }

        BearsCount.text = $"{CityDataBase._Bears.Length}/{capacity}";

        string info = $"Медведей в городе: {CityDataBase._Bears.Length}/{capacity}\nЗданий в городе: {CityDataBase._Facilities.Length}\nСредняя удовлетворённость: {(int)(City._Factors._CitySatisfaction * 100)}%\n\nОбщие факторы:";
        foreach(CityFactors.Factor factor in City._Factors._SatisfactionFactors)
        {
            info += $"\n{factor}";
        }

        BearsTip._Info = info;
    }

    private void LateUpdate()
    {
        int time = (int)(25 * 60 *  (CityTime._Time - (int)CityTime._Time));

        int h = time / 60;
        int m = time % 60;

        TimeInfo.text = $"{(h < 10 ? "0" : "")}{h}:{(m < 10 ? "0" : "")}{m}";

        TimeTip._Info = $"Представление времени в родной планете - Берлоге.\nСейчас {TimeInfo.text}  {StringDateInfo}\n\nМультипликатор скорости времени: {Mathf.Pow(2, TimeEditor._TimeIndex)}X\n\nДней вашего управления: {(int)City._Time._Time}";

        if (UpdateEnergyInfo)
        {
            float[] potencial = CityEnergosystem._Potencial;
            ElectricityTip._Info = $"Аккумулировано: {CityEnergosystem._StoredEnergy}/{CityEnergosystem._EnergyCapacity}\nПотенциальное производство: {potencial[0]}\nПотенциальный расход: {potencial[3]}\nРасход: {potencial[1]}\nПотенциальная энергосистема: {(int)(potencial[2] * potencial[2] * 100)}%\nЭнергосистема: {(int)(City._Energosystem._Effectivity * 100)}%";
        }
        if (UpdateFoodInfo)
        {
            float[] potencial = CityFoodstream._Potencial;
            FoodTip._Info = $"В хранилище: {CityFoodstream._StoredFood}/{CityFoodstream._FoodCapacity}\nПотенциальное производство: {potencial[0]}\nПотенциальный расход: {potencial[3]}\nРасход: {potencial[1]}\nПотенциальная сытость: {(int)(potencial[2] * 100)}%\nСытость: {(int)(City._Foodstream._Saturation * 100)}%";
        }
        if (UpdateSatisfactionInfo)
        {
            int capacity = 0;
            foreach (Facility facility in CityDataBase._Facilities)
            {
                if (facility is Home)
                {
                    capacity += (facility as Home)._MaxBearCount;
                }
            }

            float potencial = City._Factors.GetPotencialSatisfaction();
            string info = $"Медведей в городе: {CityDataBase._Bears.Length}/{capacity}\nЗданий в городе: {CityDataBase._Facilities.Length}\nПотенциальная удовлетворённость: {(int)(City._Factors.GetPotencialSatisfaction() * 100)}%\nСредняя удовлетворённость: {(int)(City._Factors._CitySatisfaction * 100)}%\n\nОбщие факторы:";
            foreach (CityFactors.Factor factor in City._Factors._SatisfactionFactors)
            {
                info += $"\n{factor}";
            }

            BearsTip._Info = info;
        }
    }

    public void ShowEnergyPotencial(bool state)
    {
        UpdateEnergyInfo = state;
    }
    public void ShowFoodPotencial(bool state)
    {
        UpdateFoodInfo = state;
    }
    public void ShowSatisfactionPotencial(bool state)
    {
        UpdateSatisfactionInfo = state;
    }
}
