using UnityEngine;

public class FillSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] Prefabs;
    [SerializeField] private int Count;
    [SerializeField] private Vector2 Diapazone;
    [SerializeField] private Vector2 MinMaxSize;

    private void Start()
    {
        for(int i = 0; i < Count; i++)
        {
            GameObject prefab = Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], transform);
            prefab.transform.localScale = Vector3.one * Random.Range(MinMaxSize[0], MinMaxSize[1]);
            prefab.transform.localPosition = new Vector3(Random.Range(-Diapazone.x, Diapazone.x), 0, Random.Range(-Diapazone.y, Diapazone.y));
            prefab.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        }
    }
}
