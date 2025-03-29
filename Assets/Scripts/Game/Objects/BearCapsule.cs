
using System.Collections.Generic;
using UnityEngine;

public class BearCapsule : SaveableObject, IInteractable
{
    [SerializeField] private GameObject SoundPrefab;
    [SerializeField] private GameObject EffectPrefab;
    [SerializeField] private AudioClip AudioClip;

    [SerializeField] private Outline Indicator;
    [SerializeField] private Bear.Kasta BearKasta;
    [SerializeField] private Animator Capsule;
    [SerializeField] private BearObject BearObject;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private float Metal;
    [SerializeField] private float Timer = 0;
    private bool Opened = false;

    public override string _SaveInfo => base._SaveInfo + $"Kasta({BearKasta.GetHashCode()})Opened({Opened.GetHashCode()})";

    public string _Info => Opened ? "Собрать капсулу спасения" : "Открыть капсулу спасения";
    public bool _AbstractUse => false;

    public override void SetInfo(Dictionary<string, string> parameters)
    {
        base.SetInfo(parameters);

        Opened = parameters["Opened"] == "1";
        if (Opened)
        {
            Capsule.SetBool("Opened", true);
            Timer = -228;
            return;
        }

        BearKasta = (Bear.Kasta)(StaticTools.StringToInt(parameters["Kasta"]));

        Bear bear = BearObjectioner._Instance.RequestBear(BearKasta, transform.position.x, transform.position.z);

        BearObject = City._BearObjectioner.Create(bear);

        bear._BearObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        bear._BearObject._NavMeshAgent.Warp(bear._BearObject.transform.position);
        bear._BearObject.SetTask(new BearObject.ChillInCapsuleTask(bear._BearObject));

        BearObject.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    public void Interact()
    {
        if (!NewTutorialSystem.Instance._Closed && NewTutorialSystem.Instance._TutorialStage < 4)
        {
            return;
        }

        if (Opened)
        {
            if(Timer > 0)
            {
                return;
            }

            FindObjectOfType<PlayerVisual>().PlayAnimation(PlayerVisual.ActAnimations.GroundMine, Gather);
            return;
        }

        Capsule.SetBool("Opened", true);
        Opened = true;
        AudioSource.Play();
    }
    public void Gather()
    {
        Vector3 position = CameraChanger.Instance._Camera.position + CameraChanger.Instance._Camera.forward * 0.5f;
        AudioSource audioSource = Instantiate(SoundPrefab, position, transform.rotation).GetComponent<AudioSource>();
        Instantiate(EffectPrefab, transform.position + Vector3.up * 0.5f, transform.rotation);
        audioSource.clip = AudioClip;
        audioSource.pitch = Random.Range(0.8f, 1f);
        audioSource.Play();

        City._Storage._Metal += Metal;
        FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"Металл", $"+{Metal}" });
        City._CityStatistics.AddStatistic(new CityStatistics.Statistic("Разобрана капсула спасения", (int)City._Time._WorldTime / 60, Metal, CityStorage.ResourceType.Metal));

        NewTutorialSystem.Instance.SaveAbleCollected();

        Destroy(gameObject);
    }

    private void Update()
    {
        if (Opened)
        {
            if(Timer > 0)
            {
                Timer -= Time.unscaledDeltaTime;
            }
            else if(Timer != -228)
            {

                Bear bear = BearObject._Bear;
                bear._BearObject = BearObject;

                City._DataBase.RegisterBear(bear, false);

                if (bear._BearObject == null)
                {
                    bear._BearObject = City._BearObjectioner.Create(bear);

                    bear._BearObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                    bear._BearObject._NavMeshAgent.Warp(bear._BearObject.transform.position);
                }
                bear._BearObject.SetTask(new BearObject.RandomWalkTask(bear._BearObject, "бродит", 8));

                FindObjectOfType<FirstFaceMessenger>()?.Show(new string[] { $"{bear._Kasta} {bear._Name} спасён" });

                NewTutorialSystem.Instance.BearSaved();

                Timer = -228;
            }
        }
    }

    public void Indicate(bool state)
    {
        if (!NewTutorialSystem.Instance._Closed && NewTutorialSystem.Instance._TutorialStage < 4)
        {
            return;
        }

        if (Indicator != null)
        {
            Indicator.enabled = state;
        }
    }
}
