using UnityEngine;

public class Ground : MonoBehaviour
{
    public enum GroundMaterial { Wood, Metal, Beton, Snow }

    [SerializeField] private GroundMaterial Material;

    public GroundMaterial _Material
    {
        get
        {
            return Material;
        }
        set
        {
            Material = value;
        }
    }
}
