using UnityEngine;
using UnityEngine.UI;

public class PathSlot : MonoBehaviour
{
    [SerializeField] private UserFileOpen FileOpen;
    [SerializeField] private Text Name;
    [SerializeField] private string Path;

    public float _Width
    {
        get
        {
            float width = 5 + Name.preferredWidth;

            if(width < 20)
            {
                width = 20;
            }

            GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0);

            return width;
        }
    }

    public void SetInfo(UserFileOpen fileOpen, string path)
    {
        FileOpen = fileOpen;
        Path = path;

        string name = System.IO.Path.GetFileName(path);
        
        if(name == "")
        {
            name = path;
        }

        Name.text = name;
    }

    public void Click() => FileOpen.Open(Path, true, true);
}
