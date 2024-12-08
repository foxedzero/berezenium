using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip[] AudioClips;
    [SerializeField] private Animator Animator;
    private bool Opened = false;

    public bool _Opened
    {
        get
        {
            return Opened;
        }
        set
        {
            Opened = value;

            Animator.SetBool("isOpen", Opened);

            AudioSource.clip = AudioClips[Random.Range(0, AudioClips.Length)];
            AudioSource.pitch = Random.Range(0.95f, 1.05f);
            AudioSource.Play();
        }
    }

    public void Interact()
    {
        _Opened = !Opened;
    }
}
