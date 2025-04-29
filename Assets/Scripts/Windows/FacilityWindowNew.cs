using UnityEngine;
using UnityEngine.UI;

public class FacilityWindowNew : DefaultWindow
{
    [SerializeField] private GameObject BearSlotPrefab;
    [SerializeField] private Image Icon;
    [SerializeField] private Text Describtion;
    [SerializeField] private RectTransform Content;
    [SerializeField] private RectTransform AddBearButton;
    [SerializeField] private RectTransform AddRobotsButton;
    [SerializeField] private Text[] Parameters;
    [SerializeField] private Tipper[] ParameterTips;
    [SerializeField] private GameObject[] Buttons;
    private GameObject[] Slots = new GameObject[0];
    private Facility Facility = null;

    public void Button(int index)
    {

    }

    public void AddRobot(bool remove)
    {

    }

    public void AddBear()
    {

    }

    public void SetInfo(Facility facility)
    {
        Facility = facility;

        if(facility is Home) //Вместимость, Хладостойкость, Электрозатратность, Отдых, Антистресс
        {

        }
        else if(facility is EnergyProcuder) //Специализация, Вместимость, Коэф усталости, Хладостойкость, Работа, Производство, Лимит потребления, Ресурс  
        {

        }
        else if (facility is Accumulator) //Ёмкость  
        {

        }
        else if (facility is FoodProducer) //Специализация, Вместимость, Коэф усталости, Хладостойкость, Электрозатратность, Производство
        {
           
        }
        else if (facility is ConstructionProject) //Специализация, Вместимость, Коэф усталости, Хладостойкость, Электрозатратность, Работа, Объём работы
        {
     
        }
        else if (facility is ResourceAssimilator)
        {
           
        }
        else if (facility is Assimilator) //Специализация, Использовать роботов, Вместимость, Коэф усталости, Хладостойкость, Электрозатратность, Добыча, Тип ресурса
        {
         
        }
        else if (facility is Factory)
        {
         
        }
        else if (facility is Laboratory)
        {
         
        }
        else if (facility is MedFacility)
        {
          
        }
        else if (facility is Finders)
        {
          
        }
    }

    public void UpdateInfo()
    {
        
    }
}
