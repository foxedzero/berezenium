using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureLoader : MonoBehaviour
{
    [SerializeField] private Material DefaultMaterial;
    [SerializeField] private Material SnowMaterial;
    [SerializeField] private Texture2D DefaultSnowTexture;

    [SerializeField] private GameObject[] TreePrefabs;
    [SerializeField] private GameObject[] RockPrefabs;
    [SerializeField] private Material DefaultTreeMaterial;
    [SerializeField] private Material DefaultRockMaterial;

    private Material BoxOverride = null;
    private Material ScrapOverride = null;

    public Material _BoxOverride => BoxOverride;
    public Material _ScrapOverride => ScrapOverride;

    public void UpdateList()
    {
        string path = Path.Combine(Application.dataPath, UserContent._DirectoryName);

        BoxOverride = null;
        ScrapOverride = null;

        string texturePath = Path.Combine(path, "snow.png");
        if (File.Exists(texturePath))
        {
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(File.ReadAllBytes(texturePath));

            SnowMaterial.SetTexture("_BaseMap", texture);
        }
        else
        {
            SnowMaterial.SetTexture("_BaseMap", DefaultSnowTexture);
        }

        texturePath = Path.Combine(path, "bereza.png");
        if (File.Exists(texturePath))
        {
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(File.ReadAllBytes(texturePath));

            Material bereza = new Material(DefaultMaterial);
            bereza.SetTexture("_BaseMap", texture);
            bereza.name = Path.GetFileNameWithoutExtension(texturePath);
            foreach (GameObject prefab in TreePrefabs)
            {
                prefab.GetComponentInChildren<MeshRenderer>().material = bereza;
            }
        }
        else
        {
            foreach (GameObject prefab in TreePrefabs)
            {
                prefab.GetComponentInChildren<MeshRenderer>().material = DefaultTreeMaterial;
            }
        }

        texturePath = Path.Combine(path, "box.png");
        if (File.Exists(texturePath))
        {
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(File.ReadAllBytes(texturePath));

            BoxOverride = new Material(DefaultMaterial);
            BoxOverride.SetTexture("_BaseMap", texture);
        }

        texturePath = Path.Combine(path, "rock.png");
        if (File.Exists(texturePath))
        {
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(File.ReadAllBytes(texturePath));

            Material rock = new Material(DefaultMaterial);
            rock.SetTexture("_BaseMap", texture);
            rock.name = Path.GetFileNameWithoutExtension(texturePath);
            foreach (GameObject prefab in RockPrefabs)
            {
                prefab.GetComponentInChildren<MeshRenderer>().material = rock;
            }
        }
        else
        {
            foreach (GameObject prefab in RockPrefabs)
            {
                prefab.GetComponentInChildren<MeshRenderer>().material = DefaultRockMaterial;
            }
        }

        texturePath = Path.Combine(path, "scrap.png");
        if (File.Exists(texturePath))
        {
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(File.ReadAllBytes(texturePath));

            ScrapOverride = new Material(DefaultMaterial);
            ScrapOverride.SetTexture("_BaseMap", texture);
        }
    }
}
