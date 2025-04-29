using UnityEngine;
using System.IO;

public class FirstScene : MonoBehaviour
{
    [SerializeField] private MapGenerationPanel MapGeneration;

    public SaveData GetData()
    {
        string path = Path.Combine(Application.persistentDataPath, "LocalSave.json");

        SaveData data = null;

        if (File.Exists(path))
        {
            try
            {
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));
            }
            catch
            {
                data = null;
                Debug.LogError("локальные данные оказались повреждены.");
            }
        }

        if(data == null)
        {
            if(MapGeneration._Seed != "")
            {
                return NewGame();
            }
        }

        return data;
    }

    public SaveData NewGame()
    {
        PlayerPrefs.SetInt("Tutorial", -1);
        PlayerPrefs.SetInt("TutorialTip", 0);
        PlayerPrefs.SetInt("PlayerCapsuled", 1);
        PlayerPrefs.Save();

        SaveData saveData = new SaveData();
        saveData.Seed = MapGeneration._Seed;
        saveData.MapSize = MapGeneration._Size;

        saveData.Facilities = new string[0];

        int bounds = 0;
        switch (MapGeneration._Size)
        {
            case 0:
                bounds = 150;
                break;
            case 1:
                bounds = 175;
                break;
            case 2:
                bounds = 200;
                break;
        }

        saveData.Fields = new string[5];
        Vector2[] positions = new Vector2[5];
        for (int i = 0; i < 3; i++)
        {
            Vector2 newPosition = new Vector2();

            for (int iter = 0; iter < 100; iter++)
            {
                newPosition = Vector2.zero;
                newPosition.x = Mathf.Lerp(-bounds/2+10, bounds/2-10, ProceduralTools.PseudoRandom((saveData.Seed + $"x{iter}met{i}").GetHashCode()));
                newPosition.y = Mathf.Lerp(-bounds / 2 + 10, bounds / 2 - 10, ProceduralTools.PseudoRandom((saveData.Seed + $"y{iter}met{i}").GetHashCode()));
                bool valid = true;

                if (newPosition.magnitude < 45)
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
            saveData.Fields[i] = $"Transform({newPosition.x};{newPosition.y};0)Resource(Metal)";
        }
        for (int i = 3; i < 5; i++)
        {
            Vector2 newPosition = new Vector2();
            for (int iter = 0; iter < 100; iter++)
            {
                newPosition = Vector2.zero;
                newPosition.x = Mathf.Lerp(-bounds / 2 + 10, bounds / 2 - 10, ProceduralTools.PseudoRandom((saveData.Seed + $"x{iter}ber{i}").GetHashCode()));
                newPosition.y = Mathf.Lerp(-bounds / 2 + 10, bounds / 2 - 10, ProceduralTools.PseudoRandom((saveData.Seed + $"y{iter}ber{i}").GetHashCode()));
                bool valid = true;

                if (newPosition.magnitude < 45)
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
            saveData.Fields[i] = $"Transform({newPosition.x};{newPosition.y};0)Resource(Berezenium)";
        }

        Bear.Kasta[] kastaPool = new Bear.Kasta[] {Bear.Kasta.Конструктор, Bear.Kasta.Первопроходец};

        int wirate = PlayerPrefs.GetInt("PlayerWinrate");

        int crateCount = Mathf.RoundToInt(30 * Mathf.Max(0.35f, 1 - wirate * 0.15f)); // 7% роботы, 67% мёд, 26% энергомёд
        int suhostoyCount = Mathf.RoundToInt(20 * Mathf.Max(0.35f, 1 - wirate * 0.15f));
        int scrap = Mathf.RoundToInt(10 * Mathf.Max(0.35f, 1 - wirate * 0.15f));

        saveData.Saveables = new string[2 + crateCount + suhostoyCount+ scrap];
        for(int i =0; i < 2; i++)
        {
            saveData.Saveables[i] = $"Prefab(BearCapsule)Transform({RandomSpot(20, saveData.Seed + $"capsule{i}po")})Kasta({kastaPool[i].GetHashCode()})Opened(0)";
        }
        for(int i = 2; i < crateCount + 2; i++)
        {
            saveData.Saveables[i] = $"Prefab(Crate)Transform({RandomSpot(bounds/2, saveData.Seed + $"crate{i - 2}gg")}){GenerateCrate(saveData.Seed, i - 2, crateCount)}";
        }
        for (int i = crateCount + 2; i < crateCount + 2 + scrap; i++)
        {
            saveData.Saveables[i] = $"Prefab(Scrap)Transform({RandomSpot(bounds / 2, saveData.Seed + $"scrap{i - 2 - crateCount}sdafa")})";
        }
        for(int i = crateCount + 2 + scrap; i < crateCount + 2 + scrap + suhostoyCount; i++)
        {
            saveData.Saveables[i] = $"Prefab(Suhostoy)Transform({RandomSpot(bounds / 2, saveData.Seed + $"suh{i - 2 - scrap - crateCount}fafass")})";
        }

        saveData.Bears = new string[0];

        if (MapGeneration._StartBonus)
        {
            saveData.CityStorage = "EHoney(25)Berezenium(0)Metal(25)Wood(75)Robots(1)Bee(0)Snowrunners(0)Astressin(0)Ratonik(0)Steamul(0)Antisleep(10)";
        }
        //saveData.CityStorage = "EHoney(1000)Berezenium(1000)Metal(1000)Wood(1000)Robots(1000)Bee(1000)Snowrunners(1000)Astressin(1000)Ratonik(1000)Steamul(1000)Antisleep(1000)";

        File.WriteAllText(Path.Combine(Application.persistentDataPath, "LocalSave.json"), JsonUtility.ToJson(saveData));

        return saveData;
    }

    private string RandomSpot(int size, string seed)
    {
        Vector2 position = Vector3.zero ;

        int iter = 0;
        do
        {
            position = Vector2.zero ;
            position.x = Mathf.Lerp(-size, size, ProceduralTools.PseudoRandom((seed + $"xalp{iter}ha").GetHashCode()));
            position.y = Mathf.Lerp(-size, size, ProceduralTools.PseudoRandom((seed + $"y{iter}hjjjkk").GetHashCode()));

            iter++;
        }
        while (position.magnitude < 3);

        return $"{position.x};{position.y};{Mathf.Lerp(0, 360, ProceduralTools.PseudoRandom((seed + $"rot").GetHashCode()))}" ;
    }

    private string GenerateCrate(string seed, float index, int max)
    {
        float percent = index / max;

        if(percent <= 0.07f)
        {
            return $"Resource(4)Amount(1)";
        }
        else if(percent <= 0.74f)
        {
            return $"Resource(6)Amount({(Mathf.RoundToInt(Mathf.Lerp(6, 10, ProceduralTools.PseudoRandom((seed + $"cratehoney{index}").GetHashCode()))))})";
        }
        else
        {
            return $"Resource(0)Amount({(Mathf.RoundToInt(Mathf.Lerp(3, 5, ProceduralTools.PseudoRandom((seed + $"crateenerhoney{index}").GetHashCode()))))})";
        }
    }
}
