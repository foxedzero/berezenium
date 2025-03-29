using Unity.AI.Navigation;
using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] private CityDataBase CityDataBase;
    [SerializeField] private CityEnergosystem CityEnergosystem;
    [SerializeField] private CityFactors CityFactors;
    [SerializeField] private CityFoodstream CityFoodstream;
    [SerializeField] private CityResearch CityResearch;
    [SerializeField] private CityStorage CityStorage;
    [SerializeField] private CityTime CityTime;
    [SerializeField] private CityMessenger CityMessenger;
    [SerializeField] private Constructor Constructor;
    [SerializeField] private CityGeology CityGeology;
    [SerializeField] private CitySchedule CitySchedule;
    [SerializeField] private CityNotation CityNotation;
    [SerializeField] private Weather Weather;
    [SerializeField] private BearObjectioner BearObjectioner;
    [SerializeField] private CitySally CitySally;
    [SerializeField] private ObjectLoader SaveableLoader;
    [SerializeField] private Pokazateli Pokazateli;
    [SerializeField] private CityStatistics CityStatistics;
    [SerializeField] private TerrainGenerator TerrainGenerator;

    [SerializeField] private PlayerPlacer PlayerPlacer;

    private static City Instance;
    public static City _Instance => Instance;
    public static CityDataBase _DataBase => Instance. CityDataBase;
    public static CityEnergosystem _Energosystem => Instance.CityEnergosystem;
    public static CityFactors _Factors => Instance.CityFactors;
    public static CityFoodstream _Foodstream => Instance.CityFoodstream;
    public static CityResearch _Research => Instance.CityResearch;
    public static CityStorage _Storage => Instance.CityStorage;
    public static CityTime _Time => Instance.CityTime;
    public static CityMessenger _CityMessenger => Instance.CityMessenger;
    public static Constructor _Constructor => Instance.Constructor;
    public static CityGeology _CityGeology => Instance.CityGeology;
    public static CitySchedule _CitySchedule => Instance.CitySchedule;
    public static CityNotation _CityNotation => Instance.CityNotation;
    public static Weather _Weather => Instance.Weather;
    public static BearObjectioner _BearObjectioner => Instance.BearObjectioner;
    public static CitySally _CitySally => Instance.CitySally;
    public static ObjectLoader _SaveableLoader => Instance.SaveableLoader;
    public static Pokazateli _Pokazateli => Instance.Pokazateli;
    public static CityStatistics _CityStatistics => Instance.CityStatistics;

    public static PlayerPlacer _PlayerPlacer => Instance.PlayerPlacer;

    private void Awake()
    {
        Instance = this;

        BearObjectioner.Initialize();
        TerrainGenerator.Initialize();

        SaveData SaveData = SaveManager._Instance._SaveData;

        CityGeology._SaveInfo = SaveData.Fields;

        CityDataBase.Load(SaveData);

        CityTime._WorldTime = SaveData.GameTime;
        Weather._SaveInfo = SaveData.Weather;
        CityStorage._SaveInfo = SaveData.CityStorage;
        CityFoodstream._SaveInfo = SaveData.CityFoodstream;
        CityEnergosystem._SaveInfo = SaveData.CityEnergosystem;
        CityResearch._SaveInfo = SaveData.Research;
        CitySally._SaveInfo = SaveData.CitySally;
        Pokazateli._SaveInfo = SaveData.ShowParameter;
        SaveableLoader.Load(SaveData.Saveables);
        CityStatistics._SaveInfo = SaveData.CityStatistics;
        CityFactors._SaveInfo = SaveData.CityFactors;

        Schedule[] schedules = new Schedule[SaveData.Schedules.Length];
        for(int i = 0; i < schedules.Length; i++)
        {
            schedules[i] = new Schedule(SaveData.Schedules[i]);
        }
        CitySchedule.SetUpSchedule(schedules);

        PlayerPlacer.PlacePlayer(SaveData.PlayerPosition);
    }

}
