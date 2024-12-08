using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.EventSystems;

public class UserFileOpen : MonoBehaviour
{
    [SerializeField] private GameObject FileSlotPrefab;
    [SerializeField] private GameObject PathSlotPrefab;
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private RectTransform Content;
    [SerializeField] private RectTransform Field;
    [SerializeField] private Text Label;
    [SerializeField] private Sprite[] Sprites;
    [SerializeField] private FileSlot[] Slots;
    [SerializeField] private PathSlot[] PathSlots;
    [SerializeField] private string[] History;
    [SerializeField] private int Position;
    private Delegate ToReturn = null;
    private string[] Filters = new string[0];

    private Vector2 StartPosition = Vector2.zero;

    public delegate void Delegate(string path);

    private Vector2 CalculatePosition()
    {
        Vector2 viewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        return new Vector2(1920 * viewPort.x, StaticTools.ScreenHeight * viewPort.y);
    }

    public void Close() => Destroy(gameObject);

    public void SetInfo(Delegate toReturn, string[] filters, string label)
    {
        Filters = filters;
        ToReturn = toReturn;
        Label.text = label;

        History = new string[] {"pc"};
        Open("pc", true, false);

        Vector2 position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);

        if (position.x + RectTransform.sizeDelta.x / 2 > 1920)
        {
            position.x -= RectTransform.sizeDelta.x;
        }

        RectTransform.anchoredPosition = position;
    }

    public void AskPath()
    {
        UserInput input = UserInteract.AskInput("путь", EnterPath);

        input._Value = History[Position];

        input._ReturnOnFail = false;
    }

    public void EnterPath(string path)
    {
        if(path == "" || path == "pc")
        {
            History = new string[] { "pc" };
            Position = 0;
            Open("pc", true, false);
            return;
        }
        else
        {
            if (!Path.IsPathRooted(path))
            {
                return;
            }

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                return;
            }
        }

        History = new string[] {path};
        string pathCopy = path;
        while(pathCopy != null)
        {
            pathCopy = Path.GetDirectoryName(pathCopy);

            if(pathCopy == null)
            {
                break;
            }
            else
            {
                History = StaticTools.ExpandMassive(History, pathCopy, 0);
            }
        }

        History = StaticTools.ExpandMassive(History, "pc", 0);

        Position = History.Length - 1;

        if (Directory.Exists(path))
        {
            Open(path, true, false);
        }
        else
        {
            Open(path, false, false);
        }
    }

    public void Open(string path, bool directory, bool history)
    {
        foreach(FileSlot slot in Slots)
        {
            Destroy(slot.gameObject);
        }

        if (history)
        {
            int index = StaticTools.IndexOf(History, path);
            if(index > -1)
            {
                Position = index;

                if (Position < History.Length)
                {
                    while (History.Length - 1 > Position)
                    {
                        History = StaticTools.ReduceMassive(History, History.Length - 1);
                    }
                }
            }
            else
            {
                if (Position < History.Length)
                {
                    while (History.Length - 1 > Position)
                    {
                        History = StaticTools.ReduceMassive(History, History.Length - 1);
                    }
                }

                History = StaticTools.ExpandMassive(History, path);
                Position++;
            }
        }

        if (directory)
        {
            if(path == "pc")
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                Slots = new FileSlot[drives.Length];
                for(int i = 0; i < drives.Length; i++)
                {
                    FileSlot slot = Instantiate(FileSlotPrefab, Content).GetComponent<FileSlot>();
                    slot.SetInfo(this, Sprites[0], drives[i].RootDirectory.FullName, true);
                    slot.SetFileName(drives[i].RootDirectory.FullName);
                    Slots[i] = slot;
                }
            }
            else if(Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                string[] files = new string[0];

                if(Filters.Length > 0)
                {
                    foreach(string filter in Filters)
                    {
                        files = StaticTools.ExpandMassive(files, Directory.GetFiles(path, filter));
                    }
                }
                else
                {
                    files = Directory.GetFiles(path);
                }

                Slots = new FileSlot[directories.Length + files.Length];
                for(int i = 0; i < directories.Length; i++)
                {
                    FileSlot slot = Instantiate(FileSlotPrefab, Content).GetComponent<FileSlot>();
                    slot.SetInfo(this, Sprites[0], directories[i], true);
                    Slots[i] = slot;
                }
                for (int i = 0; i < files.Length; i++)
                {
                    FileSlot slot = Instantiate(FileSlotPrefab, Content).GetComponent<FileSlot>();
                    slot.SetInfo(this, Sprites[1], files[i], false);
                    Slots[i + directories.Length] = slot;
                }
            }

            PlaceSlots();
        }
        else
        {
            ToReturn.Invoke(path);
            Destroy(gameObject);
        }
    }

    private void PlaceSlots()
    {
        int x = 75;
        int y = 75;

        foreach(FileSlot slot in Slots)
        {
            slot._RectTransform.anchoredPosition = new Vector2(x, -y);
            x += 110;

            if(x > 600)
            {
                x = 75;
                y += 110;
            }
        }

        Content.sizeDelta = new Vector2(0, y + 100);
        Content.anchoredPosition = new Vector2(0, 0);

        foreach(PathSlot slot in PathSlots)
        {
            Destroy(slot.gameObject);
        }
        PathSlots = new PathSlot[0];

        x = 0;
        for(int i = Position - 4; i <= Position; i++)
        {
            if(i >= 0)
            {
                PathSlot slot = Instantiate(PathSlotPrefab, Field.transform).GetComponent<PathSlot>();

                slot.SetInfo(this, History[i]);
                PathSlots = StaticTools.ExpandMassive(PathSlots, slot);
            }
        }

        StartCoroutine(UpdatePathSlotsPosition());
    }

    public void Move(bool back)
    {
        if (back)
        {
            if(Position > 0)
            {
                Position--;
                Open(History[Position], true, false);
            }
        }
        else
        {
            if(Position + 1 < History.Length)
            {
                Position++;
                Open(History[Position], true, false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartPosition = RectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform.anchoredPosition = StartPosition + eventData.position - eventData.pressPosition;
    }

    private IEnumerator UpdatePathSlotsPosition()
    {
        yield return new WaitForEndOfFrame();

        float x = 0;
        foreach (PathSlot slot in PathSlots)
        {
            x += slot._Width / 2;

            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);

            x += slot._Width / 2;
        }
    }
}
