using System.IO;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    private static GlobalVariables Instance;

    private GlobalVariablesData Data = null;

    public static float _SecondTimeMultiplier => Instance.Data.SecondTimeMultiplier;
    public static float _PlayerSpeed => Instance.Data.PlayerSpeed;
    public static float _BearFoodCost => Instance.Data.BearRequiredFood;
    public static int _ColdAddset => Instance.Data.ColdAddset;

    private void Awake()
    {
        Instance = this;

        string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string file = Path.Combine(path, "globalVars.txt");
        if (File.Exists(file))
        {
            try
            {
                Data = JsonUtility.FromJson<GlobalVariablesData>(File.ReadAllText(file));
            }
            catch
            {
                Debug.LogError("Файл глобальных переменных был повреждён.");

                Data = new GlobalVariablesData();
                File.WriteAllText(file, JsonUtility.ToJson(Data));
            }
        }
        else
        {
            Data = new GlobalVariablesData();
            File.WriteAllText(file, JsonUtility.ToJson(Data));
        }
    }

    public class GlobalVariablesData
    {
        public float SecondTimeMultiplier;
        public float PlayerSpeed;
        public float BearRequiredFood;
        public int ColdAddset;

        public  GlobalVariablesData()
        {
            SecondTimeMultiplier = 1;
            PlayerSpeed = 3.5f;
            BearRequiredFood = 1;
            ColdAddset = 0;
        }
    }
}
