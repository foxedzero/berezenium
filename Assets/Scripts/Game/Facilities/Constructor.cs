using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Constructor : MonoBehaviour
{
    public enum ConstructCategory { Электроэнергия, Жилище, Производство, Добыча, Пища, Медицина, Прочее }

    [SerializeField] private GameObject DemolishEffect;
    [SerializeField] private GameObject BuildEffect;

    [SerializeField] private MeshRenderer GridRenderer;

    [SerializeField] private ConstructInfo ConstructProject;
    [SerializeField] private ConstructInfo[] Constructions = new ConstructInfo[0];
    [SerializeField] private Transform Map;

    [SerializeField] private LayerMask ConstructionMask;
    [SerializeField] private LayerMask TreeMask;
    [SerializeField] private GameObject PlaceMarker;
    [SerializeField] private MeshRenderer Renderer;
    [SerializeField] private Material[] MarkerMaterials;
    [SerializeField] private Grid Grid;

    private ConstructInfo Constructing = null;
    private Vector3Int Position = new Vector3Int(0, 0);
    private int Direction = 0;
    private bool PlaceLocked = false;
    private UserConfirm Ask = null;

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

            Direction = 0;

            if (Constructing != null)
            {
                UnlockPlacer();
                GridRenderer.enabled = true;
                PlaceMarker.SetActive(true);
                PlaceMarker.transform.localScale = new Vector3(Constructing.Sizes.x, 0.2f, Constructing.Sizes.y);
                PlaceMarker.transform.localEulerAngles = new Vector3(0, 90 * Direction, 0);
            }
            else
            {
                GridRenderer.enabled = false;
                PlaceMarker.SetActive(false);
            }
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

        if(InputManager.GetButtonDown(InputManager.ButtonEnum.Cancel) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            _Constructing = null;
            return;
        }

        if (!PlaceLocked)
        {
            PlaceMarker.transform.position = Grid._Point;
            PlaceMarker.transform.localEulerAngles = new Vector3(0, 90 * Direction, 0);
        }

        if (Constructing.Category == ConstructCategory.Добыча && Constructing.MiningResource != CityStorage.ResourceType.Wood)
        {
            if (!PlaceLocked)
            {
                PlaceMarker.transform.position = Grid._Point;
                PlaceMarker.transform.localEulerAngles = new Vector3(0, 90 * Direction, 0);
            }

            Collider[] colliders = Physics.OverlapBox(Grid._Point, new Vector3(Constructing.Sizes.x / 2f, 40, Constructing.Sizes.y / 2f), Quaternion.Euler(0, 90 * Direction, 0), ConstructionMask);
            bool finded = false;
            foreach (Collider collider in colliders)
            {
                ResourceField field = collider.GetComponentInParent<ResourceField>();
                if (field != null && field._ResourceType == Constructing.MiningResource)
                {
                    finded = true;
                    Renderer.material = MarkerMaterials[0];
                    PlaceMarker.transform.position = field.transform.position;
                    PlaceMarker.transform.rotation = field.transform.rotation;

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
                Renderer.material = MarkerMaterials[1];
            }
        }
        else
            if (Grid._PointAtGrid)
        {
            if (InputManager.GetButtonDown(InputManager.ButtonEnum.Rotate))
            {
                Direction = (Direction + 1) % 4;
            }

            if (Physics.CheckBox(Grid._Point, new Vector3(Constructing.Sizes.x / 2f, 40, Constructing.Sizes.y / 2f), Quaternion.Euler(0, 90 * Direction, 0), ConstructionMask))
            {
                Renderer.material = MarkerMaterials[1];
            }
            else
            {
                Renderer.material = MarkerMaterials[0];

                if (CheckClick() && Ask == null)
                {
                    if (InputManager.GetButtonDown(InputManager.ButtonEnum.Interact))
                    {
                        Position = Grid._Point;

                        PlaceLocked = true;

                        Ask = UserInteract.AskConfirm("Новое здание!", $"Вы собираетесь построить \"{Constructing.Name}\" в точке x{Position.x} z{Position.z}", EndConstruct);
                        Ask.OnSkip = UnlockPlacer;
                    }
                }
            }
        }
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
                City._Storage._Wood -= Constructing.WoodCost;
                City._Storage._Metal -= Constructing.MetalCost;
                City._Storage._Berezenium -= Constructing.BerezenuimCost;

                if (Constructing.Category == ConstructCategory.Добыча)
                {
                    (Build(ConstructProject, Position, Direction) as ConstructionProject).SetInfo(Constructing, ResourceField);
                }
                else
                {
                    (Build(ConstructProject, Position, Direction) as ConstructionProject).SetInfo(Constructing);
                }

                foreach (Collider collider in Physics.OverlapBox(Position, new Vector3(Constructing.Sizes[0] / 2, 5, Constructing.Sizes[1] / 2), Quaternion.Euler(0, Direction * 90, 0), TreeMask))
                {
                    Destroy(collider.gameObject);
                }

                Instantiate(BuildEffect, Position + Vector3Int.up * 6, Quaternion.EulerAngles(-90, 0, 0));

                City._CityMessenger.SetMessage(new CityMessenger.CityMessage("Проект начат", $"Здание \"{Constructing.Name}\" на x{Position.x} z{Position.z} начало строиться.\nПожалуйста назначте конструкторов для постройки.\nПри нормальных условиях, здание будет готово через {Constructing.BuildWork} дней"), false);
                _Constructing = null;
            }
            else
            {
                UserInteract.AskMessage("Недостаточно ресурсов", $"В хранилище не хватает ресурсов для постройки данного строения.{(City._Storage._Wood < Constructing.WoodCost ? $"\nНехваток древесины: {Constructing.WoodCost - City._Storage._Wood}" : $"")}{(City._Storage._Metal < Constructing.MetalCost ? $"\nНехваток металла: {Constructing.MetalCost - City._Storage._Metal}" : $"")}{(City._Storage._Berezenium < Constructing.BerezenuimCost ? $"\nНехваток безениума: {Constructing.BerezenuimCost - City._Storage._Berezenium}" : $"")}");
            }
        }

        UnlockPlacer();
    }

    public void BuildWorkDayPassed()
    {
        foreach(Facility facility in City._DataBase._Facilities)
        {
            if(facility is ConstructionProject)
            {
                (facility as ConstructionProject).DayPassed();
            }
        }
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

        if(facility is ExsaustProducer)
        {
            (facility as ExsaustProducer).SetField(field);
        }

        facility.transform.position = position;

        facility.transform.localEulerAngles = new Vector3(0, direction * 90, 0);

        facility._ConstructInfo = info;

        foreach(Collider collider in Physics.OverlapBox(facility.transform.position, new Vector3(info.Sizes[0]/ 2, 5, info.Sizes[1] / 2), Quaternion.Euler(0, direction * 90, 0), TreeMask))
        {
            Destroy(collider.gameObject);
        }

        City._DataBase.RegisterFacility(facility, false);

        return facility;
    }

    public Facility Load(string data)
    {
        string type = StaticTools.GetParameter(data, "Type");
        string[] transform = StaticTools.GetParameter(data, "Transform").Split(";");

        if(type == "ConstructProject")
        {
            string construct = StaticTools.GetParameter(data, "Construct");

            ConstructionProject facility = Instantiate(ConstructProject.Prefab, Map).GetComponent<ConstructionProject>();
            facility.transform.position = new Vector3(int.Parse(transform[0]), 0, int.Parse(transform[1]));
            facility.transform.localEulerAngles = new Vector3(0, 90 * int.Parse(transform[2]), 0);
            facility._ConstructInfo = ConstructProject;

            foreach (ConstructInfo consturct in Constructions)
            {
                if (consturct.Prefab.name == construct)
                {
                    facility.SetInfo(consturct);

                    foreach (Collider collider in Physics.OverlapBox(facility.transform.position, new Vector3(consturct.Sizes[0] / 2, 5, consturct.Sizes[1] / 2), Quaternion.Euler(0, Direction * 90, 0), TreeMask))
                    {
                        Destroy(collider.gameObject);
                    }

                    break;
                }
            }

            facility._SaveInfo = data;

            return facility;
        }
        else
        {
            foreach (ConstructInfo consturct in Constructions)
            {
                if (consturct.Prefab.name == type)
                {
                    Facility facility = Instantiate(consturct.Prefab, Map).GetComponent<Facility>();
                    facility.transform.position = new Vector3(int.Parse(transform[0]), 0, int.Parse(transform[1]));
                    facility.transform.localEulerAngles = new Vector3(0, 90 * int.Parse(transform[2]), 0);
                    facility._ConstructInfo = consturct;
                    facility._SaveInfo = data;

                    foreach (Collider collider in Physics.OverlapBox(facility.transform.position, new Vector3(consturct.Sizes[0] / 2, 5, consturct.Sizes[1] / 2), Quaternion.Euler(0, Direction * 90, 0), TreeMask))
                    {
                        Destroy(collider.gameObject);
                    }

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

        public float BuildSkill;
        public float BuildWork;
        public float WoodCost;
        public float MetalCost;
        public float BerezenuimCost;

        public GameObject Prefab;
    }
}
