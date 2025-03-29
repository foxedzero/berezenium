using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UserContent;

public class BearRedactor : MonoBehaviour
{
    [SerializeField] private Image[] BearIcon;

    [SerializeField] private InputField Name;
    [SerializeField] private Text Kasta;
    [SerializeField] private InputField Health;
    [SerializeField] private InputField Age;
    [SerializeField] private Text Face;
    [SerializeField] private Text Brows;
    [SerializeField] private Text Color;
    [SerializeField] private InputField WorkMultiplier;
    [SerializeField] private InputField StressDelta;
    [SerializeField] private InputField Regeneration;
    [SerializeField] private InputField Stamina;

    [SerializeField] private GameObject Model;

    [SerializeField] private BearObjectioner BearObjectioner;

    private CustomBear[] LoadedBears = new CustomBear[0];

    private CustomBear CustomBear = null;

    private void Start()
    {
        Randomize();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Randomize()
    {
        CustomBear bear = new CustomBear();

        string[] maleNames = Resources.Load<TextAsset>("MaleNames").text.Split("\n");

        int age = UnityEngine.Random.Range(16, 30);
        int face = UnityEngine.Random.Range(0, BearObjectioner.FaceCount);
        int brows = UnityEngine.Random.Range(0, BearObjectioner.BrowsCount);
        int color = UnityEngine.Random.Range(0, BearObjectioner.BodyColorCount);

        bear.Name = maleNames[Random.Range(0, maleNames.Length)];
        bear.Kasta = (Bear.Kasta)(Random.Range(1, 6));
        bear.Health = Random.Range(4, 8);
        bear.Age = age;
        bear.Face = face;
        bear.Brows = brows;
            bear.BodyColor = color;
                bear.WorkMultiplier = Random.Range(0.5f, 2f);
        bear.Stressful = Random.Range(-0.25f, 0.25f);
        bear.Regeneration = Random.Range(0, 50);
        bear.Restful = Random.Range(0, 0.5f);

        Load(bear);
    }

    public void Load()
    {
        string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path, "*.ub", SearchOption.AllDirectories);

        LoadedBears = new CustomBear[0];
        string[] variants = new string[0];
        int[] indexes = new int[0];
        for(int i = 0; i < files.Length; i++)
        {
            CustomBear bear = null;
            bool valid = true;
            try
            {
                bear = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomBear>(File.ReadAllText(files[i]));
            }
            catch
            {
                valid = false;
            }

            if (valid && bear != null)
            {
                LoadedBears = StaticTools.ExpandMassive(LoadedBears, bear);
                variants = StaticTools.ExpandMassive(variants, bear.Name);
                indexes = StaticTools.ExpandMassive(indexes, LoadedBears.Length - 1);
            }
        }
       
        if(variants.Length > 0)
        {
            UserInteract.AskVariants("Загрузить медведя", variants, indexes, Load);
        }
        else
        {
            UserInteract.AskMessage("Не найдены медведи !", "К сожалению в папке с пользовательским контентом не обнаружены конфигурации медведей.\nВы можете создать их здесь или импортировать извне.");
        }
    }
    public void Load(int index)
    {
        Load(LoadedBears[index]);
    }
    public void Load(CustomBear bear)
    {
        CustomBear = bear;

        Name.SetTextWithoutNotify(CustomBear.Name);
        Kasta.text = CustomBear.Kasta.ToString();
        Health.SetTextWithoutNotify(CustomBear.Health.ToString());
        Age.SetTextWithoutNotify(CustomBear.Age.ToString());
        Face.text = CustomBear.Face.ToString();
        Brows.text = CustomBear.Brows.ToString();

        switch (CustomBear.BodyColor)
        {
            case 0:
                Color.text = "Бурый";
                break;
            case 1:
                Color.text = "Полярный";
                break;
            case 2:
                Color.text = "Серый";
                break;
            case 3:
                Color.text = "Алый";
                break;
            case 4:
                Color.text = "Тёмный";
                break;
            case 5:
                Color.text = "Светло бурый";
                break;
        }

        WorkMultiplier.SetTextWithoutNotify($"{Mathf.Round(CustomBear.WorkMultiplier * 100)}%");
        StressDelta.SetTextWithoutNotify($"{Mathf.Round(CustomBear.Stressful)}%");
        Regeneration.SetTextWithoutNotify($"{Mathf.Round(CustomBear.Regeneration)}%");
        Stamina.SetTextWithoutNotify($"{Mathf.Round(CustomBear.Restful * 100)}%");

        UpdateIcon();
        UpdateModel();
    }

    public void UpdateIcon()
    {
        BearLook bearLook = BearObjectioner.GetBearIcon(new SuperBear(CustomBear));

        if(bearLook.Face == null)
        {
            BearIcon[0].sprite = bearLook.Head;
            BearIcon[0].color = new Color(1, 1, 1, 1);
            BearIcon[1].color = new Color(0, 0, 0, 0);
            BearIcon[2].color = new Color(0, 0, 0, 0);
        }
        else
        {
            BearIcon[0].sprite = bearLook.Head;
            BearIcon[0].color = bearLook.SkinColor;
            BearIcon[1].sprite = bearLook.Face;
            BearIcon[2].sprite = bearLook.Brows;
            BearIcon[1].color = new Color(1, 1, 1, 1);
            BearIcon[2].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateModel()
    {
        if (Model != null)
        {
            Destroy(Model);
        }

        BearObject bearObject = BearObjectioner.Create(new SuperBear(CustomBear));
        Model = bearObject.gameObject;
    }

    public void Save()
    {
        string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if(File.Exists(Path.Combine(path, $"{CustomBear.Name}.ub")))
        {
            UserInteract.AskConfirm("Медведь уже существует !", "Хотите ли перезаписать его данные ?", AbsoluteSave);
            return;
        }

        File.WriteAllText(Path.Combine(path, $"{CustomBear.Name}.ub"), Newtonsoft.Json.JsonConvert.SerializeObject(CustomBear));

        UserInteract.AskMessage("Файл успешно сохранён", "Конфигурацию медведя вы можете найти в папке игры, в пользовательском контенте.");
    }

    public void AbsoluteSave(bool state)
    {
        if (state)
        {
            string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(Path.Combine(path, $"{CustomBear.Name}.ub"), Newtonsoft.Json.JsonConvert.SerializeObject(CustomBear));

            UserInteract.AskMessage("Файл успешно сохранён", "Конфигурацию медведя вы можете найти в папке игры, в пользовательском контенте.");
        }
    }

    public void SetName(string name)
    {
        if(name.Length <= 0)
        {
            return;
        }

        if(name.Length > 30)
        {
            name = name.Substring(0, 30);
        }

        CustomBear.Name = name;
    }

    public void SetHealth(string value)
    {
        CustomBear.Health = Mathf.Clamp(StaticTools.StringToInt(value), 1, 10);
        Health.SetTextWithoutNotify(CustomBear.Health.ToString());
    }

    public void SetAge(string value)
    {
        CustomBear.Age = Mathf.Clamp(StaticTools.StringToInt(value), 16, 70);
        Age.SetTextWithoutNotify(CustomBear.Age.ToString());
    }

    public void SetWorkMultiplier(string value)
    {
        CustomBear.WorkMultiplier = Mathf.Max(StaticTools.StringToFloat(value), 0) / 100f;
        WorkMultiplier.SetTextWithoutNotify($"{Mathf.Round(CustomBear.WorkMultiplier * 100)}%");
    }

    public void SetStressDelta(string value)
    {
        CustomBear.Stressful = StaticTools.StringToFloat(value);
        StressDelta.SetTextWithoutNotify($"{Mathf.Round(CustomBear.Stressful)}%");
    }

    public void SetRestful(string value)
    {
        CustomBear.Restful = StaticTools.StringToFloat(value) / 100f;
        Stamina.SetTextWithoutNotify($"{Mathf.Round(CustomBear.Restful * 100)}%");
    }

    public void SetRegeneration(string value)
    {
        CustomBear.Regeneration = Mathf.Clamp(StaticTools.StringToFloat(value), 0, 100);
        Regeneration.SetTextWithoutNotify($"{Mathf.Round(CustomBear.Regeneration)}%");
    }

    public void SetKasta()
    {
        UserInteract.AskVariants("Специализация", new string[] { "Пасечник", "Конструктор", "Программист", "Биоинженер", "Первопроходец" }, new int[] {  1, 2, 3, 4, 5 }, SetKasta);
    }
    public void SetKasta(int index)
    {
        CustomBear.Kasta = (Bear.Kasta)index;
        Kasta.text = CustomBear.Kasta.ToString();

        UpdateModel();
    }

    public void SetColor()
    {
        UserInteract.AskVariants("Окрас меха", new string[] { "Бурый", "Полярный", "Серый", "Алый", "Тёмный", "Светло бурый" }, new int[] { 0, 1, 2, 3, 4, 5 }, SetColor);
    }
    public void SetColor(int index)
    {
        CustomBear.BodyColor = index;
        switch (CustomBear.BodyColor)
        {
            case 0:
                Color.text = "Бурый";
                break;
            case 1:
                Color.text = "Полярный";
                break;
            case 2:
                Color.text = "Серый";
                break;
            case 3:
                Color.text = "Алый";
                break;
            case 4:
                Color.text = "Тёмный";
                break;
            case 5:
                Color.text = "Светло бурый";
                break;
        }

        UpdateModel();
        UpdateIcon();
    }

    public void SetFace()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            CustomBear.Face = (CustomBear.Face + 1) % BearObjectioner.FaceCount;
        }
        else
        {
            CustomBear.Face --;
            if(CustomBear.Face < 0)
            {
                CustomBear.Face = BearObjectioner.FaceCount - 1;
            }
        }

        Face.text = CustomBear.Face.ToString();
        UpdateModel();
        UpdateIcon();
    }

    public void SetBrows()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            CustomBear.Brows = (CustomBear.Brows + 1) % BearObjectioner.BrowsCount;
        }
        else
        {
            CustomBear.Brows--;
            if (CustomBear.Brows < 0)
            {
                CustomBear.Brows = BearObjectioner.BrowsCount - 1;
            }
        }

        Brows.text = CustomBear.Brows.ToString();
        UpdateModel();
        UpdateIcon();
    }
}
