using UnityEngine;

public class ResourceField : MonoBehaviour
{
    [SerializeField] private CityStorage.ResourceType ResourceType;
    [SerializeField] private Vector3 Sizes;
    [SerializeField] private float Count;

    public CityStorage.ResourceType _ResourceType => ResourceType;
    public Vector3 _Sizes => Sizes;
    public float _Count
    {
        get
        {
            return Count;
        }
        set
        {
            Count = value;
        }
    }

    public string _SaveInfo
    {
        get
        {
            return $"Transform({Mathf.FloorToInt(transform.position.x)};{Mathf.FloorToInt(transform.position.z)};{Mathf.FloorToInt(transform.localEulerAngles.y / 90)})Resource({ResourceType})Count({Count})";
        }
        set
        {
            string[] trans = StaticTools.GetParameter(value, "Transform").Split(';');
            Vector3 position = new Vector3(StaticTools.StringToInt(trans[0]), 0, StaticTools.StringToInt(trans[1]));
            transform.localPosition = position;
            transform.localEulerAngles = new Vector3(0, 90 * StaticTools.StringToInt(trans[2]), 0);

            Count = StaticTools.StringToFloat(StaticTools.GetParameter(value, "Count"));
        }
    }
}
