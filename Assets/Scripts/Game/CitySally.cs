using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CitySally : MonoBehaviour
{
    [SerializeField] private string[] MapInfo;
    private float[][] Map = null;

    [SerializeField] private Sally[] Sallies;
    [SerializeField] private TileContent[] Contents;
  //  [SerializeField] private float MapOpen;

    public float[][] _Map => Map;
    public Sally[] _Sallies => Sallies;
    public TileContent[] _Contents => Contents;


    public event SimpleVoid OnSalliesUpdate = null;
    public event SimpleVoid OnContentsUpdate = null;

    public static Vector2Int TownPoint => new Vector2Int(12, 12);

    public string _SaveInfo
    {
        get
        {
            string contentInfo = "";
            for(int i = 0; i < Contents.Length; i++)
            {
                contentInfo += $"{Contents[i].State};";
            }
            if (contentInfo.EndsWith(";"))
            {
                contentInfo = contentInfo.Remove(contentInfo.Length - 1);
            }
            string info = $"Contents({contentInfo})Count({Sallies.Length})";

            for(int i = 0; i < Sallies.Length; i++)
            {
                info += $"S{i}({Sallies[i]._SaveInfo})";
            }

            return info;
        }
        set
        {
            Map = new float[MapInfo.Length][];
            for (int i = 0; i < Map.Length; i++)
            {
                string[] mapline = MapInfo[i].Split(" ");
                float[] line = new float[mapline.Length];
                for (int ii = 0; ii < line.Length; ii++)
                {
                    line[ii] = StaticTools.StringToFloat(mapline[ii]);
                }
                Map[i] = line;
            }

            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            string[] contents = parameters["Contents"].Split(";");
            for(int i = 0; i < contents.Length; i++)
            {
                Contents[i].State = StaticTools.StringToInt(contents[i]);
            }

            Sallies = new Sally[int.Parse(parameters["Count"])];
            for(int i = 0; i < Sallies.Length; i++)
            {
                Sally sally = new Sally();
                Sallies[i] = sally;
                sally._SaveInfo = parameters[$"S{i}"];
            }

            OnSalliesUpdate?.Invoke();
        }
    }

    private void Awake()
    {
        if(Map != null)
        {
            return;
        }
        Map = new float[MapInfo.Length][];
        for (int i = 0; i < Map.Length; i++)
        {
            string[] mapline = MapInfo[i].Split(" ");
            float[] line = new float[mapline.Length];
            for (int ii = 0; ii < line.Length; ii++)
            {
                line[ii] = StaticTools.StringToFloat(mapline[ii]);
            }
            Map[i] = line;
        }
    }

    public void NotifyContents() => OnContentsUpdate?.Invoke();

    public void RegisterSally(Sally sally, bool remove)
    {
        if (remove)
        {
            Sallies = StaticTools.RemoveFromMassive(Sallies, sally);
            sally.Notify();
        }
        else
        {
            Sallies = StaticTools.ExcludingExpandMassive(Sallies, sally);
        }

        OnSalliesUpdate?.Invoke();
    }

    public void HourPassed()
    {
        foreach(Sally sally in Sallies)
        {
            sally.HourPassed();
        }
    }

    [Serializable]
    public class TileContent
    {
        public Vector2Int Position;
        public int State = 0; //0 - не разведан, 1 - разведан, 2 - собран

        public Bear.Kasta[] BearsKast;

        public int EnergyHoney;
        public int Honey;
        public int Wood;
        public int Metal;
        public int Berezenium;
        public int Robots;
    }

    [System.Serializable]
    public class Sally
    {
        [SerializeField] private string Name;
        [SerializeField] private Vector2 Position;
        [SerializeField] private Vector2 Destination;
        [SerializeField] private int Honey;
        [SerializeField] private float Saturation; 
        [SerializeField] private bool Snowrunner = false;
        [SerializeField] private bool AllowMove = false; 

        private Bear[] Bears = new Bear[0];

        [SerializeField] private Vector2[] Path = null;
        [SerializeField] private Vector2Int[] IntPath = null;
        [SerializeField] private float CurrentDistance = 0;
        [SerializeField] private int CurrentDirection = 0;

        private TileContent[] TileContents = new TileContent[0];

        public Vector3 MeetPoint = Vector2.zero;

        public event SimpleVoid OnChanges = null;

        public Bear[] _Bears => Bears;
        public string _Name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
                OnChanges?.Invoke();
            }
        }
        public Vector2Int[] _Path => IntPath;
        public Vector2 _Position
        {
            get
            {
                return Position;
            }
            set
            {
                Position = value;
                RebuildPath();
            }
        }
        public Vector2 _Destination
        {
            get
            {
                return Destination;
            }
            set
            {
                Destination = value;

                RebuildPath();
            }
        }
        public bool _Snowrunner
        {
            get
            {
                return Snowrunner;
            }
            set
            {
                Snowrunner = value;
                OnChanges?.Invoke();
            }
        }
        public bool _AllowMove
        {
            get
            {
                return AllowMove;
            }
            set
            {
                AllowMove = value;

                if (Bears.Length == 0)
                {
                    AllowMove = false;

                    UserInteract.AskMessage("Нельзя продолжить путь", "Отряд пуст, чтобы начать вылазку назначьте медведей.");
                }
                else if (Path == null || Path.Length == 0)
                {
                    AllowMove = false;

                    UserInteract.AskMessage("Нельзя продолжить путь", "Местоположение отряда совпадает с точкой назначения.");
                }
                else
                {
                    bool zero = true;
                    foreach (Bear bear in Bears)
                    {
                        if (bear._Health > 1)
                        {
                            zero = false;
                            break;
                        }
                    }

                    if (zero)
                    {
                        AllowMove = false;

                        UserInteract.AskMessage("Невозможно продолжить путь", "Все медведи в отряде в критическом состоянии.\nВам следует отправить другой отряд на помощь.");
                    }
                }

                OnChanges?.Invoke();
            }
        }

        public float _TeamSpeed
        {
            get
            {
                if(Bears.Length == 0)
                {
                    return 0;
                }

                float average = 0;
                foreach (Bear bear in Bears)
                {
                    average += bear._Work;
                    if(bear._Kasta == Bear.Kasta.Первопроходец)
                    {
                        average++;
                    }
                }
                average /= Bears.Length;
                return average  + (Snowrunner ? 10 : 0);
            }
        }
        public float _Speed => _TeamSpeed * City._CitySally._Map[Mathf.FloorToInt(Position.y)][Mathf.FloorToInt(Position.x)];
        public float _Distance
        {
            get
            {
                float lenght = 0;

                if(Path.Length == 0 || CurrentDirection >= Path.Length)
                {
                    return lenght;
                }

                Vector2Int position = new Vector2Int(Mathf.FloorToInt(Position.x), Mathf.FloorToInt(Position.y));

                Vector2 targetTile = Position + Path[CurrentDirection].normalized * CurrentDistance;
                Vector2 nextTile = position + IntPath[CurrentDirection] + new Vector2(0.5f, 0.5f);

                if (targetTile != nextTile)
                {
                    lenght += CurrentDistance / City._CitySally.Map[position.y][position.x];
                }
                else
                {
                    nextTile += new Vector2(-0.5f * (IntPath[CurrentDirection].x), -0.5f * (IntPath[CurrentDirection].y));

                    lenght += Vector2.Distance(Position, nextTile) / City._CitySally.Map[position.y][position.x];
                    lenght += Vector2.Distance(nextTile, targetTile) / City._CitySally.Map[position.y + IntPath[CurrentDirection].y][position.x + IntPath[CurrentDirection].x];

                    position += IntPath[CurrentDirection];
                }

                for (int i = CurrentDirection + 1; i < IntPath.Length; i++)
                {
                    lenght += DistancePC(position, position + IntPath[i], City._CitySally.Map);
                    position += IntPath[i];
                }

                return lenght;
            }
        }
        public int _Honey
        {
            get
            {
                return Honey;
            }
            set
            {
                Honey = value;
                OnChanges?.Invoke();
            }
        }
        public float _Saturation => Saturation;

        public string _SaveInfo
        {
            get
            {
                string info = $"Name({Name})Pos({Position.x};{Position.y})Dest({Destination.x};{Destination.y})Honey({Honey})Snowrunner({Snowrunner.GetHashCode()})Bears(";

                foreach (Bear bear in Bears)
                {
                    info += $"{Array.IndexOf(City._DataBase._Bears, bear)};";
                }
                if (info.EndsWith(";"))
                {
                    info = info.Remove(info.Length - 1);
                }
                info += ")";

                info += "Contents(";
                foreach(TileContent content in TileContents)
                {
                    info += $"{StaticTools.IndexOf(City._CitySally.Contents, content)};";
                }
                if (info.EndsWith(";"))
                {
                    info = info.Remove(info.Length - 1);
                }
                info += ")";

                return info;
            }
            set
            {
                Dictionary<string, string> parameters = StaticTools.GetParameters(value);

                Name = parameters["Name"];
                Position = new Vector2(float.Parse(parameters["Pos"].Split(";")[0]), float.Parse(parameters["Pos"].Split(";")[1]));
                Destination = new Vector2(float.Parse(parameters["Dest"].Split(";")[0]), float.Parse(parameters["Dest"].Split(";")[1]));
                Honey = int.Parse(parameters["Honey"]);
                Snowrunner = parameters["Snowrunner"] == "1";
                
                string[] bears = parameters["Bears"].Split(";");
                if (bears[0] != "")
                {
                    Bears = new Bear[bears.Length];
                    for (int i = 0; i < Bears.Length; i++)
                    {
                        Bears[i] = City._DataBase._Bears[StaticTools.StringToInt(bears[i])];
                        Bears[i]._Sally = this;
                    }
                }

                string[] contents = parameters["Contents"].Split(';');
                if (contents[0] != "")
                {
                    TileContents = new TileContent[contents.Length];
                    for (int i = 0; i < TileContents.Length; i++)
                    {
                        TileContents[i] = City._CitySally._Contents[StaticTools.StringToInt(contents[i])];
                    }
                }

                MeetPoint = Bear.GetRandomPosition();

                RebuildPath();
            }
        }

        public Sally()
        {
            try
            {
                do
                {
                    MeetPoint = new Vector3(UnityEngine.Random.Range(-45f, 45f), 0, UnityEngine.Random.Range(-45f, 45f));
                }
                while (Physics.CheckSphere(MeetPoint, 0.1f, 256));
            }
            catch
            {

            }
        }

        public static float DistancePC(Vector2Int a, Vector2Int b, float[][] map)
        {
            if (a.x - b.x != 0 && a.y - b.y != 0)
            {
                return 0.705f / map[a.y][a.x] + 0.705f / map[b.y][b.x];
            }

            return 0.5f / map[a.y][a.x] + 0.5f / map[b.y][b.x];
        }

        public void Notify() => OnChanges?.Invoke();

        public void AutoSupply()
        {
            bool pervoprohodec = false;
            foreach (Bear bear in City._DataBase._Bears)
            {
                if (Bears.Length >= 2)
                {
                    break;
                }

                if (bear._Sally == null && bear._Facility == null && bear._Kasta == Bear.Kasta.Первопроходец)
                {
                    pervoprohodec = true;
                    Bears = StaticTools.ExpandMassive(Bears, bear);
                    bear._Sally = this;
                }
            }
            if (!pervoprohodec)
            {
                foreach (Bear bear in City._DataBase._Bears)
                {
                    if (Bears.Length >= 2)
                    {
                        break;
                    }

                    if (bear._Sally == null && bear._Facility == null)
                    {
                        Bears = StaticTools.ExpandMassive(Bears, bear);
                        bear._Sally = this;
                    }
                }
            }

            if(Bears.Length == 0)
            {
                UserInteract.AskMessage("Не удалось снарядить отряд", "Не хватает свободных медведей, назначьте медведей вручную или освободите их от работы.");
                return;
            }

            if(City._Storage._Snowrunners > 0)
            {
                Snowrunner = true;

                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Name}", (int)(City._Time._WorldTime / 60), -1, CityStorage.ResourceType.Snowrunners));

                City._Storage._Snowrunners--;
            }

            int hours = Mathf.CeilToInt(_Distance / _TeamSpeed / CityTime._DaySection);
            int requiredFood = Mathf.CeilToInt(hours / 25f * 3f * Bears.Length) * 2 + Bears.Length;

            if(City._Foodstream._StoredFood >= requiredFood)
            {
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Name}", (int)(City._Time._WorldTime / 60), -requiredFood, CityStorage.ResourceType.Honey));
                City._Foodstream._StoredFood -= requiredFood;
                Honey = requiredFood;
            }
            else
            {
                Honey = (int)City._Foodstream._StoredFood;
                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Name}", (int)(City._Time._WorldTime / 60), -Honey, CityStorage.ResourceType.Honey));
                City._Foodstream._StoredFood -= Honey;

                UserInteract.AskMessage("Внимание", "Мы не смогли достаточно снабдить едой отряд. Возможно стоит накопить еду, либо надееться, что голод не коснётся отряда.");
            }

            NewTutorialSystem.Instance.AutoEquipSally();

            OnChanges?.Invoke();
        }

        private void RebuildPath()
        {
            CurrentDirection = 0;
            CurrentDistance = 0;

            Vector2Int position = new Vector2Int(Mathf.FloorToInt(Position.x), Mathf.FloorToInt(Position.y));
            SquareAStar squareAStar = new SquareAStar(City._CitySally._Map, position, new Vector2Int(Mathf.FloorToInt(Destination.x), Mathf.FloorToInt(Destination.y)));
          
            if (squareAStar._Path == null || squareAStar._Path.Length == 0)
            {
                Path = new Vector2[0];
                IntPath = new Vector2Int[0];
                OnChanges?.Invoke();
                return;
            }

            IntPath = squareAStar._Path;
            Path = new Vector2[squareAStar._Path.Length];
            Path[0] = squareAStar._Path[0] - Position + position + Vector2.one / 2f;
            CurrentDistance = Path[0].magnitude;

            for (int i = 1; i < Path.Length; i++)
            {
                Path[i] = squareAStar._Path[i];
            }

            if(!(Mathf.FloorToInt(_Position.x) == TownPoint.x && Mathf.FloorToInt(_Position.y) == TownPoint.y))
            {
                _AllowMove = true;
            }

            OnChanges?.Invoke();
        }

        public void RegisterBear(Bear bear, bool remove)
        {
            if (remove)
            {
                Bears = StaticTools.RemoveFromMassive(Bears, bear);
                bear._Sally = null;
            }
            else
            {
                Bears = StaticTools.ExcludingExpandMassive(Bears, bear);
                bear._Sally = this;
            }
            OnChanges?.Invoke();
        }

        public void Combine(Sally sally)
        {
            if(sally == null || sally == this || !StaticTools.Contains(City._CitySally.Sallies, sally))
            {
                return;
            }

            City._CitySally.RegisterSally(sally, true);

            foreach (Bear bear in sally._Bears)
            {
                Bears = StaticTools.ExcludingExpandMassive(Bears, bear);
                bear._Sally = this;
            }

            foreach (TileContent content in sally.TileContents)
            {
                TileContents = StaticTools.ExcludingExpandMassive(TileContents, content);
            }

            Saturation = (Saturation + sally.Saturation) / 2f;

            Honey += sally._Honey;
            if (sally._Snowrunner)
            {
                Snowrunner = true;
            }

            OnChanges?.Invoke();
        }

        public void AddContent(TileContent content)
        {
            string info = "Вы спасли упавших медведей, а также собрали ресурсы, которые они собирали.\n\nПрисоединившиеся медведи:";

            for(int i = 0; i < content.BearsKast.Length; i++)
            {
                Bear newBear = BearObjectioner._Instance.RequestBear(content.BearsKast[i], content.Position.x, content.Position.y);
                newBear._Effects[4] = UnityEngine.Random.Range(25, 75);
                City._DataBase.RegisterBear(newBear, false);

                RegisterBear(newBear, false);
                info += $"\n{newBear._Kasta} {newBear._Name}";
            }

            info += "\n\nРесурсы:";
            if (content.EnergyHoney > 0)
            {
                info += $"\nЭнергомёд: {content.EnergyHoney}";
            }
            if (content.Honey > 0)
            {
                info += $"\nМёд: {content.Honey}";
            }
            if (content.Wood > 0)
            {
                info += $"\nДревесина: {content.Wood}";
            }
            if (content.Metal > 0)
            {
                info += $"\nМеталл: {content.Metal}";
            }
            if (content.Berezenium > 0)
            {
                info += $"\nБерезениум: {content.Berezenium}";
            }
            if (content.Robots > 0)
            {
                info += $"\nРоботы: {content.Robots}";
            }

            info += "\n\nСтоит ли вернуться в город ?";

            TileContents = StaticTools.ExpandMassive(TileContents, content);

            UserInteract.AskConfirm("Медведи спасены", info, ReturnToTown);

            NewTutorialSystem.Instance.VoprosCollected();
        }
        public void ReturnToTown(bool answer)
        {
            if (answer)
            {
                _Destination = CitySally.TownPoint;
            }
        }

        public void HourPassed()
        {
            int hour = ((int)City._Time._WorldTime % 1500) / 60;
            if (hour == 8 || hour == 16 || hour == 24)
            {
                Saturation = Mathf.Clamp01((float)Honey / Bears.Length / GlobalVariables._BearFoodCost);
                Honey = Mathf.Max(0, Honey - Mathf.RoundToInt(Bears.Length * GlobalVariables._BearFoodCost));
            }
            if(Saturation <= 0.4f && Honey > 0 && Bears.Length > 0)
            {
                float coef = 0;
                if(hour < 8)
                {
                    coef = (8 - hour) / 8f;
                }
                else if(hour < 16)
                {
                    coef = (16 - hour) / 8f;
                }
                else if(hour < 24)
                {
                    coef = (24 - hour) / 8f;
                }
                else if (hour < 32)
                {
                    coef = (32 - hour) / 8f;
                }

                Saturation = Mathf.Clamp01((float)Honey / Mathf.Round(coef * Bears.Length * GlobalVariables._BearFoodCost));
                Honey = Mathf.Max(0, Honey - Mathf.RoundToInt(coef * Bears.Length));
            }
            
            if(Saturation == 0)
            {
                foreach(Bear bear in Bears)
                {
                    bear._Health -= UnityEngine.Random.Range(0, 100f) < 5 ? 1 : 0;
                }
            }

            if (Path.Length == 0)
            {
                return;
            }

            if (!AllowMove)
            {
                return;
            }

            bool zero = true;
            foreach (Bear bear in Bears)
            {
                if (bear._Health > 1)
                {
                    zero = false;
                    break;
                }
            }
            if (zero)
            {
                _AllowMove = false;
                return;
            }

            float time = CityTime._DaySection;
            int it = 0;

            while (time > 0)
            {
                it++;
                Vector2 targetTile = Position + Path[CurrentDirection].normalized * CurrentDistance;
                Vector2 nextTile = new Vector2Int(Mathf.FloorToInt(Position.x), Mathf.FloorToInt(Position.y)) + IntPath[CurrentDirection] + new Vector2(0.5f, 0.5f);
                
                float speed = _Speed;

                if(targetTile != nextTile)
                {
                    if (time * speed > CurrentDistance)
                    {
                        time -= CurrentDistance / speed;
                        Position += Path[CurrentDirection].normalized * CurrentDistance;
                        CurrentDistance = 0;
                    }
                    else
                    {
                        Position += Path[CurrentDirection].normalized * time * speed;
                        CurrentDistance -= time * speed;
                        time = 0;
                    }
                }
                else
                {
                    nextTile += new Vector2(-0.5f * (IntPath[CurrentDirection].x), -0.5f * (IntPath[CurrentDirection].y));

                    float overDistance = Vector2.Distance(Position, nextTile);

                    if (time * speed > overDistance)
                    {
                        overDistance += 0.01f;
                        time -= overDistance / speed;
                        Position += Path[CurrentDirection].normalized * overDistance;
                        CurrentDistance -= overDistance;

                        Vector2Int intPosition = new Vector2Int(Mathf.FloorToInt(Position.x), Mathf.FloorToInt(Position.y));
                        if (intPosition.x == CitySally.TownPoint.x && intPosition.y == CitySally.TownPoint.y)
                        {
                            foreach(TileContent content in TileContents)
                            {
                                City._Storage.AddContent(content);
                                City._CityStatistics.AddStatistic(new CityStatistics.Statistic($"вылазка {Name}", (int)(City._Time._WorldTime / 60), content.Honey, CityStorage.ResourceType.Honey));
                                City._Foodstream._StoredFood += content.Honey;
                            }
                            TileContents = new TileContent[0];

                            Vector3 position = Vector3.zero;
                            foreach (Bear bear in Bears)
                            {
                                if (bear._BearObject == null && bear._Health > 1)
                                {
                                    City._BearObjectioner.Create(bear);

                                    position = Vector3.zero;
                                    if (IntPath[CurrentDirection].x != 0)
                                    {
                                        position.x = -95 * IntPath[CurrentDirection].x;
                                    }
                                    else
                                    {
                                        position.x = UnityEngine.Random.Range(-5, 5f);
                                    }
                                    if (IntPath[CurrentDirection].y != 0)
                                    {
                                        position.z = -95 * IntPath[CurrentDirection].y;
                                    }
                                    else
                                    {
                                        position.z = UnityEngine.Random.Range(-5, 5f);
                                    }
                                    bear._BearObject.transform.position = position;


                                    bear._BearObject._NavMeshAgent.Warp(bear._BearObject.transform.position);
                                    bear._BearObject.SetTask(new BearObject.SallyTask(bear._BearObject, "вылазка", this));
                                }
                            }
                        }
                        else
                        {
                            foreach(TileContent tile in City._CitySally._Contents)
                            {
                                if(tile.State == 0)
                                {
                                    if(tile.Position == intPosition)
                                    {
                                        tile.State = 1;
                                        City._CitySally.NotifyContents();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Position += Path[CurrentDirection].normalized * time * speed;
                        CurrentDistance -= time * speed;
                        time = 0;
                    }
                }

                if (CurrentDistance <= 0)
                {
                    CurrentDirection++;

                    if(CurrentDirection >= Path.Length)
                    {
                        AllowMove = false;

                        NewTutorialSystem.Instance.SallyDestinated();

                        City._CityMessenger.AddMessage(new CityMessenger.CityMessage($"Отряд вылазки ожидает", $"Следующая вылазка ждёт следующих поручений {Name}."));
                        RebuildPath();
                        return;
                    }
                    CurrentDistance = Path[CurrentDirection].magnitude;
                }
            }


            OnChanges?.Invoke();
        }
    }
}
