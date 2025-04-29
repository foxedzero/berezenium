using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Constructor : MonoBehaviour, ICancelable
{
    public enum ConstructCategory { Электроэнергия, Жилище, Производство, Добыча, Пища, Медицина, Прочее }

    [SerializeField] private GameObject DemolishEffect;
    [SerializeField] private GameObject BuildEffect;

    [SerializeField] private ConstructInfo ConstructProject;
    [SerializeField] private ConstructInfo[] Constructions = new ConstructInfo[0];
    [SerializeField] private Transform Map;

    [SerializeField] private LayerMask ConstructionMask;
    [SerializeField] private LayerMask TreeMask;
    [SerializeField] private Transform PlaceMarker;
    [SerializeField] private MeshRenderer Renderer;
    [SerializeField] private Material MarkerMaterial;
    [SerializeField] private Grid Grid;
    [SerializeField] private GameObject GridObject;

    private ConstructInfo Constructing = null;
    private Vector3Int Position = new Vector3Int(0, 0);
    private int Direction = 0;
    private bool PlaceLocked = false;
    private UserConfirm Ask = null;

    private int MapSize = 0;

    private ResourceField ResourceField = null;

    public ConstructInfo[] _Constructions
    {
        get
        {
            return Constructions;
        }
    }
    public ConstructInfo _Constructing
    {
        get
        {
            return Constructing;
        }
        set
        {
            Constructing = value;

            if(value != null && value.Name == "Барак")
            {
                NewTutorialSystem.Instance.BarakSelected();
            }

            Direction = 0;

            if (Constructing != null)
            {
                UnlockPlacer();

                if(PlaceMarker != null)
                {
                    Destroy(PlaceMarker.gameObject);
                }

                PlaceMarker = Instantiate(Constructing.Preview).transform;
                GridObject.SetActive(true);

                CancelQueue.Register(this, false);
            }
            else
            {
                if (PlaceMarker != null)
                {
                    Destroy(PlaceMarker.gameObject);
                }

                GridObject.SetActive(false);

                CancelQueue.Register(this, true);
            }
        }
    }

    private void Start()
    {
        switch (SaveManager._Instance._SaveData.MapSize)
        {
            case 0:
                MapSize = 150;
                break;
            case 1:
                MapSize = 175;
                break;
            case 2:
                MapSize = 200;
                break;
        }
    }

    private void OnDisable()
    {
        _Constructing = null;
    }

    private void Update()
    {
        if(Constructing == null)
        {
            return;
        }

        Vector3Int position = Grid._Point;

        if (Physics.BoxCast(position + Vector3.up * 50, new Vector3(Constructing.Sizes.x/2, 0.01f, Constructing.Sizes.y/2), Vector3.down, out RaycastHit hit22, Quaternion.Euler(0, 90 * Direction, 0), 100, 128))
        {
            position.y = Mathf.RoundToInt( hit22.point.y);
        }

        if (!PlaceLocked)
        {
            PlaceMarker.position = Grid._Point;
            PlaceMarker.localEulerAngles = new Vector3(0, 90 * Direction, 0);
        }

        if (Constructing.Category == ConstructCategory.Добыча && Constructing.MiningResource != CityStorage.ResourceType.Wood)
        {
            if (!PlaceLocked)
            {
                PlaceMarker.position = position;
                PlaceMarker.localEulerAngles = new Vector3(0, 90 * Direction, 0);
            }

            Collider[] colliders = Physics.OverlapBox(position, new Vector3(Constructing.Sizes.x / 2f, 40, Constructing.Sizes.y / 2f), Quaternion.Euler(0, 90 * Direction, 0), ConstructionMask);
            bool finded = false;
            foreach (Collider collider in colliders)
            {
                ResourceField field = collider.GetComponentInParent<ResourceField>();
                if (field != null && field._ResourceType == Constructing.MiningResource)
                {
                    finded = true;
                    MarkerMaterial.color = new Color(0.4074746f, 0.3999999f, 0.8f, 0.85f);
                    PlaceMarker.position = field.transform.position;
                    PlaceMarker.rotation = field.transform.rotation;

                    if (CheckClick() && Ask == null)
                    {
                        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Interact))
                        {
                            ResourceField = field;
                            Position = new Vector3Int(Mathf.FloorToInt(field.transform.position.x), 0, Mathf.FloorToInt(field.transform.position.z));
                            Direction = (int)ResourceField.transform.localEulerAngles.y / 90;

                            PlaceLocked = true;

                            Ask = UserInteract.AskConfirm("Новое здание!", $"Вы собираетесь построить \"{Constructing.Name}\" в точке x{Position.x} z{Position.z}", EndConstruct);
                            Ask.OnSkip = UnlockPlacer;
                        }
                    }
                    break;
                }
            }

            if (!finded)
            {
                MarkerMaterial.color = new Color(0.8f, 0, 0, 0.85f);
            }
        }
        else if(Grid._PointAtGrid)
        {
            if (InputManager.GetButtonDown(InputManager.ButtonEnum.Rotate))
            {
                Direction = (Direction + 1) % 4;
            }

            Vector4 endpoints;
            if (Direction % 2 != 0)
            {
                endpoints = new Vector4(Grid._Point.x + Constructing.Sizes.y / 2f, Grid._Point.x - Constructing.Sizes.y / 2f , Grid._Point.z + Constructing.Sizes.x / 2f , Grid._Point.z - Constructing.Sizes.x / 2f );
            }
            else
            {
                endpoints = new Vector4(Grid._Point.x + Constructing.Sizes.x / 2f, Grid._Point.x - Constructing.Sizes.x / 2f , Grid._Point.z + Constructing.Sizes.y / 2f, Grid._Point.z - Constructing.Sizes.y / 2f + 1);
            }

            if (Physics.CheckBox(Grid._Point, new Vector3(Constructing.Sizes.x / 2f + 1.25f, 40, Constructing.Sizes.y / 2f + 1.25f), Quaternion.Euler(0, 90 * Direction, 0), ConstructionMask))
            {
                MarkerMaterial.color = new Color(0.8f, 0, 0, 0.85f);
            }
            else if(endpoints.x > MapSize / 2f || endpoints.y < -MapSize / 2f || endpoints.z > MapSize / 2f || endpoints.w < -MapSize / 2f)
            {
                MarkerMaterial.color = new Color(0.8f, 0, 0, 0.85f);
            }
            else
            {
                MarkerMaterial.color = new Color(0.4074746f, 0.3999999f, 0.8f, 0.85f);

                if (CheckClick() && Ask == null)
                {
                    if (InputManager.GetButtonDown(InputManager.ButtonEnum.Interact))
                    {
                        Position = position;

                        PlaceLocked = true;

                        Ask = UserInteract.AskConfirm("Новое здание!", $"Вы собираетесь построить \"{Constructing.Name}\" в точке x{Position.x} z{Position.z}", EndConstruct);
                        Ask.OnSkip = UnlockPlacer;
                    }
                }
            }
        }
    }

    public void Cancel()
    {
        _Constructing = null;
    }

    public void UnlockPlacer()
    {
        PlaceLocked = false;
    }

    public void EndConstruct(bool state)
    {
        if(Constructing == null)
        {
            return;
        }

        if (state)
        {
            if (City._Storage._Wood >= Constructing.WoodCost && City._Storage._Metal >= Constructing.MetalCost && City._Storage._Berezenium >= Constructing.BerezenuimCost)
            {
                if (Constructing.WoodCost > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"проект {Constructing.Name} #{City._DataBase._Facilities.Length}", (int)(City._Time._WorldTime / 60), -Constructing.WoodCost, CityStorage.ResourceType.Wood));
                    City._Storage._Wood -= Constructing.WoodCost;
                }
                if (Constructing.MetalCost > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"проект {Constructing.Name} #{City._DataBase._Facilities.Length}", (int)(City._Time._WorldTime / 60), -Constructing.MetalCost, CityStorage.ResourceType.Metal));
                    City._Storage._Metal -= Constructing.MetalCost;
                }
                if (Constructing.BerezenuimCost > 0)
                {
                    City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"проект {Constructing.Name} #{City._DataBase._Facilities.Length}", (int)(City._Time._WorldTime / 60), -Constructing.BerezenuimCost, CityStorage.ResourceType.Berezenium));
                    City._Storage._Berezenium -= Constructing.BerezenuimCost;
                }

                if (Constructing.Category == ConstructCategory.Добыча)
                {
                    (Build(ConstructProject, Position, Direction) as ConstructionProject).SetInfo(Constructing, ResourceField);
                }
                else
                {
                    (Build(ConstructProject, Position, Direction) as ConstructionProject).SetInfo(Constructing);
                }

                if (Constructing.Name == "Барак")
                {
                    NewTutorialSystem.Instance.BarakSetuped();
                }

                Instantiate(BuildEffect, Position + Vector3Int.up * 6, Quaternion.Euler(-90, 0, 0));
                _Constructing = null;
            }
            else
            {
                UserInteract.AskMessage("Недостаточно ресурсов", $"В хранилище не хватает ресурсов для постройки данного строения.{(City._Storage._Wood < Constructing.WoodCost ? $"\nНехваток древесины: {Constructing.WoodCost - City._Storage._Wood}" : $"")}{(City._Storage._Metal < Constructing.MetalCost ? $"\nНехваток металла: {Constructing.MetalCost - City._Storage._Metal}" : $"")}{(City._Storage._Berezenium < Constructing.BerezenuimCost ? $"\nНехваток безениума: {Constructing.BerezenuimCost - City._Storage._Berezenium}" : $"")}");
            }
        }

        UnlockPlacer();
    }

    private bool CheckClick()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>(0);
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count == 0)
        {
            return true;
        }

        return false;
    }

    public void SpawnDemolishEffect(Vector3 position)
    {
        Instantiate(DemolishEffect, position, transform.rotation);
    }

    public Facility Build(ConstructInfo info, Vector3Int position, int direction, ResourceField field = null)
    {
        Facility facility = Instantiate(info.Prefab, Map).GetComponent<Facility>();

        if(facility is ResourceAssimilator)
        {
            (facility as ResourceAssimilator).SetField(field);
        }

        facility.transform.position = position;

        facility.transform.localEulerAngles = new Vector3(0, direction * 90, 0);

        facility._ConstructInfo = info;

        City._DataBase.RegisterFacility(facility, false);

        facility.AutoAssign();

        return facility;
    }

    public Facility Load(string data)
    {
        Dictionary<string, string> parameters = StaticTools.GetParameters(data);

        string[] transform = parameters["Transform"].Split(";");

        if (parameters["Type"] == "ConstructProject")
        {
            string construct = parameters["Construct"];

            ConstructionProject facility = Instantiate(ConstructProject.Prefab, Map).GetComponent<ConstructionProject>();

            facility.transform.localEulerAngles = new Vector3(0, 90 * int.Parse(transform[2]), 0);
            facility._ConstructInfo = ConstructProject;

            foreach (ConstructInfo consturct in Constructions)
            {
                if (consturct.Prefab.name == construct)
                {
                    facility.SetInfo(consturct);
                    break;
                }
            }

            Vector3Int position = new Vector3Int(int.Parse(transform[0]), 0, int.Parse(transform[1]));
            if (Physics.BoxCast(position + Vector3.up * 50, new Vector3(facility._Construction.Sizes.x / 2, 0.01f, facility._Construction.Sizes.y / 2), Vector3.down, out RaycastHit hit22, facility.transform.rotation, 100, 128))
            {
                position.y = Mathf.RoundToInt(hit22.point.y);
            }

            facility.transform.position = position;

            facility._SaveInfo = data;

            return facility;
        }
        else
        {
            foreach (ConstructInfo consturct in Constructions)
            {
                if (consturct.Prefab.name == parameters["Type"])
                {
                    Facility facility = Instantiate(consturct.Prefab, Map).GetComponent<Facility>();
                    facility.transform.localEulerAngles = new Vector3(0, 90 * int.Parse(transform[2]), 0);
                    facility._ConstructInfo = consturct;
                    facility._SaveInfo = data;

                    Vector3Int position = new Vector3Int(int.Parse(transform[0]), 0, int.Parse(transform[1]));
                    if (Physics.BoxCast(position + Vector3.up * 50, new Vector3(consturct.Sizes.x / 2, 0.01f, consturct.Sizes.y / 2), Vector3.down, out RaycastHit hit22, facility.transform.rotation, 100, 128))
                    {
                        position.y = Mathf.RoundToInt(hit22.point.y);
                    }

                    facility.transform.position = position;

                    return facility;
                }
            }
        }

        return null;
    }

    [System.Serializable]
    public class ConstructInfo
    {
        public ConstructCategory Category;

        public CityStorage.ResourceType MiningResource;

        public string Name;
        [Multiline] public string Description;
        public Vector2 Sizes;
        public Sprite Icon;

        public CityResearch.ResearchType Research;
        public int ResearchLevel;

        public float BuildWork;
        public float WoodCost;
        public float MetalCost;
        public float BerezenuimCost;

        public GameObject Prefab;
        public GameObject Preview;
    }
}
