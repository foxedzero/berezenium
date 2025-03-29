using UnityEngine;
using UnityEngine.UI;

public class CityNotation : MonoBehaviour
{
    [SerializeField] private RectTransform HomeIsue;
    [SerializeField] private RectTransform WorkIsue;
    [SerializeField] private RectTransform ResearchIsue;
    [SerializeField] private RectTransform FoodIsue;
    [SerializeField] private RectTransform EnergosystemIsue;
    [SerializeField] private RectTransform ColdIsue;
    [SerializeField] private RectTransform HealthIsue;
    [SerializeField] private RectTransform SleepIsue;
    [SerializeField] private RectTransform StressIsue;
    [SerializeField] private RectTransform SallyIsue;
    [SerializeField] private RectTransform StressTestIsue;

    [SerializeField] private Tipper HomeTip;
    [SerializeField] private Tipper WorkTip;
    [SerializeField] private Tipper ResearchTip;
    [SerializeField] private Tipper FoodTip;
    [SerializeField] private Tipper EnergosystemTip;
    [SerializeField] private Tipper ColdTip;
    [SerializeField] private Tipper HealthTip;
    [SerializeField] private Tipper SleepTip;
    [SerializeField] private Tipper StressTip;
    [SerializeField] private Tipper SallyTip;
    [SerializeField] private Tipper StressTestTip;

    private void Start()
    {
        HourPassed();
    }

    public void HourPassed()
    {
        bool homeIsue = false;
        string homeInfo = "Иметь жильё медведю первая необходимость, так как в доме он отдыхает и проводит свой досуг.\n\nСледующие медведи не заселены:";

        bool workIsue = false;
        string workInfo = "В этом суровом месте нельзя отлынивать от работы, если нет подходящей работы, то не бойтесь отправлять на неподходящую работу.\n\nСледующие медведи безработны:";

        bool coldIsue = false;
        string coldInfo = "Холод опасен, и мы не должны позволить ему одержать верх. Замерзая, медведи теряют здоровье и начинают испытывать стресс.\n\nСледующие медведи мёрзнут:";

        bool healthIsue = false;
        string healthInfo = "Если здоровье медведя достигнет критической точки (HP=1), он заболеет и не сможет работать. Для его восстановления потребуется построить медпункт, либо создать медведю комфортные условия для выздоровления (тепло и сытость).\n\nСледующие медведи имеют критический уровень здоровья:";

        bool sleepIsue = false;
        string sleepInfo = $"Похоже биологические часы догоняют мерзлоту этой планеты. В таком состоянии медведь недееспособен, вы же изучили лекарства от спячки ?\n\nАнтиспячкина на складе: {City._Storage._Antisleep} ед\nОни будут выданы автоматически, когда медведь заснёт.\n\nСледующие медведи в спячке или впадут в неё:";

        bool stresshIsue = false;
        float averageStress = 0;
        string stressInfo = "Медведи испытывают стресс при недостаточно комфортных условиях жизни. Он понижается дома в свободное от работы время.\n\nСледующие медведи испытывают стресс:";
        foreach (Bear bear in City._DataBase._Bears)
        {
            if(bear._Home == null && bear._Sally == null)
            {
                homeIsue = true;
                homeInfo += $"\n{bear._Kasta} {bear._Name}";
            }
            if(bear._Facility == null && bear._Sally == null)
            {
                workIsue = true;
                workInfo += $"\n{bear._Kasta} {bear._Name}";
            }
            if (bear._HeatLevel < 0)
            {
                coldIsue = true;
                coldInfo += $"\n{bear._Name}  {bear._HeatLevel}";
            }
            if (bear._Health < 3)
            {
                healthIsue = true;
                healthInfo += $"\n{bear._Name}  {bear._Health}";
            }
            if (bear._Effects[3] <= 50)
            {
                sleepIsue = true;
                sleepInfo += $"\n{bear._Name}  {bear._Effects[3]} ч";
            }
            float stress = bear._StressDelta;
            averageStress += bear._Stress;
            if (bear._Stress > 50 || stress > 1)
            {
                stresshIsue = true;
                stressInfo += $"\n{bear._Name}  {Mathf.RoundToInt(bear._Stress)}%  {(stress > 0 ? "+" : "")}{Mathf.RoundToInt(stress * CityTime._DaySection)} %/ч";
            }
        }
        if(City._DataBase._Bears.Length > 0)
        {
            averageStress /= City._DataBase._Bears.Length;

            if(averageStress > 40)
            {
                stresshIsue = true;
            }

            stressInfo += $"\n\nСреднее значение стресса всех медведей: {(int)averageStress}%";
        }

        float x = 50;

        if (City._Factors._StressTest)
        {
            StressTestTip._Info = $"<color=red>У ВАС {City._Factors._EndTime} Ч, ЧТОБЫ СТАБИЛИЗИРОВАТЬ СТРЕСС ДО 50% И МЕНЬШЕ!</color>";
            StressTestIsue.gameObject.SetActive(true);

            StressTestIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            StressTestIsue.gameObject.SetActive(false);
        }
        if (homeIsue)
        {
            HomeTip._Info = homeInfo ;
            HomeIsue.gameObject.SetActive(true);

            HomeIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            HomeIsue.gameObject.SetActive(false);
        }
        if (workIsue)
        {
            WorkTip._Info = workInfo;
            WorkIsue.gameObject.SetActive(true);

            WorkIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            WorkIsue.gameObject.SetActive(false);
        }
        if (coldIsue)
        {
            ColdTip._Info = coldInfo ;
            ColdIsue .gameObject.SetActive(true);

            ColdIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            ColdIsue.gameObject.SetActive(false);
        }
        if (stresshIsue)
        {
            StressTip._Info = stressInfo ;
            StressIsue .gameObject.SetActive(true);

            StressIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            StressIsue.gameObject.SetActive(false);
        }
        if (healthIsue)
        {
            HealthTip._Info = healthInfo;
            HealthIsue.gameObject.SetActive(true);

            HealthIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            HealthIsue .gameObject.SetActive(false);
        }
        if (sleepIsue)
        {
            SleepTip._Info = sleepInfo;
            SleepIsue.gameObject.SetActive(true);

            SleepIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            SleepIsue.gameObject.SetActive(false);
        }

        bool researchIsue = false;
        bool thereLaboratory = false;
        string researchInfo = "Если в лабораториях не ведутся разработки, это негативно сказывается на общем прогрессе развития нашей колонии.\n\nСледующие лаборатории простаивают без дела:";
        for(int i = 0; i < City._DataBase._Facilities.Length; i++)
        {
            if(City._DataBase._Facilities[i] is Laboratory)
            {
                if(City._DataBase._Facilities[i]._Bears.Length <= 0 || City._Research.GetResearchLevel((City._DataBase._Facilities[i] as Laboratory)._Target) >= 3)
                {
                    researchIsue = true;
                    researchInfo += $"\n{City._DataBase._Facilities[i]._ConstructInfo.Name} #{i}";
                }
                thereLaboratory = true;
            }
        }

        if (!thereLaboratory)
        {
            ResearchTip._Info = "Отсутствуют лаборатории, изучения невозможны. Рекомендуется построить хотя-бы одну лабораторию и назначить исследование.";
            ResearchIsue.gameObject.SetActive(true);

            ResearchIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else if (researchIsue)
        {
            ResearchTip._Info = researchInfo;
            ResearchIsue.gameObject.SetActive(true);

            ResearchIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            ResearchTip.gameObject.SetActive(false);
        }

        if ((int)(City._Foodstream._StoredFood / City._Foodstream.CurrentConsume() / 3) <= 3)
        {
            FoodTip._Info = $"Запасов еды хватает лишь на 3 дня или менее. Рекомендуется постройка пасеки, либо оптимизация потребления.\n\nТекущие темпы производства: {City._Foodstream.CurrentProduce()} ед/ч\nПотребление: {City._Foodstream.CurrentConsume() * 3} ед/сут\nКоличество дней, на которое хватит запаса еды: {(int)(City._Foodstream._StoredFood / City._Foodstream.CurrentConsume() / 3)}";
            FoodIsue.gameObject.SetActive(true);

            FoodIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            FoodIsue.gameObject .SetActive(false);
        }

        if(City._Energosystem.CurrentConsume() > City._Energosystem.CurrentProduce())
        {
            EnergosystemTip._Info = $"Потребление превосходит производство. Вам стоит построить электростанции, либо оптимизировать потребление электроэнергии.\n\nТекущие темпы производства: {City._Energosystem.CurrentProduce()} ед/ч\nПотребление: {City._Energosystem.CurrentConsume()} ед/ч";
            EnergosystemIsue.gameObject.SetActive(true);

            EnergosystemIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            EnergosystemIsue.gameObject.SetActive(false);
        }

        bool sallyIsue = false;
        string sallyInfo = "Если отряд разведки простаивает, скорее всего, ему не были отданы соответствующие приказы. Расформируйте его, либо отдайте распоряжения.\n\nСледующие отряды ожидают:";
        foreach(CitySally.Sally sally in City._CitySally._Sallies)
        {
            if (!sally._AllowMove)
            {
                sallyIsue = true;
                sallyInfo += $"\n{sally._Name}";
            }
        }

        if (sallyIsue)
        {
            SallyTip._Info = sallyInfo;
            SallyIsue.gameObject.SetActive(true);

            SallyIsue.anchoredPosition = new Vector2(x, 3);
            x += 60;
        }
        else
        {
            SallyIsue.gameObject.SetActive(false);
        }
    }
}
