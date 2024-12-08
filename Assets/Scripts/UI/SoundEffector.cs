using UnityEngine;

public class SoundEffector : MonoBehaviour
{
     private static SoundEffector Instance;

    [SerializeField] private AudioSource FacilityIntro;

    [SerializeField] private AudioSource EnableDisable;
    [SerializeField] private AudioClip[] EnableDisableClips;

    [SerializeField] private AudioSource UISource;
    [SerializeField] private AudioClip[] UIClips;

    private void Awake()
    {
        Instance = this;
    }

    public static void PlayUI(int sound)
    {
        Instance.UISource.clip = Instance.UIClips[sound];
        Instance.UISource.pitch = Random.Range(0.90f, 1.1f);

        Instance.UISource.Play();
    }

    public static void PlayFasilityIntro(AudioClip clip)
    {
        Instance.FacilityIntro.clip = clip;
        Instance.FacilityIntro.Play();
    }

    public static void PlayEnableDisable(bool state)
    {
        Instance.EnableDisable.clip = Instance. EnableDisableClips[state.GetHashCode()];
        Instance.EnableDisable.Play();
    }
}
