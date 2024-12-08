using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private LayerMask GridMask;
    [SerializeField] private LayerMask DefaultMask;
    [SerializeField] private Vector3Int Point;
    [SerializeField] private bool PointAtGrid;

    public Vector3Int _Point => Point;
    public bool _PointAtGrid => PointAtGrid;

    private void Update()
    {
        bool onGrid = false;

        RaycastHit hit;
        if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, GridMask))
        {
            Point = new Vector3Int(Mathf.FloorToInt(hit.point.x), 0, Mathf.FloorToInt(hit.point.z));
            onGrid = true;
        }

        PointAtGrid = onGrid && !Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, DefaultMask);
    }
}
