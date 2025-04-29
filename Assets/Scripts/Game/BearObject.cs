using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BearObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Bear Bear;
    [SerializeField] private Outline Indicator;
    [SerializeField] private NavMeshAgent NavMesh;
    [SerializeField] private BearObjectTask Task;
    [SerializeField] private Animator Animator;
    [SerializeField] private BearVisual Visual;
    [SerializeField] private AudioSource AudioSource;

    [SerializeField] private LayerMask GroundMask;

    public Bear _Bear => Bear;
    public BearObjectTask _Task => Task;
    public BearVisual _Visual => Visual;
    public NavMeshAgent _NavMeshAgent => NavMesh;

    public string _Info => "Поговорить";
    public bool _AbstractUse => true;

    private void OnDestroy()
    {
        Bear._BearObject = null;
    }
    public void SetBear(Bear bear)
    {
        Bear = bear;
        Animator.SetBool("Bad", Bear._Health <= 3);
    }
    public void CheckHealth()
    {
        if(Bear._Nedeesposoben)
        {
            Bear._BearObject = null;
            Destroy(gameObject);
        }
        Animator.SetBool("Bad", Bear._Health <= 3);
    }
    public void SetTask(BearObjectTask task)
    {
        if(Task != null)
        {
            Task.Stop();
        }

        Task = task;

        if(Task != null)
        {
            Task.Start();
        }
    }

    public void Indicate(bool state)
    {
        if (Indicator != null)
        {
            Indicator.enabled = state;
        }
        else
        {
            Destroy(this);
        }

    }
    public void Interact()
    {
        if(!StaticTools.Contains(City._DataBase._Bears, Bear))
        {
            return;
        }

        if (CameraChanger.Instance._MapCamera)
        {
            WindowCreator.CreateWindow<BearWindow>().SetInfo(Bear);
        }
        else if(Bear._Health > 3 && Bear._Stress < 50)
        {
            if(!(Task is TalkTask))
            {
                SetTask(new TalkTask(this, AudioSource, Task));
            }
        }
    }

    private void Update()
    {
        Task?.Tick();
    }

    [System.Serializable]
    public class BearObjectTask
    {
        [SerializeField] protected string Name;
        protected BearObject BearObject;

        public virtual void Start() { }

        public virtual void Tick() { }

        public virtual void Stop() { }
    }

    public class TalkTask : BearObjectTask
    {
        private BearObjectTask Previous;
        private AudioSource AudioSource;
        private SkinnedMeshRenderer Head;
        private float Rotation = 0;
        private float EndTime = 0;

        private float TargetLipsing = 0;
        private float CurrentLipsing = 0;
        private float[] SampleData;
        private float ClipTime = 0.05f;

        public TalkTask(BearObject bearObject, AudioSource voice, BearObjectTask previous)
        {
            BearObject =    bearObject;
            Previous = previous;
            AudioSource = voice;
            Head = bearObject._Visual._Head;
        }

        public override void Start()
        {
           Rotation = BearObject.transform.eulerAngles.y;

            switch (BearObject.Bear._Kasta)
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
        }

        public override void Tick()
        {
            if (Vector3.Distance(CameraChanger.Instance._Camera.transform.position, BearObject.transform.position) > 10)
            {
                AudioSource.Stop();
                BearObject.SetTask(Previous);
                Head.SetBlendShapeWeight(0, 0);

                BearObject._Visual._HeadBone.transform.localEulerAngles = new Vector3(0, 0, 0);
                return;
            }

            Rotation = Quaternion.LookRotation(CameraChanger.Instance._Camera.transform.position - BearObject.transform.position).eulerAngles.y;

            float yRot = BearObject.transform.localEulerAngles.y;
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

            BearObject.transform.eulerAngles = new Vector3(0, yRot, 0);
            BearObject._Visual._HeadBone.transform.eulerAngles = new Vector3(0, Rotation, 0);

            ClipTime -= Time.unscaledDeltaTime;
            if(ClipTime <= 0)
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
                BearObject.SetTask(Previous);
                Head.SetBlendShapeWeight(0, 0);

                BearObject._Visual._HeadBone.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        public override void Stop()
        {
            AudioSource.Stop();
            Head.SetBlendShapeWeight(0, 0);
            BearObject._Visual._HeadBone.transform.localEulerAngles = new Vector3(0, 0, 0);
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
    
    public class SallyTask : BearObjectTask
    {
        private CitySally.Sally Sally;
        private Vector3 Point;
        private float IdleTime = 0;
        private bool Stopped = false;

        private GameObject Snowrunner = null;

        private Vector2Int LastDirection = Vector2Int.zero;

        public SallyTask(BearObject bearObject, string name, CitySally.Sally sally)
        {
            BearObject = bearObject;
            Name = name;
            Sally = sally;

            Sally.OnChanges += CheckSally;
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 position;

            do
            {
                position = Sally.MeetPoint + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
                if (Physics.Raycast(position + Vector3.up * 50, Vector3.down, out RaycastHit hit, 200, BearObject.GroundMask))
                {
                    position = hit.point;
                }
            }
            while (Physics.CheckSphere(position, 0.1f, 256));

            return position;
        }
        private Vector3 GetSallyPosition()
        {
            Vector3 position;

            if (Sally._Path == null || Sally._Path.Length <= 0)
            {
                return GetRandomPosition();
            }

            position =Vector3.zero;
            if(Sally._Path[0].x != 0)
            {
                position.x = 80 * Sally._Path[0].x;
            }
            else
            {
                position.x = Random.Range(-5, 5f);
            }
            if (Sally._Path[0].y != 0)
            {
                position.z = 80 * Sally._Path[0].y;
            }
            else
            {
                position.z = Random.Range(-5, 5f);
            }
            if (Physics.Raycast(position + Vector3.up * 50, Vector3.down, out RaycastHit hit, 200, BearObject.GroundMask))
            {
                position = hit.point;
            }

            return position;
        }

        public void CheckSally()
        {
            if(BearObject == null || BearObject.NavMesh == null)
            {
                return;
            }

            if (Sally._AllowMove)
            {
                if(Sally._Path == null || Sally._Path.Length == 0)
                {
                    return;
                }

                if (LastDirection == Sally._Path[0])
                {
                    return;
                }

                LastDirection = Sally._Path[0];

                if (Snowrunner != null)
                {
                    BearObject.NavMesh.speed = Random.Range(10, 12f);
                }
                else
                {
                    BearObject.NavMesh.speed = BearObject.Bear._Health <= 3 ? 2 : Random.Range(3.5f, 5);
                }

                Point = GetSallyPosition();
                BearObject.NavMesh.destination = Point;
                Stopped = false;
                BearObject.Animator.SetInteger("state", 1);
            }
            else if (Mathf.FloorToInt(Sally._Position.x) == CitySally.TownPoint.x && Mathf.FloorToInt(Sally._Position.y) == CitySally.TownPoint.y)
            {
                LastDirection = Vector2Int.zero;

                IdleTime = Random.Range(5, 30f);
                if (Snowrunner != null)
                {
                    BearObject.NavMesh.speed = Random.Range(10, 12f);
                }
                else
                {
                    BearObject.NavMesh.speed = BearObject.Bear._Health <= 3 ? 1 : Random.Range(2f, 3);
                }
                Point = GetRandomPosition();
                BearObject.NavMesh.destination = Point;
                Stopped = false;
                BearObject.Animator.SetInteger("state", 1);
            }
        }

        public override void Start()
        {
            CheckSally();
        }

        public override void Tick()
        {
            if (LastDirection != Vector2Int.zero)
            {
                if (Vector3.Distance(BearObject.transform.position, Point) < 0.5f)
                {
                    BearObject.Animator.SetInteger("state", 0);
                    BearObject.SetTask(null);
                    Destroy(BearObject.gameObject);
                }
                return;
            }

            if (BearObjectioner._Instance != null)
            {
                if (Sally._Snowrunner)
                {
                    if (Snowrunner == null)
                    {
                        Snowrunner = Instantiate(BearObjectioner._Instance._SnowrunnerPrefab, BearObject.transform);
                        BearObject.Animator.Play("Ride");
                        BearObject.NavMesh.speed = Random.Range(10, 12f);
                    }
                }
                else
                {
                    if (Snowrunner != null)
                    {
                        Destroy(Snowrunner);
                        Snowrunner = null;
                        BearObject.Animator.Play("Idle");
                        BearObject.NavMesh.speed = BearObject.Bear._Health <= 3 ? 1 : Random.Range(2f, 3);
                    }
                }
            }

            if (Stopped)
            {
                IdleTime -= Time.deltaTime;
                if (IdleTime < 0)
                {
                    IdleTime = Random.Range(5, 30f);
                    if (Sally._Snowrunner)
                    {
                        BearObject.NavMesh.speed = Random.Range(10, 12f);
                    }
                    else
                    {
                        BearObject.NavMesh.speed = BearObject.Bear._Health <= 3 ? 1 : Random.Range(2f, 3);
                    }
                    Point = GetRandomPosition();
                    BearObject.NavMesh.destination = Point;
                    Stopped = false;
                    BearObject.Animator.SetInteger("state", 1);
                }
            }
            else
            {
                if (Vector3.Distance(BearObject.transform.position, Point) < 0.5f)
                {
                    Stopped = true;
                    BearObject.Animator.SetInteger("state", 0);
                }
            }
        }

        public override void Stop()
        {
            BearObject.NavMesh.velocity = Vector3.zero;
            BearObject.NavMesh.destination = BearObject.transform.position;
            BearObject.Animator.SetInteger("state", 0);

            if(Snowrunner != null)
            {
                Destroy(Snowrunner);
                BearObject.Animator.Play("Idle");
            }

            if(Sally != null)
            {
                Sally.OnChanges -= CheckSally;
            }
        }
    }

    public class RandomWalkTask : BearObjectTask
    {
        private Vector3 Point;
        private float IdleTime = 0;
        private bool Stopped = true;

        public RandomWalkTask(BearObject bearObject, string name, float idleTime = 0)
        {
            BearObject = bearObject;
            IdleTime = idleTime;
            Name = name;
        }

        public override void Start()
        {

        }

        public override void Tick()
        {
            if (Stopped)
            {
                IdleTime -= Time.deltaTime;
                if (IdleTime < 0)
                {
                    IdleTime = Random.Range(5, 30f);
                    BearObject.NavMesh.speed = BearObject.Bear._Health <= 3 ? 1 : Random.Range(1.5f, 2.2f);
                    Point = Bear.GetRandomPosition();
                    BearObject.NavMesh.destination = Point;
                    Stopped = false;
                    BearObject.Animator.SetInteger("state", 1);
                }
            }
            else
            {
                if (Vector3.Distance(BearObject.transform.position, Point) < 0.5f)
                {
                    Stopped = true;
                    BearObject.Animator.SetInteger("state", 0);
                }
            }
        }

        public override void Stop()
        {
            BearObject.NavMesh.velocity = Vector3.zero;
            BearObject.NavMesh.destination = BearObject.transform.position;
            BearObject.Animator.SetInteger("state", 0);
        }
    }

    public class ChillInCapsuleTask : BearObjectTask
    {
        public ChillInCapsuleTask(BearObject bearObject)
        {
            BearObject = bearObject;
        }

        public override void Start()
        {
            BearObject.Animator.Play("InCapsule");
            BearObject.Animator.SetBool("InCapsule", true);
            BearObject._Visual._ClosedEyes = true;
        }

        public override void Stop()
        {
            BearObject.Animator.SetBool("InCapsule", false);
            BearObject._Visual._ClosedEyes = false;
        }
    }

    public class DestinateTask : BearObjectTask
    {
        protected Facility Destination;

        private bool Moved = false;

        public DestinateTask(BearObject bearObject, string name, Facility destination)
        {
            BearObject = bearObject;
            Name = name;
            Destination = destination;
        }

        public override void Start()
        {
            if(Destination == null)
            {
                BearObject.SetTask(null);
            }
            else
            {
                BearObject.NavMesh.destination = Destination._EnterPoint.position;
            }
        }

        public override void Tick()
        {
            if (!Moved)
            {
                if (!BearObject.NavMesh.pathPending)
                {
                    Moved = true;
                }
                return;
            }

            BearObject.Animator.SetInteger("state", 1);

            if (Vector3.Distance(BearObject.transform.position, Destination._EnterPoint.position) < 0.5f)
            {
                BearObject.Animator.SetInteger("state", 0);
                BearObject.SetTask(null);
                Destroy(BearObject.gameObject);
            }
        }

        public override void Stop()
        {
            BearObject.NavMesh.velocity = Vector3.zero;
            BearObject.NavMesh.destination = BearObject.transform.position;
            BearObject.Animator.SetInteger("state", 0);
        }
    }
}
