
using UnityEngine;
using UnityEngine.UI;
using static CitySally;

public class SallyPanel : MonoBehaviour, ICancelable
{
    private NeedCursorOrder NeedCursorOrder = new NeedCursorOrder();

    [SerializeField] private GameObject SallyHolderPrefab;
    [SerializeField] private RectTransform SallyContent;
    [SerializeField] private SallyHolder[] SallyHolders = new SallyHolder[0];

    [SerializeField] private SallyMap SallyMap;
    [SerializeField] private Text TilePoint;
    [SerializeField] private Text TileCoef;
    [SerializeField] private Text PathLength;

    [SerializeField] private Text TimeInfo;
    [SerializeField] private Tipper TimeTip;
    [SerializeField] private TimeEditor TimeEditor;

    [SerializeField] private RectTransform CurrentTile;
    [SerializeField] private Animator CurentTileAnimator;

    [SerializeField] private GameObject SallyPointPrefab;
    [SerializeField] private GameObject DirectPrefab;
    [SerializeField] private GameObject EndPrefab;
    private GameObject[] PathTiles = new GameObject[0];
    private SallyPoint[] SallyPoints = new SallyPoint[0];

    [SerializeField] private GameObject TileContentPrefab;
    private SallyContentHolder[] TileContentTiles = new SallyContentHolder[0];

    [SerializeField] private Slider ScaleSlider;
    [SerializeField] private RectTransform Map;
    [SerializeField] private RectTransform Lower;
    [SerializeField] private RectTransform Upper;
    [SerializeField] private RectTransform TileContents;
    [SerializeField] private float Sensitivity;
    private float Scale = 0.5f;

    private Vector2Int SelectedEnd;
    private TileContent SelectedContent;
    private Sally SelectedSally;
    private SallyWindow Window = null;

    [SerializeField] private Vector2Int End = Vector2Int.one * 12;

    public Sally _Current => Window != null ? Window._Sally : null;

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        City._Time.HourPassed += UpdateSallyPositions;
        City._CitySally.OnSalliesUpdate += UpdateSallyPositions;
        City._CitySally.OnContentsUpdate += UpdateTileContents;
        CancelQueue.Register(this, false);
        CursorManager.SetNeedMouse(NeedCursorOrder, false);

        UpdateTileContents();
        UpdateSallyPositions();

        NewTutorialSystem.Instance.SalliesOpened();
    }
    private void OnDisable()
    {
        City._Time.HourPassed -= UpdateSallyPositions;
        City._CitySally.OnSalliesUpdate -= UpdateSallyPositions;
        City._CitySally.OnContentsUpdate -= UpdateTileContents;
        CancelQueue.Register(this, true);
        CursorManager.SetNeedMouse(NeedCursorOrder, true);
    }

    public void ShowWay(SquareAStar star)
    {
        foreach (GameObject tile in PathTiles)
        {
            Destroy(tile);
        }

        if (star == null || star._Path == null)
        {
            return;
        }

        Vector2Int position = star._Start;
        PathTiles = new GameObject[star._Path.Length + 1];
        for (int i = 0; i < star._Path.Length; i++)
        {
            PathTiles[i] = Instantiate(DirectPrefab, Lower);
            PathTiles[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(position.x * 100 + 50f, position.y * 100 + 50f, 0);
            PathTiles[i].transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(star._Path[i].y, star._Path[i].x) * Mathf.Rad2Deg);
            position += star._Path[i];
        }
        PathTiles[PathTiles.Length - 1] = Instantiate(EndPrefab, Lower);
        PathTiles[PathTiles.Length - 1].GetComponent<RectTransform>().anchoredPosition = new Vector3(position.x * 100 + 50f, position.y * 100 + 50f, 0);
    }

    private void UpdateSallyPositions()
    {
        int count = City._CitySally._Sallies.Length;
        if (SallyPoints.Length < count)
        {
            SallyPoint[] newPoint = new SallyPoint[count - SallyPoints.Length];
            for (int i = 0; i < count - SallyPoints.Length; i++)
            {
                SallyPoint point = Instantiate(SallyPointPrefab, Upper).GetComponent<SallyPoint>();
                newPoint[i] = point;
                point.SetInfo(this, City._CitySally._Sallies[SallyPoints.Length + i]);
            }

            SallyPoints = StaticTools.ExpandMassive(SallyPoints, newPoint);
        }
        else if (SallyPoints.Length > count)
        {
            while (SallyPoints.Length > count)
            {
                Destroy(SallyPoints[SallyPoints.Length - 1].gameObject);
                SallyPoints = StaticTools.ReduceMassive(SallyPoints, SallyPoints.Length - 1);
            }
        }

        for(int i = 0; i < City._CitySally._Sallies.Length; i++)
        {
            SallyPoints[i]._Sally = City._CitySally._Sallies[i];
        }

        if (SallyHolders.Length < count)
        {
            SallyHolder[] newHolders = new SallyHolder[count - SallyHolders.Length];
            for (int i = 0; i < count - SallyHolders.Length; i++)
            {
                SallyHolder holder = Instantiate(SallyHolderPrefab, SallyContent).GetComponent<SallyHolder>();
                newHolders[i] = holder;
                holder.SetInfo(this, -25 - 60 * (SallyHolders.Length + i));
            }

            SallyHolders = StaticTools.ExpandMassive(SallyHolders, newHolders);
        }
        else if (SallyHolders.Length > count)
        {
            while (SallyHolders.Length > count)
            {
                Destroy(SallyHolders[SallyHolders.Length - 1].gameObject);
                SallyHolders = StaticTools.ReduceMassive(SallyHolders, SallyHolders.Length - 1);
            }
        }

        for (int i = 0; i < City._CitySally._Sallies.Length; i++)
        {
            SallyHolders[i]._Sally = City._CitySally._Sallies[i];
        }

        SallyContent.sizeDelta = new Vector2(0, 50 + SallyHolders.Length * 60);
    }

    private void UpdateTileContents()
    {
        foreach(SallyContentHolder content in TileContentTiles)
        {
            Destroy(content.gameObject);
        }

        TileContentTiles = new SallyContentHolder[0];
        foreach (TileContent tile in City._CitySally._Contents)
        {
            if(tile.State == 1)
            {
                SallyContentHolder content = Instantiate(TileContentPrefab, TileContents).GetComponent<SallyContentHolder>();
                content.SetInfo(tile, this);
                TileContentTiles = StaticTools.ExpandMassive(TileContentTiles, content);
            }
        }
    }

    private void Update()
    {
        if(Input.mouseScrollDelta.y > 0)
        {
            ScaleSlider.value = Scale + Sensitivity;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            ScaleSlider.value = Scale - Sensitivity;
        }

        int time = (int)(City._Time._WorldTime % 1500);

        int h = time / 60;
        int m = time % 60;

        TimeInfo.text = $"{(int)(City._Time._WorldTime / 1600 + 1)} день  {(h < 10 ? "0" : "")}{h}:{(m < 10 ? "0" : "")}{m}"; 
        TimeTip._Info = $"Сейчас {(h < 10 ? "0" : "")}{h}:{(m < 10 ? "0" : "")}{m}\n\nМультипликатор скорости времени: {Mathf.Pow(2, TimeEditor._TimeIndex)}X\n\nДней вашего управления: {(int)(City._Time._WorldTime / 1500) + 1}";

        if (SallyMap._MouseCaptured)
        {
            CurrentTile.gameObject.SetActive(true);

            if (Window != null)
            {
                Vector2 mousePosition = SallyMap.MouseToInsidePosition();
                Vector2Int end = new Vector2Int(Mathf.RoundToInt(mousePosition.x / 100f) + 12, Mathf.RoundToInt(mousePosition.y / 100f) + 12);

                if(end != End)
                {
                    End = end;

                    SquareAStar star = new SquareAStar(City._CitySally._Map, new Vector2Int(Mathf.FloorToInt(Window._Sally._Position.x), Mathf.FloorToInt(Window._Sally._Position.y)), end);
                    CurrentTile.anchoredPosition = end * 100 + Vector2Int.one * 50;
                    CurentTileAnimator.Play("PointedTile");

                    TilePoint.text = $"Координаты: {end}";
                    TileCoef.text = $"Проходимость: {(int)(City._CitySally._Map[end.y][end.x] * 100)}%";
                    PathLength.text = $"Путь: {(int)(star._Lenght * 10) / 10f}";
                }
            }
            else
            {
                Vector2 mousePosition = SallyMap.MouseToInsidePosition();
                Vector2Int end = new Vector2Int(Mathf.RoundToInt(mousePosition.x / 100f) + 12, Mathf.RoundToInt(mousePosition.y / 100f) + 12);

                if (end != End)
                {
                    End = end;

                    CurrentTile.anchoredPosition = end * 100 + Vector2Int.one * 50;
                    CurentTileAnimator.Play("PointedTile");

                    TilePoint.text = $"Координаты: {end}";
                    TileCoef.text = $"Проходимость: {(int)(City._CitySally._Map[end.y][end.x] * 100)}%";
                    PathLength.text = $"Путь: -";
                }
            }
        }
        else
        {
            CurrentTile.gameObject.SetActive(false);
        }
    }

    public void OpenSally(Sally sally)
    {
        if(Window == null)
        {
            Window = WindowCreator.CreateWindow<SallyWindow>();
            Window.Move(new Vector2(0, -StaticTools.ScreenHeight/2 + 238f));
        }

        Window.SetInfo(this, sally);
    }
    public bool ClickSally(Sally sally)
    {
        SelectedSally = sally;

        Vector2Int position = new Vector2Int(Mathf.FloorToInt(sally._Position.x), Mathf.FloorToInt(sally._Position.y));

        string[] variants = new string[0];
        int[] indexes = new int[0];
        for (int i = 0; i < City._CitySally._Sallies.Length; i++) 
        {
            Sally sally1 = City._CitySally._Sallies[i];
            
            if (Mathf.FloorToInt(sally1._Position.x) == position.x && Mathf.FloorToInt(sally1._Position.y) == position.y)
            {
                variants = StaticTools.ExpandMassive(variants, sally1._Name);
                indexes = StaticTools.ExpandMassive(indexes, i);
            }
        }

        if(variants.Length > 1)
        {
            if(Window != null)
            {
                variants = StaticTools.ExpandMassive(variants, $"Объединить {Window._Sally._Name} с отрядом");
                indexes = StaticTools.ExpandMassive(indexes, -2);
            }

            variants = StaticTools.ExpandMassive(variants, "Объединить все отряды");
            indexes = StaticTools.ExpandMassive(indexes, -1);

            UserInteract.AskVariants("Выбрать отряд", variants, indexes, ClickSally);
            return true;
        }

        return false;
    }
    public void ClickSally(int index)
    {
        if(index == -1)
        {
            Sally sally = SelectedSally;
            Vector2Int position = new Vector2Int(Mathf.FloorToInt(SelectedSally._Position.x), Mathf.FloorToInt(SelectedSally._Position.y));
            int i = 0;
            int safe = 0;
            while(i < City._CitySally._Sallies.Length && safe < 50)
            {
                safe++;
                if (City._CitySally._Sallies[i] == sally)
                {
                    i++;
                    continue;
                }
                if(position.x == Mathf.FloorToInt(City._CitySally._Sallies[i]._Position.x) && position.y == Mathf.FloorToInt(City._CitySally._Sallies[i]._Position.y))
                {
                    sally.Combine(City._CitySally._Sallies[i]);
                    i--;
                }

                i++;
            }
            return;
        }
        if(index < 0)
        {
            Vector2Int position;
            
            if(_Current != null)
            {
                position = new Vector2Int(Mathf.FloorToInt(_Current._Position.x), Mathf.FloorToInt(_Current._Position.y));
            }
            else
            {
                position = new Vector2Int(Mathf.FloorToInt(SelectedSally._Position.x), Mathf.FloorToInt(SelectedSally._Position.y));
            }
            
            string[] variants = new string[0];
            int[] indexes = new int[0];
            for (int i = 0; i < City._CitySally._Sallies.Length; i++)
            {
                Sally sally1 = City._CitySally._Sallies[i];

                if (Mathf.FloorToInt(sally1._Position.x) == position.x && Mathf.FloorToInt(sally1._Position.y) == position.y)
                {
                    variants = StaticTools.ExpandMassive(variants, sally1._Name);
                    indexes = StaticTools.ExpandMassive(indexes, i);
                }
            }

            if (index == -2 && _Current != null)
            {
                UserInteract.AskVariants($"Объединить {_Current._Name} с", variants, indexes, CombineSallies);
            }
            else if (index == -1)
            {
                Sally sally = City._CitySally._Sallies[indexes[0]];
                for(int i = 1; i < indexes.Length; i++)
                {
                    sally.Combine(City._CitySally._Sallies[indexes[i]]);
                }
            }
            return;
        }

        OpenSally(City._CitySally._Sallies[index]);
    }
    public void CombineSallies(int index)
    {
        if(_Current == null)
        {
            return;
        }

        Vector2Int position = new Vector2Int(Mathf.FloorToInt(_Current._Position.x), Mathf.FloorToInt(_Current._Position.y));

        Sally sally = City._CitySally._Sallies[index];
        if(position.x == Mathf.FloorToInt(sally._Position.x) && position.y == Mathf.FloorToInt(sally._Position.y))
        {
            _Current.Combine(sally);
        }
    }

    public void OpenSallyContent(TileContent content)
    {
        Vector2Int end = content.Position;
        SelectedContent = content;
        SelectedEnd = end;

        string[] variants = new string[0];
        int[] indexes = new int[0];
        for(int i = 0; i < City._CitySally._Sallies.Length; i++)
        {
            Sally sally = City._CitySally._Sallies[i];
            if (Mathf.FloorToInt(sally._Position.x) == content.Position.x && Mathf.FloorToInt(sally._Position.y) == content.Position.y)
            {
                variants = StaticTools.ExpandMassive(variants, $"{sally._Name}");
                indexes = StaticTools.ExpandMassive(indexes, i);
            }
        }
        if(variants .Length > 0)
        {
            if(Window != null && !(Mathf.FloorToInt(Window._Sally._Position.x) == content.Position.x && Mathf.FloorToInt(Window._Sally._Position.y) == content.Position.y))
            {
                variants = StaticTools.ExpandMassive(variants, "Отправить текущий отряд.");
                indexes = StaticTools.ExpandMassive(indexes, -1);
            }

            variants = StaticTools.ExpandMassive(variants, "Создать другой отряд и отправить.");
            indexes = StaticTools.ExpandMassive(indexes, -2);


            UserInteract.AskVariants("Спасти медведей", variants, indexes, ResqueContent);
            return;
        }

        if (City._CitySally._Map[end.y][end.x] > 0)
        {
            if (Window != null)
            {
                UserInteract.AskConfirm("Изменить маршрут", $"Вы собираетесь отправить отряд на x{end.x} y{end.y}к месту падения медведей.", Destinate);
            }
            else
            {
                UserInteract.AskConfirm("Создать отряд", $"Вы собираетесь сформировать отряд и отправить на x{end.x} y{end.y} к месту падения медведей.", CreateSally);
            }
        }
    }
    public void ResqueContent(int index)
    {
        if (index == -1)
        {
            Destinate(true);
            return;
        }
        else if(index == -2)
        {
            CreateSally(true);
            return;
        }

        SelectedContent.State = 2;
        City._CitySally.NotifyContents();

        City._CitySally._Sallies[index].AddContent(SelectedContent);
    }

    public void Click()
    {
        if (!NewTutorialSystem.Instance._Closed && NewTutorialSystem.Instance._TutorialStage < 13)
        {
            return;
        }

        Vector2 mousePosition = SallyMap.MouseToInsidePosition();
        Vector2Int end = new Vector2Int(Mathf.RoundToInt(mousePosition.x / 100f) + 12, Mathf.RoundToInt(mousePosition.y / 100f) + 12);

        SelectedEnd = end;

        if (City._CitySally._Map[end.y][end.x] > 0)
        {
            if (Window != null)
            {
                UserInteract.AskConfirm("Изменить маршрут", $"Вы собираетесь отправить отряд на x{end.x} y{end.y}.\nБудьте осторожнее, возможно провиант расчитывался на предыдущий путь.", Destinate);
            }
            else
            {
                UserInteract.AskConfirm("Создать отряд", $"Вы собираетесь сформировать отряд и отправить на x{end.x} y{end.y}.\nКак только вы снарядите отряд, отпрвьте его в путь.", CreateSally);
            }
        }
    }
    public void CreateSally()
    {
        SelectedEnd = CitySally.TownPoint;
        CreateSally(true);
    }
    public void CreateSally(bool state)
    {
        if (!state)
        {
            return;
        }

        if(City._CitySally._Sallies.Length == 10)
        {
            UserInteract.AskMessage("Невозможно создать отряд", "Количество отрядов достигло максимума.");
            return;
        }

        NewTutorialSystem.Instance.SallyCreated();

        CitySally.Sally sally = new CitySally.Sally();
        sally._Name = $"#{City._CitySally._Sallies.Length + 1}";
        sally._Position = CitySally.TownPoint + Vector2.one / 2f;
        sally._Destination = SelectedEnd + Vector2.one / 2f;
        sally._Honey = 0;

        City._CitySally.RegisterSally(sally, false);

        if (Window == null)
        {
            Window = WindowCreator.CreateWindow<SallyWindow>();
            Window.Move(new Vector2(0, -StaticTools.ScreenHeight / 2 + 238f));
        }

        Window.SetInfo(this, sally);
    }
    public void Destinate(bool state)
    {
        if (!state)
        {
            return;
        }

        if (Window != null)
        {
            Window.SetDestination(new Vector2(SelectedEnd.x + 0.5f, SelectedEnd.y + 0.5f));
        }
    }

    public void SetScale(float value)
    {
        Scale = value;
        Map.localScale = Vector3.one * Mathf.Lerp(0.35f, 1.35f, value);
    }
}
