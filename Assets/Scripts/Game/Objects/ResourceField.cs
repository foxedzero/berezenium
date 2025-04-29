using System.Collections.Generic;
using UnityEngine;

public class ResourceField : MonoBehaviour
{
    [SerializeField] private CityStorage.ResourceType ResourceType;
    [SerializeField] private Vector3 Sizes;

    public CityStorage.ResourceType _ResourceType => ResourceType;
    public Vector3 _Sizes => Sizes;
    public string _SaveInfo
    {
        get
        {
            return $"Transform({Mathf.FloorToInt(transform.position.x)};{Mathf.FloorToInt(transform.position.z)};{Mathf.FloorToInt(transform.localEulerAngles.y / 90)})Resource({ResourceType})";
        }
        set
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            string[] trans = parameters["Transform"].Split(';');
            Vector3 position = new Vector3(StaticTools.StringToInt(trans[0]), 0, StaticTools.StringToInt(trans[1]));

            if(Physics.BoxCast(position + Vector3.up * 50, new Vector3(6, 0.01f, 6), Vector3.down, out RaycastHit hit, transform.rotation, 100, 128))
            {
                position.y = hit.point.y;
            }

            transform.localPosition = position;
            transform.localEulerAngles = new Vector3(0, 90 * StaticTools.StringToInt(trans[2]), 0);
        }
    }
}
