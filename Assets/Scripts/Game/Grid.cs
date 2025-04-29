using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private LayerMask GridMask;
    [SerializeField] private LayerMask DefaultMask;
    [SerializeField] private Vector3Int Point;
    [SerializeField] private Vector3 RealPoint;
    [SerializeField] private bool PointAtGrid;

    public Vector3Int _Point => Point;
    public Vector3 _RealPoint => RealPoint;
    public bool _PointAtGrid => PointAtGrid;

    private void Update()
    {
        bool onGrid = false;

        RaycastHit hit;
        if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, GridMask))
        {
            RealPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            Point = new Vector3Int(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z));
            onGrid = true;
        }

        PointAtGrid = onGrid && !Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, DefaultMask);
    }
}
