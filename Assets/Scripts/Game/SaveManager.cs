using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    private SaveData SaveData = null;

    public string _PlayerName => SaveData.PlayerName;

    private void OnApplicationQuit()
    {
        if (City._DataBase != null)
        {
            Save();
        }
    }

    public void SetData(SaveData data)
    {
        SaveData = data;
    }

    public void Initialize()
    {
        string path = Path.Combine(Application.persistentDataPath, "LocalSave.json");
        if (File.Exists(path))
        {
            SaveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }

        if(SaveData != null)
        {
            City._CityGeology._SaveInfo = SaveData.Fields;

            City._DataBase.Load(SaveData);

            City._Time._Time = SaveData.GameTime;
            City._Weather._SaveInfo = SaveData.Weather;
            City._CityMessenger._SaveInfo = SaveData.CityMessages;
            City._Storage._SaveInfo = SaveData.CityStorage;
            City._Foodstream._SaveInfo = SaveData.CityFoodstream;
            City._Energosystem._SaveInfo = SaveData.CityEnergosystem;
            City._Factors._SaveInfo = SaveData.CityFactors;
            City._Research._SaveInfo = SaveData.Research;

            SaveData.Bears = null;
            SaveData.Facilities = null;
            SaveData.CityFactors = null;
        }
        else
        {
            SaveData = new SaveData();
        }

        City._Time.DayChanged += Save;
    }

    public void Save(NtoServerInterface.StringInfo callback)
    {
        SaveData.GameTime = Mathf.Round(City._Time._Time * 100000000) / 100000000f;

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

        SaveData.CityStorage = City._Storage._SaveInfo;
        SaveData.CityFoodstream = City._Foodstream._SaveInfo;
        SaveData.CityEnergosystem = City._Energosystem._SaveInfo;
        SaveData.CityFactors = City._Factors._SaveInfo;
        SaveData.Research = City._Research._SaveInfo;
        SaveData.CityMessages = City._CityMessenger._SaveInfo;
        SaveData.Fields = City._CityGeology._SaveInfo;
        SaveData.Weather = City._Weather._SaveInfo;

        string path = Path.Combine(Application.persistentDataPath, "LocalSave.json");

        if(callback != null)
        {
            Nto.Log log = new Nto.Log();
            log.player_name = SaveData.PlayerName;
            log.comment = "Сохранение игры.";

            NtoServerInterface.SetResource(log, Nto.ResourceType.SaveData, Newtonsoft.Json.JsonConvert.SerializeObject(SaveData), callback);
        }

        File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(SaveData));
    }

    public void Save()
    {
        Save(null);
    }
}

public class SaveData
{
    public string PlayerName;
    public float GameTime = 0;

    public string[] Facilities = new string[0];
    public string[] Bears = new string[0];
    public string[] Fields = new string[0];

    public string CityStorage = "0;0;0;0;0";
    public string CityEnergosystem = "0;0;0;1";
    public string CityFoodstream = "0;0;0;1";
    public string CityFactors = "";
    public string Research = "0;0;0;0;0;0;0;0";
    public string Weather = "0;0";

    public string[] CityMessages = new string[0];

    public bool Compare(SaveData saveData)
    {
        if (saveData == null)
        {
            return false;
        }

        if (CityStorage != saveData.CityStorage)
        {
            return false;
        }

        if (CityEnergosystem != saveData.CityEnergosystem)
        {
            return false;
        }

        if (CityFactors != saveData.CityFactors)
        {
            return false;
        }

        if (CityFoodstream != saveData.CityFoodstream)
        {
            return false;
        }

        if (Research != saveData.Research)
        {
            return false;
        }

        if (Weather != saveData.Weather)
        {
            return false;
        }

        if (PlayerName != saveData.PlayerName)
        {
            return false;
        }

        if (GameTime != saveData.GameTime)
        {
            return false;
        }
        
        if(Facilities.Length != saveData.Facilities.Length)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < Facilities.Length; i++)
            {
                if(Facilities[i] != saveData.Facilities[i])
                {
                    return false;
                }
            }
        }

        if (Bears.Length != saveData.Bears.Length)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < Bears.Length; i++)
            {
                if (Bears[i] != saveData.Bears[i])
                {
                    return false;
                }
            }
        }

        if (Fields.Length != saveData.Fields.Length)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < Fields.Length; i++)
            {
                if (Fields[i] != saveData.Fields[i])
                {
                    return false;
                }
            }
        }

        if (CityMessages.Length != saveData.CityMessages.Length)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < CityMessages.Length; i++)
            {
                if (CityMessages[i] != saveData.CityMessages[i])
                {
                    return false;
                }
            }
        }

        return true;
    }
}

