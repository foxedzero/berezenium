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
    [SerializeField] private Weather Weather;
    [SerializeField] private CityBuyments CityBuyments;
    [SerializeField] private CityHeater CityHeater;

    private static City Instance;

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
    public static Weather _Weather => Instance.Weather;
    public static CityBuyments _Buyiments => Instance.CityBuyments;
    public static CityHeater _CityHeater => Instance.CityHeater;

    private void Awake()
    {
        Instance = this;

        FindObjectOfType<SaveManager>().Initialize();
    }
}
