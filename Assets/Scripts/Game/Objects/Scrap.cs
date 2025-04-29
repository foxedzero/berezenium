
using UnityEngine;

public class Scrap : SaveableObject, IInteractable
{
    [SerializeField] private Outline Indicator;
    [SerializeField] private GameObject SoundPrefab;
    [SerializeField] private GameObject EffectPrefab;
    [SerializeField] private Mesh[] Meshes;
    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private AudioClip AudioClip;

    [SerializeField] private MeshRenderer MeshRenderer;

    public string _Info => "Собрать обломок корабля";
    public bool _AbstractUse => false;

    private void Start()
    {
        MeshFilter.mesh = Meshes[Random.Range(0, Meshes.Length)];

        if (UserContent._TextureLoader._ScrapOverride != null)
        {
            MeshRenderer.material = UserContent._TextureLoader._ScrapOverride;
        }
    }

    public void Interact()
    {
        FindObjectOfType<PlayerVisual>().PlayAnimation(PlayerVisual.ActAnimations.GroundMine, Gather);
    }
    public void Gather()
    {
        Vector3 position = CameraChanger.Instance._Camera.position + CameraChanger.Instance._Camera.forward * 0.5f;
        AudioSource audioSource = Instantiate(SoundPrefab, position, transform.rotation).GetComponent<AudioSource>();
        Instantiate(EffectPrefab, transform.position, transform.rotation);
        audioSource.clip = AudioClip;
        audioSource.pitch = Random.Range(0.9f, 1f);
        audioSource.Play();

        int amount = Random.Range(3, 4);
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Собран металлолом от корабля", (int)City._Time._WorldTime / 60, amount, CityStorage.ResourceType.Metal));
        City._Storage._Metal += amount;
        Destroy(gameObject);

        FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"Металл", $"+{amount}" });
        NewTutorialSystem.Instance.SaveAbleCollected();
    }

    public void Indicate(bool state)
    {
        if (Indicator != null)
        {
            Indicator.enabled = state;
        }
    }
}
