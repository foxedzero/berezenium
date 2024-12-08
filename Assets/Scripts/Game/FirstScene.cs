using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class FirstScene : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;

    [SerializeField] private GameObject TypeName;
    [SerializeField] private InputField InputField;

    [SerializeField] private GameObject SaveSynch;
    [SerializeField] private Text Loading;

    private bool Checked = false;

    private SaveData LocalData  = null;
    private SaveData CloudData = null;

    private void Start()
    {
        CursorManager.SetNeedMouse(new NeedCursorOrder(), false);
        
        string path = Path.Combine(Application.persistentDataPath, "LocalSave.json");

        LocalData = null;

        if (File.Exists(path))
        {
            try
            {
                LocalData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));
            }
            catch
            {
                LocalData = null;
                Debug.LogError("локальные данные оказались повреждены.");
            }

            if(LocalData.PlayerName == "")
            {
                LocalData = null;
            }
        }
        
        if(LocalData == null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || PlayerPrefs.GetString("playerName") == "")
            {
                TypeName.SetActive(true);
            }
            else
            {
                Loading.text = "Получение информации из сервера...";

                NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveCloudData);
            }
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                StartGame(LocalData);
            }
            else
            {
                Loading.text = "Получение информации из сервера...";

                NtoServerInterface.GetSaveData(LocalData.PlayerName, ReceiveCloudData);
            }
        }
    }

    public void ReceiveCloudData(string value)
    {
        Checked = true;

        Loading.text = "";

        if (value == "")
        {
            CloudData = null;
        }
        else
        {
            Nto.Player player = Newtonsoft.Json.JsonConvert.DeserializeObject<Nto.Player>(value);
            try
            {
                CloudData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(player.resources.SaveData);
            }
            catch
            {
                Debug.LogError("данные из сервера оказались повреждены.");
                CloudData = null;
            }
        }

        if(LocalData == null)
        {
            if(CloudData == null)
            {
                TypeName.SetActive(true);
            }
            else
            {
                UseLocalSave(false);
            }
            return;
        }

        if (CloudData == null || CloudData.Compare(LocalData))
        {
            UseLocalSave(true);
        }
        else
        {
            SaveSynch.SetActive(true);
        }
    }

    public void UseLocalSave(bool state)
    {
        if (state)
        {
            Loading.text = "Синхронизация с локальным файлом сохранения...";

            Nto.Log log = new Nto.Log();
            log.player_name = LocalData.PlayerName;
            log.comment = "Синхронизация с локальным файлом сохранения.";

            NtoServerInterface.SetResource(log, Nto.ResourceType.SaveData, Newtonsoft.Json.JsonConvert.SerializeObject(LocalData), SaveSyncronize);
        }
        else
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "LocalSave.json"), Newtonsoft.Json.JsonConvert.SerializeObject(CloudData));

            StartGame(CloudData);
        }

        SaveSynch.SetActive(false);
    }

    public void SaveSyncronize(string blablabla)
    {
        StartGame(LocalData);
    }

    public void NewGame()
    {
        string name = StaticTools.ToLegalFileName(InputField.text);

        if(name.Length < 1)
        {
            return;
        }

        TypeName.SetActive(false);

        PlayerPrefs.SetString("playerName", name);
        PlayerPrefs.SetInt("Tutorial", -1);
        PlayerPrefs.Save();


        SaveData saveData = new SaveData();
        saveData.PlayerName = name;

        saveData.Facilities = new string[0];
        //saveData.Facilities[1] = $"Type(Home (lvl 1))Transform(-29;-10;1)Enabled(1)Heater(0)Bears()";
        //saveData.Facilities[2] = $"Type(WoodES)Transform(18;-12;3)Enabled(0)Heater(0)Bears()";
        //saveData.Facilities[3] = $"Type(FoodStorage)Transform(-18;-25;0)Enabled(1)Heater(0)Bears()";

        saveData.Fields = new string[16];
        Vector2[] positions = new Vector2[16];
        for(int i = 0; i < 10; i++)
        {
            Vector2 newPosition = new Vector2();
            while (true)
            {
                newPosition = new Vector2(Random.Range(-230, 230), Random.Range(-230, 230));
                bool valid = true;

                if(newPosition.magnitude < 25)
                {
                    valid = false;
                }
                for (int ii = 0; ii < i; ii++)
                {
                    if (Vector2.Distance(positions[ii], newPosition) < 35)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    break;
                }
            }

            positions[i] = newPosition;
            saveData.Fields[i] = $"Transform({newPosition.x};{newPosition.y};{Random.Range(0, 4)})Resource(Metal)Count({Random.Range(300, 600)})";
        }
        for (int i = 10; i < 16; i++)
        {
            Vector2 newPosition = new Vector2();
            while (true)
            {
                newPosition = new Vector2(Random.Range(-230, 230), Random.Range(-230, 230));
                bool valid = true;

                if (newPosition.magnitude < 25)
                {
                    valid = false;
                }
                for (int ii = 0; ii < i; ii++)
                {
                    if (Vector2.Distance(positions[ii], newPosition) < 35)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    break;
                }
            }

            positions[i] = newPosition;
            saveData.Fields[i] = $"Transform({newPosition.x};{newPosition.y};{Random.Range(0, 4)})Resource(Berezenium)Count({Random.Range(200, 400)})";
        }

        string[] maleNames = Resources.Load<TextAsset>("MaleNames").text.Split("\n");
        string[] femaleNames = Resources.Load<TextAsset>("FemaleNames").text.Split("\n");

        Bear.Kasta[] kastaPool = new Bear.Kasta[] {Bear.Kasta.Конструктор, Bear.Kasta.Конструктор, Bear.Kasta.Конструктор, Bear.Kasta.Конструктор, Bear.Kasta.Программист, 
            Bear.Kasta.Программист, Bear.Kasta.Программист, Bear.Kasta.Программист, Bear.Kasta.Пасечник, Bear.Kasta.Пасечник, Bear.Kasta.Первопроходец, Bear.Kasta.Первопроходец};

        saveData.Bears = new string[12];
        for(int i = 0; i < 12; i++)
        {
            int age = Random.Range(16, 30);
            int skill = Random.Range(255, 355);

            bool woman = Random.Range(0, 10) % 2 == 0;

            int index = Random.Range(0, kastaPool.Length);
            Bear.Kasta kasta = kastaPool[index];
            kastaPool = StaticTools.ReduceMassive(kastaPool, index);

            Bear newBear = new Bear(kasta, (woman ? femaleNames[Random.Range(0, femaleNames.Length)] : maleNames[Random.Range(0, maleNames.Length)]), skill, Random.Range(7, 11), Random.Range(4, 9), age, woman);

            saveData.Bears[i] = newBear._SaveInfo;
        }

        saveData.GameTime = 0.22f;
        saveData.CityFactors = $"S$fn(Форс мажор;2;60|Оптимизм;2;15)";
        saveData.CityFoodstream = "100;0;0;1";
        saveData.CityStorage = "80;0;120;210;10";

        saveData.Weather = "4;0";

        File.WriteAllText(Path.Combine(Application.persistentDataPath, "LocalSave.json"), JsonUtility.ToJson(saveData));

        Nto.Player player = new Nto.Player();
        player.name = name;

        Nto.Resources resources  = new Nto.Resources();
        resources.LapoMoney = 1000;
        resources.SaveData = Newtonsoft.Json.JsonConvert.SerializeObject(saveData);
        resources.ItemsID = new string[0];

        player.resources = resources;

        Nto.Log log = new Nto.Log();
        log.comment = $"Игрок {name} создан.";
        log.player_name = player.name;

        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("LapoMoney", $"= 1000");
        dict.Add("SaveData", $"= {JsonUtility.ToJson(saveData)}");
        log.resources_changed = dict;

        Loading.text = "Создание записи игрока на сервере...";

        NtoServerInterface.CreatePlayer(player, log, PlayerCreated);

        LocalData = saveData;
    }

    public void PlayerCreated(string info)
    {
        StartGame(LocalData);
    }

    public void CheckPlayerExistence()
    {
        string name = StaticTools.ToLegalFileName(InputField.text);

        TypeName.SetActive(false);

        if (name.Length < 1)
        {
            return;
        }

        if (Checked)
        {
            NewGame();
            return;
        }

        Checked = true;

        Loading.text = "Поиск игрока на сервере...";

        NtoServerInterface.GetSaveData(name, ReceiveCloudData);
    }

    public void StartGame(SaveData data)
    {
        MainMenu.SetActive(true);
        FindObjectOfType<SaveManager>().SetData(data);
        Loading.text = "";
    }
}
