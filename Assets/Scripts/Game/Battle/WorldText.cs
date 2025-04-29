using UnityEngine;
using UnityEngine.UI;

public class WorldText : MonoBehaviour
{
    [SerializeField] private Text Text;

    public string _Text
    {
        get
        {
            return Text.text;
        }
        set
        {
            Text.text = value;
        }
    }

    public Color _Color
    {
        get
        {
            return Text.color;
        }
        set
        {
            Text.color = value;
        }
    }
}
