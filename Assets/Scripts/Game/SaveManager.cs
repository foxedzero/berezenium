using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static SaveManager Instance;

    [SerializeField] private SaveData SaveData = null;

    public static SaveManager _Instance => Instance;
    public SaveData _SaveData => SaveData;

    public static string _Seed => Instance.SaveData.Seed;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        if(City._Instance != null)
        {
            if (City._DataBase != null)
            {
                Save();
            }
        }
    }

    public void SetData(SaveData data)
    {
        SaveData = data;
    }

    public void Save()
    {
        SaveData.GameTime = City._Time._WorldTime;

        SaveData.Bears = new string[City._DataBase._Bears.Length];
        for (int i = 0; i < SaveData.Bears.Length; i++)
        {
            SaveData.Bears[i] = City._DataBase._Bears[i]._SaveInfo;
        }

        SaveData.Facilities = new string[City._DataBase._Facilities.Length];
        for (int i = 0; i < SaveData.Facilities.Length; i++)
        {
            SaveData.Facilities[i] = City._DataBase._Facilities[i]._SaveInfo;
        }

        SaveData.Schedules = new string[City._CitySchedule._Schedules.Length];
        for (int i = 0; i < SaveData.Schedules.Length; i++)
        {
            SaveData.Schedules[i] = City._CitySchedule._Schedules[i]._SaveInfo;
        }

        SaveData.PlayerPosition = City._PlayerPlacer._SaveInfo;
        SaveData.CityStorage = City._Storage._SaveInfo;
        SaveData.CityFoodstream = City._Foodstream._SaveInfo;
        SaveData.CityEnergosystem = City._Energosystem._SaveInfo;
        SaveData.Research = City._Research._SaveInfo;
        SaveData.Fields = City._CityGeology._SaveInfo;
        SaveData.Weather = City._Weather._SaveInfo;
        SaveData.CitySally = City._CitySally._SaveInfo;
        SaveData.Saveables = City._SaveableLoader._GetInfo;
        SaveData.ShowParameter = City._Pokazateli._SaveInfo;
        SaveData.CityStatistics = City._CityStatistics._SaveInfo;
        SaveData.CityFactors = City._Factors._SaveInfo;

        string path = Path.Combine(Application.persistentDataPath, "LocalSave.json");

        File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(SaveData));
    }
}

[System.Serializable]
public class SaveData
{
    public string PlayerPosition = "X(0)Y(0)Z(0)XRot(0)YRot(0)";
    public string Seed = "";
    public int MapSize = 1;
    public float GameTime = 0;
    public bool StartBonus = false;

    public string[] Facilities = new string[0];
    public string[] Bears = new string[0];
    public string[] Fields = new string[0];
    public string[] Saveables = new string[0];

    public string CityStorage = "EHoney(0)Berezenium(0)Metal(0)Wood(0)Robots(0)Bee(0)Snowrunners(0)Astressin(0)Ratonik(0)Steamul(0)Antisleep(0)";
    public string CityEnergosystem = "Stored(0)Energosystem(0)";
    public string CityFoodstream = "Food(0)Saturation(1)ConsumeMode(1)";
    public string Research = "Electricity(0)Cold(0)Medicine(0)Travels(0)Household(0)Food(0)Production(0)Mining(0)";
    public string Weather = "Cold(2)";
    public string CitySally = "Contents(0;0;0;0;0;0;0;1;0;0;0;1;0;0;0;1;1;0;0;1;0;0;0;0;0;0;0;0;0;0;0)Count(0)";
    public string ShowParameter = "Show()";
    public string CityStatistics = $"Controls(0;0;0;0;0;0;0;0;0;0;0;0)Hour(0)Count(0)";
    public string CityFactors = $"Test(0)Time(0)";

    public string[] Schedules = new string[] { "Hours(0000000111111111111110000)Bears()" };
}

