using UnityEngine;

public class astarTest : MonoBehaviour
{
    [SerializeField] private GameObject TilePrefab;
    private GameObject[] Tiles = new GameObject[0];

    [SerializeField] private Vector2Int Point;
    [SerializeField] private Transform End;
    [SerializeField] private string[] MapInfo;
    private float[][] Map;

    private void Start()
    {
        Map = new float[MapInfo.Length][];
        for (int i = 0; i < Map.Length; i++)
        {
            string[] mapline = MapInfo[i].Split(" ");
            float[] line = new float[mapline.Length];
            for (int ii = 0; ii < line.Length; ii++)
            {
                line[ii] = StaticTools.StringToFloat(mapline[ii]);

                Instantiate(TilePrefab, new Vector3(ii + 0.5f, i + 0.5f, 0.1f), transform.rotation).GetComponent<SpriteRenderer>().color = new Color(line[ii], line[ii], line[ii]);
            }
            Map[i] = line;
        }

        print(Newtonsoft.Json.JsonConvert.SerializeObject(Map));
    }

    private void Update()
    {
        SquareAStar star = new SquareAStar(Map, Point, new Vector2Int(Mathf.FloorToInt(End.position.x), Mathf.FloorToInt(End.position.y)));

        if (star._Path == null || star._Path.Length == 0)
        {
            return;
        }

        foreach (GameObject tile in Tiles)
        {
            Destroy(tile);
        }

        Vector2Int position = Point;
        Tiles = new GameObject[star._Path.Length + 1];
        for (int i = 0; i < star._Path.Length; i++)
        {
            Tiles[i] = Instantiate(TilePrefab, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), transform.rotation);
            position += star._Path[i];
        }
        Tiles[Tiles.Length - 1] = Instantiate(TilePrefab, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), transform.rotation);

    }
}
