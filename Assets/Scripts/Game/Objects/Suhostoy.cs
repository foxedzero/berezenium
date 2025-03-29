
using UnityEngine;

public class Suhostoy : SaveableObject, IInteractable
{
    [SerializeField] private GameObject SoundPrefab;
    [SerializeField] private GameObject EffectPrefab;
    [SerializeField] private Outline Indicator;
    [SerializeField] private AudioClip AudioClip;

    public string _Info => "Срубить сухостой";
    public bool _AbstractUse => false;

    public void Interact()
    {
        FindObjectOfType<PlayerVisual>().PlayAnimation(PlayerVisual.ActAnimations.GroundChop, Gather);
    }
    public void Gather()
    {
        Vector3 position = CameraChanger.Instance._Camera.position + CameraChanger.Instance._Camera.forward * 0.5f;
        AudioSource audioSource = Instantiate(SoundPrefab, position, transform.rotation).GetComponent<AudioSource>();
        Instantiate(EffectPrefab, transform.position, transform.rotation);
        audioSource.clip = AudioClip;
        audioSource.pitch = Random.Range(0.8f, 1f);
        audioSource.Play();

        int amount = Random.Range(14, 25);
        City._Storage._Wood += amount;
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Срублен сухостой", (int)City._Time._WorldTime / 60, amount, CityStorage.ResourceType.Wood));
        Destroy(gameObject);

        FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"Древесина", $"+{amount}" });
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
