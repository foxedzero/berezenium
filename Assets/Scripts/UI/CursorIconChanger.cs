using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private static CursorManager Instance = null;

    [SerializeField] private CursorIconInfo DefaultCursor;
    private CursorIconInfo[] Icons = new CursorIconInfo[0];
    private NeedCursorOrder[] NeedMouse = new NeedCursorOrder[0];

    public static bool _UILocked => Instance.NeedMouse.Length > 0;

    private void Awake()
    {
        Instance = this;

        _EditIcon(DefaultCursor, false);
    }

    public static void SetNeedMouse(NeedCursorOrder needMouse, bool remove)
    {
        if (remove)
        {
            Instance.NeedMouse = StaticTools.RemoveFromMassive(Instance.NeedMouse, needMouse);
        }
        else
        {
            Instance.NeedMouse = StaticTools.ExpandMassive(Instance.NeedMouse, needMouse);
        }

        if(Instance.NeedMouse.Length > 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void _EditIcon(CursorIconInfo iconInfo, bool remove)
    {
        if (remove)
        {
            Icons = StaticTools.RemoveFromMassive(Icons, iconInfo);
        }
        else
        {
            Icons = StaticTools.ExcludingExpandMassive(Icons, iconInfo);
        }

        _UpdateIcon();
    }

    private void _UpdateIcon()
    {
        int high = 0;
        for(int i = 0; i < Icons.Length; i++)
        {
            if(Icons[high].Order <= Icons[i].Order)
            {
                high = i;
            }
        }

        Cursor.SetCursor(Icons[high].Texture, Vector2.zero, CursorMode.Auto);
    }

    public static void EditIcon(CursorIconInfo iconInfo, bool remove) => Instance._EditIcon(iconInfo, remove);

    public static void UpdateIcon() => Instance._UpdateIcon();
}

[System.Serializable]
public class CursorIconInfo
{
    public Texture2D Texture;
    public int Order;
}

public class NeedCursorOrder
{

}