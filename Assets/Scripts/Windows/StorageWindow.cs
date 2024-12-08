using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;

public class StorageWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private Text EnergyHoney;
    [SerializeField] private Text Berezenium;
    [SerializeField] private Text Metal;
    [SerializeField] private Text Wood;
    [SerializeField] private Text Robots;
    [SerializeField] private Text Bears;
    [SerializeField] private Text Facilities;
    [SerializeField] private Text Electricity;
    [SerializeField] private Text StoredFood;

    [SerializeField] private Tipper EnergyHoneyTip;
    [SerializeField] private Tipper BerezeniumTip;
    [SerializeField] private Tipper MetalTip;
    [SerializeField] private Tipper WoodTip;
    [SerializeField] private Tipper RobotsTip;
    [SerializeField] private Tipper BearsTip;
    [SerializeField] private Tipper FacilitiesTip;
    [SerializeField] private Tipper ElectricityTip;
    [SerializeField] private Tipper StoredFoodTip;

    public override string _Label => "Ресурсы города";

    private void OnDestroy()
    {
        City._Storage.OnChanges -= UpdateInfo;
        City._DataBase.OnBearChanges -= UpdateInfo;
        City._DataBase.OnFacilityChanges -= UpdateInfo;
        City._Time.DayChanged -= UpdateInfo;
    }

    private void Start()
    {
        City._Storage.OnChanges += UpdateInfo;
        City._DataBase.OnBearChanges += UpdateInfo;
        City._DataBase.OnFacilityChanges += UpdateInfo;
        City._Time.DayChanged += UpdateInfo;

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        EnergyHoney.text = $"Энергомёд: {City._Storage._EnergyHoney}";
        Berezenium.text = $"Березениум: {City._Storage._Berezenium}";
        Metal.text = $"Металл: {City._Storage._Metal}";
        Wood.text = $"Древесина: {City._Storage._Wood}";
        Robots.text = $"Роботы: {City._Storage._Robots}";
        Facilities.text = $"Здания: {City._DataBase._Facilities.Length}";
        Electricity.text = $"Электроэнергия: {City._Energosystem._StoredEnergy}/{City._Energosystem._EnergyCapacity}";
        StoredFood.text = $"Еда: {City._Foodstream._StoredFood}/{City._Foodstream._FoodCapacity}";

        IndexedProducer[] energyHoneyProduce = new IndexedProducer[0];
        IndexedProducer[] berezeniumHoneyProduce = new IndexedProducer[0];
        IndexedProducer[] metalHoneyProduce = new IndexedProducer[0];
        IndexedProducer[] woodHoneyProduce = new IndexedProducer[0];
        IndexedProducer[] robotsHoneyProduce = new IndexedProducer[0];
        IndexedHome[] homes = new IndexedHome[0];
        IndexedEnergy[] energyProcuders = new IndexedEnergy[0];
        IndexedFood[] foodProducers = new IndexedFood[0];

        Facility[] facilities = City._DataBase._Facilities;

        for (int i = 0; i < facilities.Length; i++)
        {
            Producer producer = facilities[i] as Producer;
            if (producer != null)
            {
                switch (producer._ResourceType)
                {
                    case CityStorage.ResourceType.EnergyHoney:
                        energyHoneyProduce = StaticTools.ExpandMassive(energyHoneyProduce, new IndexedProducer(producer, i));
                        break;
                    case CityStorage.ResourceType.Berezenium:
                        berezeniumHoneyProduce = StaticTools.ExpandMassive(berezeniumHoneyProduce, new IndexedProducer(producer, i));
                        break;
                    case CityStorage.ResourceType.Metal:
                        metalHoneyProduce = StaticTools.ExpandMassive(metalHoneyProduce, new IndexedProducer(producer, i));
                        break;
                    case CityStorage.ResourceType.Wood:
                        woodHoneyProduce = StaticTools.ExpandMassive(woodHoneyProduce, new IndexedProducer(producer, i));
                        break;
                    case CityStorage.ResourceType.Robots:
                        robotsHoneyProduce = StaticTools.ExpandMassive(robotsHoneyProduce, new IndexedProducer(producer, i));
                        break;
                }
            }
            else if (facilities[i] is Home)
            {
                homes = StaticTools.ExpandMassive(homes, new IndexedHome(facilities[i] as Home, i));
            }
            else if (facilities[i] is EnergyProcuder)
            {
                energyProcuders = StaticTools.ExpandMassive(energyProcuders, new IndexedEnergy(facilities[i] as EnergyProcuder, i));
            }
            else if (facilities[i] is FoodProducer)
            {
                foodProducers = StaticTools.ExpandMassive(foodProducers, new IndexedFood(facilities[i] as FoodProducer, i));
            }
        }

        string info = "";
        float summproduce = 0;
        foreach(IndexedProducer producer in energyHoneyProduce)
        {
            summproduce += producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal;
            info += $"\n{producer.Producer._ConstructInfo.Name} #{producer.Index}: {producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal}";
        }
        info = $"Энергомёд - основной ресурс и источник энергии медведей, который они смогли извлечь из простого мёда путём хитрых и даже гениальных химический решений. " +
            $"\nВ данный момент времени медведи смогли выйти на масштабное производство такого мёда, однако из-за переизбытка некоторые медведи теряют смысл куда-либо стремиться и уходят в зажировку." +
            $"\nМы оказались отрезаны от логистических узлов и цивилизации, поэтому придётся производить его самостоятельно." +
            $"\n\nПотенциальное производство: {summproduce}\n" + info;
        EnergyHoneyTip._Info = info;

        info = "";
        summproduce = 0;
        foreach (IndexedProducer producer in berezeniumHoneyProduce)
        {
            summproduce += producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal;
            info += $"\n{producer.Producer._ConstructInfo.Name} #{producer.Index}: {producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal}";
        }
        info = $"Березениум - новый элемент, названный в честь плнеты месторождения Березия.\nУчёных заинтересовал данный минерал, судя по данным и изображениям, полученные с помощью зондов." +
            $"\nЧтобы поближе изучить данный элемент был подготовлен исследовательский экипаж и космолёт, который мы возглавили." +
            $"\nЭлемент таит множество интересных свойств, одно из которых антигравитационное, поэтому его стоит извучить." +
            $"\n\nПотенциальное производство: {summproduce}\n" + info;
        BerezeniumTip._Info = info;

        info = "";
        summproduce = 0;
        foreach (IndexedProducer producer in metalHoneyProduce)
        {
            summproduce += producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal;
            info += $"\n{producer.Producer._ConstructInfo.Name} #{producer.Index}: {producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal}";
        }
        info = $"Металл - стандартный и незаменимый ресурс во всей вселенной.\nЕго используют для сооружения зданий, кораблей, самолётов, в создании электроники, в том числе роботов и дронов.\n\nПотенциальное производство: {summproduce}\n" + info;
        MetalTip._Info = info;

        info = "";
        summproduce = 0;
        foreach (IndexedProducer producer in woodHoneyProduce)
        {
            summproduce += producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal;
            info += $"\n{producer.Producer._ConstructInfo.Name} #{producer.Index}: {producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal}";
        }
        info = $"Древесина - отличный органический ресурс, который можно будет отыскать почти на каждой планете, заселённой жизнью." +
            $"\nПредки медведей активно использовали древесину для застройки своих берлог, а также для их отапливания." +
            $"\n\nПотенциальное производство: {summproduce}\n" + info;
        WoodTip._Info = info;

        info = "";
        summproduce = 0;
        foreach (IndexedProducer producer in robotsHoneyProduce)
        {
            summproduce += producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal;
            info += $"\n{producer.Producer._ConstructInfo.Name} #{producer.Index}: {producer.Producer.GetPotencialEffectivity() * producer.Producer._Nominal}";
        }
        info = $"Роботы и дроны - веха современной эры, позволяющая медведям реже заниматься тяжёлой работой и мелкими перевозками.\n" +
            $"С появлением оных немедленно развилась специальность программиста, которая подразумевает их управление, настраивание и алгоритмизацию." +
            $"\n\nПотенциальное производство: {summproduce}\n" + info;
        RobotsTip._Info = info;

        info = "";
        summproduce = 0;
        foreach(IndexedHome home in homes)
        {
            summproduce += home.Home._MaxBearCount;
            info += $"\n{home.Home._ConstructInfo.Name} #{home.Index}: {home.Home._MaxBearCount}";
        }
        Bears.text = $"Медведи: {City._DataBase._Bears.Length}/{summproduce}";
        BearsTip._Info = $"Медведь - разумное живое существо. Своими технологиями и упорством смогли возглавить свою планету и другие планеты путём кротовых нор." +
            $"\nЕсли сравнивать их с людьми, медведи менее агрессивны к друг другу, более индивидуальны, возможно из-за своей немногочисленности, и рациональны, так как большинство медведей технари." +
            $"\nНепростые условия на родной планете вынудили их акцентировать внимание на воздушном транспорте, но медведи смотрели выше и смогли покорить космос." +
            $"\nНесмотря на текущий достаток, они не перестают работать и учиться. " +
            $"\n\nСуммарная вместимость: {summproduce}\n" + info;

        info = "";
        summproduce = 0;
        foreach(IndexedEnergy procuder in energyProcuders)
        {
            summproduce += procuder.Producer.GetPotencialEffectivity() * procuder.Producer._BaseProduce;
            info += $"\n{procuder.Producer._ConstructInfo.Name} #{procuder.Index}: {procuder.Producer.GetPotencialEffectivity() * procuder.Producer._BaseProduce}";
        }
        ElectricityTip._Info = $"Электричество - важный ресурс города, позволяющий ему функционировать." +
            $"\nВ обжитых городах проблемы с током не возникают из-за обилия энергомёда." +
            $"\nМы же находимся не на родных планетах, поэтому мы должны сделать стабильную энергосистему своими руками." +
            $"\nКогда потребление электричества превысит производство, потребителям придётся снизить свою мощность, что значительно повлияет на их работу." +
            $"\n\nПотенциальная выработка: {summproduce}\n" + info;

        info = "";
        summproduce = 0;
        foreach (IndexedFood procuder in foodProducers)
        {
            summproduce += procuder.Producer.GetPotencialEffectivity() * procuder.Producer._BaseProduce;
            info += $"\n{procuder.Producer._ConstructInfo.Name} #{procuder.Index}: {procuder.Producer.GetPotencialEffectivity() * procuder.Producer._BaseProduce}";
        }
        StoredFoodTip._Info = $"Еда - все съедобное медведем, в нашем случае есть только мёд." +
            $"\nОбычные города всегда прокормят своих медведей, главное чтобы медведь не ушёл в зажировку. Здесь же придётся перейти на самообеспечение." +
            $"\nИз мёда на химических заводах производят энергомёд, который уже не стоит употреблять в виде еды." +
            $"\nВажно следить, чтобы медведи были сыты, иначе они будут недовольны, а их работоспособность уменьшится." +
            $"\n\nПотенциальное производство: {summproduce}\n" + info;
    }

    private class IndexedProducer
    {
        public Producer Producer;
        public int Index;

        public IndexedProducer(Producer producer, int index)
        {
            Producer = producer;
            Index = index;
        }
    }
    private class IndexedHome
    {
        public Home Home;
        public int Index;

        public IndexedHome(Home home, int index)
        {
            Home = home;
            Index = index;
        }
    }
    private class IndexedEnergy
    {
        public EnergyProcuder Producer;
        public int Index;

        public IndexedEnergy(EnergyProcuder producer, int index)
        {
            Producer = producer;
            Index = index;
        }
    }
    private class IndexedFood
    {
        public FoodProducer Producer;
        public int Index;

        public IndexedFood(FoodProducer producer, int index)
        {
            Producer = producer;
            Index = index;
        }
    }
}
