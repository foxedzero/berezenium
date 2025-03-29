
using UnityEngine;

public class StartCapsule : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject SoundPrefab;
    [SerializeField] private GameObject EffectPrefab;
    [SerializeField] private AudioClip AudioClip;

    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private LayerMask TreeMask;

    [SerializeField] private Animator Capsule;
    [SerializeField] private Animator Player;
    [SerializeField] private Outline Indicator;
    [SerializeField] private PlayerMovement Movement;
    [SerializeField] private CapitansHotkey CapitansHotkey;
    [SerializeField] private Transform CameraPoint;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private Transform[] CameraParents;
    [SerializeField] private GameObject[] Colliders;
    [SerializeField] private bool Opened;
    [SerializeField] private float OpenTime = 5;
    [SerializeField] private float GetUpTime = 2;

    public bool _AbstractUse => false;

    public string _Info => Opened ? "Собрать капсулу спасения" : "Выйти из капсулы";

    public void Indicate(bool state)
    {
        if (Opened)
        {
            if (Indicator != null)
            {
                Indicator.enabled = state;
            }
        }
    }

    private void Start()
    {
        Debug.DrawRay(transform.position + Vector3.up * 50, Vector2.down * 200, Color.red);
        foreach (RaycastHit hit2 in Physics.RaycastAll(transform.position + Vector3.up * 50, Vector3.down, 200, TreeMask))
        {
            print(hit2.transform.gameObject);
            Destroy(hit2.transform.gameObject);
        }

        if (PlayerPrefs.GetInt("PlayerCapsuled") == 1)
        {
            if(Physics.Raycast(Vector3.up * 50, Vector3.down, out RaycastHit hit, 200, GroundMask))
            {
                transform.position = hit.point + Vector3.up * 0.25f;
                Movement.transform.position = hit.point + Vector3.up * 0.25f;
                PlayerPrefs.SetInt("PlayerCapsuled", 0);
                PlayerPrefs.Save();
                Player.Play("InCapsule");
                Movement._InCapsule = true;
                CapitansHotkey._InCapsule = true;
                CameraPoint.parent = CameraParents[0];
            }

        }
        else
        {

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Opened)
        {
            if(OpenTime > 0)
            {
                OpenTime -= Time.deltaTime;
            }
            else
            {
                Player.SetBool("InCapsule", false);

                if (GetUpTime > 0)
                {
                    GetUpTime -= Time.deltaTime;
                }
                else if (Movement._InCapsule)
                {
                    NewTutorialSystem.Instance.StartCapsuleQuit();
                    Movement._InCapsule = false;
                    CapitansHotkey._InCapsule = false;
                    CameraPoint.parent = CameraParents[1];
                    CameraPoint.localPosition = new Vector3(0, 0.6139981f, 0);
                    Colliders[0].SetActive(false);
                    Colliders[1].SetActive(true);
                    FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"Вы теперь можете двигаться" });
                }
            }
        }
    }

    public void Interact()
    {
        if (Opened)
        {
            if(OpenTime <= 0)
            {
                FindObjectOfType<PlayerVisual>().PlayAnimation(PlayerVisual.ActAnimations.GroundMine, Gather);
            }
            return;
        }

        Opened  = true;
        AudioSource.Play();
        Capsule.SetBool("Opened", true);
    }
    public void Gather()
    {
        AudioSource audioSource = Instantiate(SoundPrefab, transform.position, transform.rotation).GetComponent<AudioSource>();
        Instantiate(EffectPrefab, transform.position + Vector3.up * 0.8f, transform.rotation);
        audioSource.clip = AudioClip;
        audioSource.pitch = Random.Range(0.9f, 1f);
        audioSource.Play();

        City._Storage._Metal += 5;
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Разобрана капсула спасения", (int)City._Time._WorldTime / 60, 5, CityStorage.ResourceType.Metal));
        FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"Металл", $"+{5}" });

        Destroy(gameObject);
        NewTutorialSystem.Instance.SaveAbleCollected();
    }
}
