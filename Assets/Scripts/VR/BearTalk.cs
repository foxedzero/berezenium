using UnityEngine;
using static BearObject;

public class BearTalk : MonoBehaviour
{
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Transform HeadBone;
    [SerializeField] private SkinnedMeshRenderer Head;
    [SerializeField] private Bear.Kasta Kasta;
    [SerializeField] private AudioSource AudioSource;

    private float Rotation = 0;
    private float EndTime = 0;

    private float TargetLipsing = 0;
    private float CurrentLipsing = 0;
    private float[] SampleData;
    private float ClipTime = 0.05f;

    private bool Play = false;

    public void Talk()
    {
        if (Play)
        {
            return;
        }

        Rotation = transform.eulerAngles.y;

        switch (Kasta)
        {
            case Bear.Kasta.Пасечник:
                AudioSource.clip = BearObjectioner._Instance._PasechnikVoices[Random.Range(0, 8)];
                break;
            case Bear.Kasta.Конструктор:
                AudioSource.clip = BearObjectioner._Instance._ConstructorVoices[Random.Range(0, 10)];
                break;
            case Bear.Kasta.Программист:
                AudioSource.clip = BearObjectioner._Instance._ProgrammistVoices[Random.Range(0, 8)];
                break;
            case Bear.Kasta.Биоинженер:
                AudioSource.clip = BearObjectioner._Instance._BioingenerVoices[Random.Range(0, 9)];
                break;
            case Bear.Kasta.Первопроходец:
                AudioSource.clip = BearObjectioner._Instance._PervoprohodecVoices[Random.Range(0, 9)];
                break;
            default:
                AudioSource.clip = BearObjectioner._Instance._PasechnikVoices[Random.Range(0, 8)];
                break;
        }

        AudioSource.Play();
        EndTime = Time.unscaledTime + AudioSource.clip.length;

        SampleData = new float[256];

        Play = true;
    }

    public void Update()
    {
        if (!Play)
        {
            return;
        }

        if (Vector3.Distance(PlayerCamera.position, transform.position) > 10)
        {
            AudioSource.Stop();
            Head.SetBlendShapeWeight(0, 0);

            HeadBone.transform.localEulerAngles = new Vector3(0, 0, 0);

            Play = false;
            return;
        }

        Rotation = Quaternion.LookRotation(PlayerCamera.position - transform.position).eulerAngles.y;

        float yRot = transform.localEulerAngles.y;
        if (yRot - Rotation > 180)
        {
            yRot -= 360;
        }
        if (Rotation - yRot > 180)
        {
            yRot += 360;
        }
        if (Mathf.Abs(Rotation - yRot) > 30)
        {
            if (Rotation > yRot)
            {
                yRot = Rotation - 30;
            }
            else
            {
                yRot = Rotation + 30;
            }
            yRot = NormalizeRotation(yRot);
        }

        transform.eulerAngles = new Vector3(0, yRot, 0);
        HeadBone.transform.eulerAngles = new Vector3(0, Rotation, 0);

        ClipTime -= Time.unscaledDeltaTime;
        if (ClipTime <= 0)
        {
            ClipTime = 0.05f;

            AudioSource.clip.GetData(SampleData, AudioSource.timeSamples);
            TargetLipsing = 0f;
            foreach (var sample in SampleData)
            {
                TargetLipsing += Mathf.Abs(sample);
            }

            TargetLipsing = (TargetLipsing / 256 * 1000);
            if (TargetLipsing < 10)
            {
                TargetLipsing = 0;
            }
        }

        if (CurrentLipsing != TargetLipsing)
        {
            if (CurrentLipsing > TargetLipsing)
            {
                CurrentLipsing += -1500 * Time.deltaTime;
                if (CurrentLipsing <= TargetLipsing)
                {
                    CurrentLipsing = TargetLipsing;
                }
            }
            else
            {
                CurrentLipsing += 1500 * Time.deltaTime;
                if (CurrentLipsing >= TargetLipsing)
                {
                    CurrentLipsing = TargetLipsing;
                }
            }
        }

        Head.SetBlendShapeWeight(0, CurrentLipsing);

        if (Time.unscaledTime > EndTime)
        {
            AudioSource.Stop();
            Head.SetBlendShapeWeight(0, 0);

            HeadBone.transform.localEulerAngles = new Vector3(0, 0, 0);

            Play = false;
        }
    }

    private float NormalizeRotation(float value)
    {
        value %= 360;
        if (value < 0)
        {
            value += 360;
        }
        value %= 360;

        return value;
    }
}
