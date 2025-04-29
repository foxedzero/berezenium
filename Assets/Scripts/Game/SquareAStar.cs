using Unity.VisualScripting;
using UnityEngine;

public class SquareAStar
{
    private Vector2Int[] Path = new Vector2Int[0];
    private float Lenght = -1;

    private float[][] Map = null;
    private Vector2Int End;
    private Vector2Int Start;

    private float[][] OriginMap = null;

    private Cell[] OpenCells = new Cell[100];
    private int OpenPointer = 0;

    public Vector2Int[] _Path => Path;
    public Vector2Int _Start => Start;
    public Vector2Int _End => End;
    public float _Lenght
    {
        get
        {
            if(Lenght == -1 && Path != null)
            {
                Lenght = 0;
                Vector2Int position = Start;

                foreach(Vector2Int direction in Path)
                {
                    Lenght += DistancePC(position, position + direction, this);
                    position += direction;
                }
            }

            return Lenght;  
        }
    }

    public SquareAStar(float[][] map, Vector2Int start, Vector2Int end)
    {
        Start = start;
        End = end;

        OriginMap = map;

        Map = new float[map.Length][];

        for (int i = 0; i < Map.Length; i++)
        {
            if (map[i] != null)
            {
                float[] line = new float[map[i].Length];
                for (int ii = 0; ii < line.Length; ii++)
                {
                    line[ii] = map[i][ii];
                }

                Map[i] = line;
            }
        }

        Solve();
    }

    private void Solve()
    {
        if (Start.x < 0 || Start.y < 0)
        {
            Path = null;
            return;
        }

        if (Map[Start.y] == null || Start.x >= Map[Start.y].Length)
        {
            Path = null;
            return;
        }
        else if (Map[Start.y][Start.x] == 0)
        {
            Path = null;
            return;
        }

        if (End.x < 0 || End.y < 0)
        {
            Path = null;
            return;
        }

        if (Map[End.y] == null || End.x >= Map[End.y].Length)
        {
            Path = null;
            return;
        }
        else if (Map[End.y][End.x] == 0)
        {
            Path = null;
            return;
        }

        Open(null, Start);
        if (OpenCells[0] == null)
        {
            Path = null;
            return;
        }

        if(Start == End)
        {
            Path = new Vector2Int[0];
            return;
        }

        Cell current = OpenCells[0];

        int safe = 0;

        while (safe < 10000)
        {
            safe++;

            current = LowestF();
            if (current == null)
            {
                Path = null;
                return;
            }
            else if (current.Position == End)
            {
                break;
            }
            else
            {
                OpenPointer--;
                OpenCells[current.Index] = OpenCells[OpenPointer];
                OpenCells[OpenPointer].Index = current.Index;

                Map[OpenCells[OpenPointer].Position.y][OpenCells[OpenPointer].Position.x] = -2 - current.Index;
                Map[current.Position.y][current.Position.x] = -2;

                OpenCells[OpenPointer] = null;

                Expand(current);
            }
        }

        if (safe < 10000)
        {
            Path = current.Path(End);

            int index = 0;
            Vector2Int position = Start;
            Vector2Int[] newPath = new Vector2Int[0];
            bool last = false;
            while (index + 1 < Path.Length)
            {
                Vector2Int direction = Path[index + 1] + Path[index];
                if (direction.x != 0 && direction.y != 0)
                {
                    if (OriginMap[position.y + Path[index + 1].y][position.x + Path[index + 1].x] > 0)
                    {
                        newPath = StaticTools.ExpandMassive(newPath, direction);
                        index++;

                        if (index + 1 >= Path.Length)
                        {
                            last = true;
                        }

                        position += direction;
                    }
                    else
                    {
                        newPath = StaticTools.ExpandMassive(newPath, Path[index]);
                        position += Path[index];
                    }
                }
                else
                {
                    newPath = StaticTools.ExpandMassive(newPath, Path[index]);
                    position += Path[index];
                }

                index++;
            }

            if (!last)
            {
                newPath = StaticTools.ExpandMassive(newPath, Path[Path.Length - 1]);
            }

            Path = newPath;
        }

        OpenCells = null;
    }

    private void Expand(Cell current)
    {
        Open(current, current.Position + new Vector2Int(0, 1));
        Open(current, current.Position + new Vector2Int(1, 0));
        Open(current, current.Position + new Vector2Int(0, -1));
        Open(current, current.Position + new Vector2Int(-1, 0));
    }

    private void Open(Cell parent, Vector2Int position)
    {
        if (position.x < 0 || position.y < 0)
        {
            return;
        }

        if(position.y >= Map.Length)
        {
            return;
        }
        else if (position.x >= Map[position.y].Length)
        {
            return;
        }
        else if (Map[position.y][position.x] <= 0)
        {
            if (Map[position.y][position.x] < -2)
            {
                int index = -(int)(Map[position.y][position.x] + 2);

                if (OpenCells[index].Parent.J > parent.J)
                {
                    OpenCells[index].SetParent(parent);
                }
            }

            return;
        }

        Map[position.y][position.x] = -2 - OpenPointer;

        Cell newCell = new Cell(this, Distance(position, End), parent, position);
        newCell.Index = OpenPointer;
        OpenCells[OpenPointer] = newCell;
        OpenPointer++;

        if (OpenCells.Length <= OpenPointer)
        {
            Cell[] newOpen = new Cell[OpenCells.Length * 4];
            System.Array.Copy(OpenCells, newOpen, OpenCells.Length);

            OpenCells = newOpen;
        }
    }

    private Cell LowestF()
    {
        if (OpenCells.Length < 1)
        {
            return null;
        }

        int lowest = 0;

        for (int i = 1; i < OpenPointer; i++)
        {
            if (OpenCells[lowest].F > OpenCells[i].F)
            {
                lowest = i;
            }
        }

        return OpenCells[lowest];
    }

    //public static float Distance(Vector3 a, Vector3 b) => (Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z)) +  Mathf.Abs(a.y - b.y) * 7 ) * 10;
    //public static int Distance(Vector3 a, Vector3 b) => Mathf.RoundToInt(Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) * 16 + (a.z - b.z) * (a.z - b.z)) * 10);
    public static float Distance(Vector2 a, Vector2 b) => Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    public static float DistancePC(Vector2Int a, Vector2Int b, SquareAStar aStar)
    {
        if (a.x - b.x != 0 && a.y - b.y != 0)
        {
            return 0.705f / aStar.OriginMap[a.y][a.x] + 0.705f / aStar.OriginMap[b.y][b.x];
        }

        return 0.5f / aStar.OriginMap[a.y][a.x] + 0.5f / aStar.OriginMap[b.y][b.x];
    }

    [System.Serializable]
    private class Cell
    {
        public SquareAStar AStar;
        public Cell Parent;
        public Vector2Int Position;
        public float J; //path lenght
        public float H; //distance to end
        public float F; // summand of distances

        public int Index = 0;

        public Cell(SquareAStar aStar, float h, Cell parent, Vector2Int position)
        {
            AStar = aStar;

            Position = position;

            H = h;
            
            Parent = parent;
            if (Parent != null)
            {
                J = Parent.J + DistancePC(position, Parent.Position, AStar);
            }
            else
            {
                J = 0;
            }

            F = H + J;
        }

        public void SetParent(Cell parent)
        {
            Parent = parent;
            if (Parent != null)
            {
                J = Parent.J + DistancePC(Position, Parent.Position, AStar);
            }
            else
            {
                J = 0;
            }

            F = H + J;
        }

        public Vector2Int[] Path(Vector2Int childPosition)
        {
            Vector2Int position = childPosition - Position;

            if (Parent != null)
            {
                if (position == Vector2Int.zero)
                {
                    return Parent.Path(Position);
                }

                return StaticTools.ExpandMassive(Parent.Path(Position), position);
            }
            else
            {
                return new Vector2Int[1] { position };
            }
        }
    }

}
