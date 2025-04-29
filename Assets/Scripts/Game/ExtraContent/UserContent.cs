using System.IO;
using UnityEngine;
using static Bear;

public class UserContent : MonoBehaviour
{
    static private UserContent Instance;

    [SerializeField] private AudioLoader AudioLoader;
    [SerializeField] private TextureLoader TextureLoader;
    [SerializeField] private CustomBear[] CustomBears;

    public static string _DirectoryName => "USERCONTENT";
    public CustomBear[] _Bears => CustomBears;
    public static UserContent _Instance => Instance;

    public static AudioLoader _AudioLoader => Instance.AudioLoader;
    public static TextureLoader _TextureLoader => Instance.TextureLoader;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateMods()
    {
        string path = Path.Combine(Application.dataPath, _DirectoryName);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path, "*.ub", SearchOption.AllDirectories);
        CustomBears = new CustomBear[0];
        foreach (string file in files)
        {
            CustomBear bear = null;
            bool valid = true;
            try
            {
                 bear = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomBear>(File.ReadAllText(file));
            }
            catch
            {
                valid = false;
            }

            if (valid && bear != null)
            {
                CustomBears = StaticTools.ExpandMassive(CustomBears, bear);
            }
        }

        AudioLoader.UpdateList();
        TextureLoader.UpdateList();
    }

    [System.Serializable]
    public class CustomBear
    {
        public string Name;
        public Kasta Kasta;

        public int Health; 
        public int Age; 

        public int Face;
        public int Brows;
        public int BodyColor;

        public float WorkMultiplier;
        public float Stressful;
        public float Restful;
        public float Regeneration;
    }
}
