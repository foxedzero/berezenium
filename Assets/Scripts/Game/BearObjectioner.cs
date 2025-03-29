using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BearObjectioner : MonoBehaviour
{
    private static BearObjectioner Instance;

    [SerializeField] private BearModelData BearModels;
    [SerializeField] private Transform Map;

    [SerializeField] private GameObject SnowrunnerPrefab;

    [SerializeField] private Sprite Head;
    [SerializeField] private Sprite[] BearFaces;
    [SerializeField] private Sprite[] BearBrows;
    [SerializeField] private Color[] SkinColor;
    [SerializeField] private Material[] SkinMaterials;

    [SerializeField] private AudioClip[] PasechnikVoices;
    [SerializeField] private AudioClip[] ConstructorVoices;
    [SerializeField] private AudioClip[] ProgrammistVoices;
    [SerializeField] private AudioClip[] BioingenerVoices;
    [SerializeField] private AudioClip[] PervoprohodecVoices;

    public static BearObjectioner _Instance => Instance;

    public GameObject _SnowrunnerPrefab => SnowrunnerPrefab;
    public AudioClip[] _PasechnikVoices => PasechnikVoices;
    public AudioClip[] _ConstructorVoices => ConstructorVoices;
    public AudioClip[] _ProgrammistVoices => ProgrammistVoices;
    public AudioClip[] _BioingenerVoices => BioingenerVoices;
    public AudioClip[] _PervoprohodecVoices => PervoprohodecVoices;
    public Material[] _SkinMaterials => SkinMaterials;

    public static int FaceCount => 13;
    public static int BrowsCount => 12;
    public static int BodyColorCount => 10;

    public void Initialize()
    {
        Instance = this;
    }

    public Bear RequestBear(Bear.Kasta kasta, float x, float y)
    {
        foreach(UserContent.CustomBear customBear in UserContent._Instance._Bears)
        {
            if(customBear.Kasta == kasta)
            {
                bool valid = true;
                foreach(Bear bear in City._DataBase._Bears)
                {
                    if(bear._Name == customBear.Name)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return new SuperBear(customBear);
                }
            }
        }

        int health = Mathf.RoundToInt(Mathf.Lerp(4, 7, ProceduralTools.PseudoRandom($"{SaveManager._Seed}HEaздоровье{x}{kasta}{y}12k".GetHashCode())));

        string[] maleNames = Resources.Load<TextAsset>("MaleNames").text.Split("\n");
        string name = maleNames[Mathf.RoundToInt(Mathf.Lerp(0, maleNames.Length - 1, ProceduralTools.PseudoRandom($"{SaveManager._Seed}nameee{x}и{kasta}мя12k{y}".GetHashCode())))];

        int age = Mathf.RoundToInt(Mathf.Lerp(16, 35, ProceduralTools.PseudoRandom($"{SaveManager._Seed}Aзраст{x}{kasta}{y}аа".GetHashCode())));
        int face = Mathf.RoundToInt(Mathf.Lerp(0, FaceCount - 1, ProceduralTools.PseudoRandom($"ace{SaveManager._Seed}F{x}{kasta}{y}uu".GetHashCode())));
        int brows = Mathf.RoundToInt(Mathf.Lerp(0, BrowsCount - 1, ProceduralTools.PseudoRandom($"B{SaveManager._Seed}R{x}{kasta}О{y}ОВи".GetHashCode())));
        int color = Mathf.RoundToInt(Mathf.Lerp(0, BodyColorCount - 1, ProceduralTools.PseudoRandom($"{SaveManager._Seed}Ve{x}t{kasta}e{y}c".GetHashCode())));
        int antislee = age >= 20 ? Mathf.Max(100, Mathf.RoundToInt(Mathf.Lerp(900, 1000, ProceduralTools.PseudoRandom($"{SaveManager._Seed}не{x}{kasta}{y}спать".GetHashCode()))) - (int)(City._Time._WorldTime / 60)) : 9125 * Mathf.Abs(20 - age);

        Bear newBear = new Bear(kasta, name, health, age, face, brows, color, antislee);

        return newBear;
    }

    public BearObject Create(Bear bear)
    {
        GameObject prefab;
        Mesh brows;
        Mesh head;
        switch (bear._Kasta)
        {
            case Bear.Kasta.Пасечник:
                prefab = BearModels._PasechnikPrefab;
                brows = BearModels._PasechnikBrows[bear._Brows];
                head = BearModels._PasechnikHeads[bear._Face];
                break;
            case Bear.Kasta.Конструктор:
                prefab = BearModels._ConstructorPrefab;
                brows = BearModels._ConstructorBrows[bear._Brows];
                head = BearModels._ConstructorHeads[bear._Face];
                break;
            case Bear.Kasta.Программист:
                prefab = BearModels._ProgramistPrefab;
                brows = BearModels._ProgramistBrows[bear._Brows];
                head = BearModels._ProgramistHeads[bear._Face];
                break;
            case Bear.Kasta.Биоинженер:
                prefab = BearModels._BioingenerPrefab;
                brows = BearModels._BioingenerBrows[bear._Brows];
                head = BearModels._BioingenerHeads[bear._Face];
                break;
            case Bear.Kasta.Первопроходец:
                prefab = BearModels._PervoprohodecPrefab;
                brows = BearModels._PervoprohodecBrows[bear._Brows];
                head = BearModels._PervoprohodecHeads[bear._Face];
                break;
            default:
                prefab = BearModels._PasechnikPrefab;
                brows = BearModels._PasechnikBrows[bear._Brows];
                head = BearModels._PasechnikHeads[bear._Face];
                break;
        }

        BearObject bearObject = Instantiate(prefab, Map.transform).GetComponent<BearObject>();
        bearObject.SetBear(bear);
        bear._BearObject = bearObject;

        bearObject._Visual._Brows.sharedMesh = brows;
        bearObject._Visual._Head.sharedMesh = head;
        for(int i = 0; i < bearObject._Visual._Skin.Length; i++)
        {
            List<Material> materials = new List<Material>();
            bearObject._Visual._Skin[i].GetSharedMaterials(materials);
            materials[bearObject._Visual._SkinId[i]] = SkinMaterials[bear._BodyColor];
            bearObject._Visual._Skin[i].SetSharedMaterials(materials);
        }

        return bearObject;
    }

    public BearLook GetBearIcon(int face, int brows, int skinColor)
    {
        return new BearLook ( Head, BearFaces[face], BearBrows[brows], SkinColor[skinColor]);
    }

    public BearLook GetBearIcon(SuperBear superBear)
    {
        Texture2D texture = new Texture2D(128, 128);

        if (File.Exists(Path.Combine(Application.dataPath, UserContent._DirectoryName, $"{superBear._Name}.png")))
        {
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, UserContent._DirectoryName, $"{superBear._Name}.png")));

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f);

            return new BearLook(sprite, null, null, default);
        }
        else
        if (File.Exists(Path.Combine(Application.dataPath, UserContent._DirectoryName, $"{superBear._Name}.jpg")))
        {
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, UserContent._DirectoryName, $"{superBear._Name}.jpg")));

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f);

            return new BearLook(sprite, null, null, default);
        }
        else
        {
            return GetBearIcon(superBear._Face, superBear._Brows, superBear._BodyColor);
        }
    }
}

public class BearLook
{
    public Sprite Head;
    public Sprite Face;
    public Sprite Brows;
    public Color SkinColor;

    public BearLook(Sprite head, Sprite face, Sprite brows, Color skinColor)
    {
        Head = head;
        Face = face;
        Brows = brows;
        SkinColor = skinColor;
    }
}