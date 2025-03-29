using Unity.AI.Navigation;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private float HeightMultiplier;
    [SerializeField] private float Mastab;
    [SerializeField] private int MapSize;
    [SerializeField] private int NoiseIterations;

    [Header("Объектики")]
    [SerializeField] private GameObject[] TreePrefabs;
    [SerializeField] private GameObject[] RockPrefabs;
    [SerializeField] private Transform ObjectsParent;
    [SerializeField] private int TreeCount;
    [SerializeField] private int RockCount;

    public void Initialize()
    {
        int size = 0;
        switch (SaveManager._Instance._SaveData.MapSize)
        {
            case 0:
                size = 150;
                break;
            case 1:
                size = 175;
                break;
            case 2:
                size = 200;
                break;
        }
        size += 50;
        MapSize = size;

        Mesh mesh = new Mesh();

        Vector3[] verices = new Vector3[size * size];
        Vector2[] uvs = new Vector2[verices.Length];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float value = ProceduralTools.OctavianNoise(new Vector2(x - 0.1f, y - 0.1f), SaveManager._Instance._SaveData.Seed, 0, Mastab, NoiseIterations);
                verices[x + y * size] = new Vector3(size / 2f - x, value * HeightMultiplier, size / 2f - y);
                uvs[x + y * size] = new Vector2(x, y) / size;
            }
        }

        int[] triangles = new int[6 * (size - 1) * (size - 1)];
        for (int i = 0; i < triangles.Length / 6 + size - 2; i++)
        {
            int offset = i / size * 6;
            if (i % size != size - 1)
            {
                triangles[i * 6 - offset] = i;
                triangles[i * 6 - offset + 1] = i + size;
                triangles[i * 6 - offset + 2] = i + 1;

                triangles[i * 6 - offset + 3] = i + size;
                triangles[i * 6 - offset + 4] = i + size + 1;
                triangles[i * 6 - offset + 5] = i + 1;
            }
        }

        mesh.vertices = verices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.SetUVs(0, uvs);
        mesh.RecalculateUVDistributionMetrics();

        MeshFilter.mesh = mesh;

        MeshFilter.gameObject.AddComponent<MeshCollider>();

        NavMeshSurface.activeSurfaces[0].BuildNavMesh();

        PlaceObjects();
    }

    public void PlaceObjects()
    {
        for(int i = 0; i < TreeCount; i++)
        {
            int tree = Mathf.RoundToInt(Mathf.Lerp(0, TreePrefabs.Length - 1, ProceduralTools.PseudoRandom((SaveManager._Seed + $"tree{i}").GetHashCode())));

            Transform newTree = Instantiate(TreePrefabs[tree], ObjectsParent).transform;
            newTree.localScale = Vector3.one * Mathf.Lerp(0.5f, 2f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"tree{i}size").GetHashCode()));
            newTree.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, ProceduralTools.PseudoRandom((SaveManager._Seed + $"tree{i}rotation").GetHashCode())), 0);
            newTree.position = GetSpawnPosition($"pos{i}tree");
        }
        for (int i = 0; i < RockCount; i++)
        {
            int tree = Mathf.RoundToInt(Mathf.Lerp(0, RockPrefabs.Length - 1, ProceduralTools.PseudoRandom((SaveManager._Seed + $"rock{i}").GetHashCode())));

            Transform newRock = Instantiate(RockPrefabs[tree], ObjectsParent).transform;
            newRock.localScale = Vector3.one * Mathf.Lerp(3, 10f, ProceduralTools.PseudoRandom((SaveManager._Seed + $"rock{i}size").GetHashCode()));
            newRock.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 360, ProceduralTools.PseudoRandom((SaveManager._Seed + $"rock{i}rotation").GetHashCode())), 0);
            newRock.position = GetSpawnPosition($"pos{i}rock");
        }
    }

    public Vector3 GetSpawnPosition(string addseed)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Lerp(-MapSize / 2f, MapSize / 2f, ProceduralTools.PseudoRandom((SaveManager._Seed + "xvii" + addseed + "dvv").GetHashCode()));
        position.z = Mathf.Lerp(-MapSize / 2f, MapSize / 2f, ProceduralTools.PseudoRandom((SaveManager._Seed + "z12rr" + addseed + "[p").GetHashCode()));

        if(Physics.Raycast(position + Vector3.up * 50, Vector3.down, out RaycastHit hit, 100, 128))
        {
            position.y = hit.point.y;
        }

        return position;
    }
}
