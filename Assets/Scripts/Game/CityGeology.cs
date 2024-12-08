using UnityEngine;

public class CityGeology : MonoBehaviour
{
    [SerializeField] private LayerMask TreeMask;

    [SerializeField] private GameObject BerezeniumFieldPrefab;
    [SerializeField] private GameObject MetalFieldPrefab;
    [SerializeField] private GameObject WoodFieldPrefab;
    [SerializeField] private Transform Map;
    [SerializeField] private ResourceField[] Fields;

    public ResourceField[] _Fields => Fields;
    public string[] _SaveInfo
    {
        get
        {
            string[] info = new string[Fields.Length];
            for(int i =0; i < info.Length; i++)
            {
                info[i] = Fields[i]._SaveInfo;
            }
            return info;
        }
        set
        {
            Fields = new ResourceField[value.Length];
            for(int i = 0; i < Fields.Length; i++)
            {
                bool exist = true;
                switch(StaticTools.GetParameter(value[i], "Resource"))
                {
                    case "Berezenium":
                        Fields[i] = Instantiate(BerezeniumFieldPrefab, Map).GetComponent<ResourceField>();
                        break;
                    case "Metal":
                        Fields[i] = Instantiate(MetalFieldPrefab, Map).GetComponent<ResourceField>();
                        break;
                    case "Wood":
                        Fields[i] = Instantiate(WoodFieldPrefab, Map).GetComponent<ResourceField>();
                        break;
                    default:
                        Fields = StaticTools.ReduceMassive(Fields, i);
                        exist = false;
                        break;
                }

                if (exist)
                {
                    Fields[i]._SaveInfo = value[i];

                    foreach (Collider collider in Physics.OverlapBox(Fields[i].transform.position, Fields[i]._Sizes/2, Fields[i].transform.rotation, TreeMask))
                    {
                        Destroy(collider.gameObject);
                    }
                }
            }
        }
    }
}
