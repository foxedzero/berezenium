using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FileSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Image Icon;
    [SerializeField] private Image Background;
    [SerializeField] private Text FileName;
    [SerializeField] private string Path;
    [SerializeField] private bool Directory;
    private UserFileOpen FileOpener = null;
    
    public RectTransform _RectTransform => RectTransform;

    public void SetInfo(UserFileOpen fileOpen, Sprite icon, string path, bool directory)
    {
        FileOpener = fileOpen;
        Path = path;
        Directory = directory;

        if (!directory)
        {
            string ext = System.IO.Path.GetExtension(path);

            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                Texture2D texture = new Texture2D(75, 75);
                texture.LoadImage(System.IO.File.ReadAllBytes(path));

                Icon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            }
            else
            {
                Icon.sprite = icon;
            }
        }
        else
        {
            Icon.sprite = icon;
        }

        if(Icon.sprite.rect.width > Icon.sprite.rect.height)
        {
            Icon.rectTransform.sizeDelta = new Vector2(Icon.sprite.rect.width, Icon.sprite.rect.height) / Icon.sprite.rect.width * 75;
        }
        else
        {
            Icon.rectTransform.sizeDelta = new Vector2(Icon.sprite.rect.width, Icon.sprite.rect.height) / Icon.sprite.rect.height * 75;
        }

        FileName.text = System.IO.Path.GetFileName(path);
    }

    public void SetFileName(string name) => FileName.text = name;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Background.color = new Color(0.1686275f, 0.3176471f, 0.6784314f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Background.color = new Color(0,0,0,0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FileOpener.Open(Path, Directory, true);
    }
}
