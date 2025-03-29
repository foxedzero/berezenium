using System.Collections.Generic;
using UnityEngine;

public class Crate : SaveableObject, IInteractable
{
    [SerializeField] private Outline Indicator;
    [SerializeField] private GameObject SoundPrefab;
    [SerializeField] private GameObject EffectPrefab;
    [SerializeField] private SnowMnut SnowMnut;
    [SerializeField] private AudioClip AudioClip;

    [SerializeField] private MeshRenderer MeshRenderer;

    [SerializeField] private float Wood;

    [SerializeField] private CityStorage.ResourceType ResourceType;
    [SerializeField] private int Resource;

    public override string _SaveInfo => base._SaveInfo + $"Resource({ResourceType.GetHashCode()})Amount({Resource})";
    public string _Info => "Открыть ящик";
    public bool _AbstractUse => false;

    public override void SetInfo(Dictionary<string, string> parameters)
    {
        base.SetInfo(parameters);

        ResourceType = (CityStorage.ResourceType)(StaticTools.StringToInt(parameters["Resource"]));
        Resource = StaticTools.StringToInt(parameters["Amount"]);

        if(UserContent._TextureLoader._BoxOverride != null)
        {
            MeshRenderer.material = UserContent._TextureLoader._BoxOverride;
        }
    }

    public void Interact()
    {
        SnowMnut.enabled = true;
        FindObjectOfType<PlayerVisual>().PlayAnimation( PlayerVisual.ActAnimations.GroundChop, Gather);
    }
    public void Gather()
    {
        Vector3 position = CameraChanger.Instance._Camera.position + CameraChanger.Instance._Camera.forward * 0.5f;
        AudioSource audioSource = Instantiate(SoundPrefab, position, transform.rotation).GetComponent<AudioSource>();
        Instantiate(EffectPrefab, transform.position + Vector3.up * 0.8f, transform.rotation);
        audioSource.clip = AudioClip;
        audioSource.pitch = Random.Range(0.75f, 0.9f);
        audioSource.Play();

        City._Storage._Wood += Wood;
        City._Storage.AddResource( ResourceType, Resource );
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Древесина ящика", (int)City._Time._WorldTime / 60, Wood, CityStorage.ResourceType.Wood));
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Припасы с ящика", (int)City._Time._WorldTime / 60, Resource, ResourceType));
        Destroy(gameObject);

        FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"Древесина", $"+{Wood}" });
        FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"{CityStorage.ResourceName(ResourceType)}", $"+{Resource}" });
        NewTutorialSystem.Instance.SaveAbleCollected();
    }

    public void Indicate(bool state)
    {
        if(Indicator != null)
        {
            Indicator.enabled = state;
        }
    }
}
