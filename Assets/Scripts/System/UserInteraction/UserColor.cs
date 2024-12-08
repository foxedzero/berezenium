using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserColor : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public delegate void ColorVoid(Color color);

    [SerializeField] protected RectTransform RectTransform;
    [SerializeField] private Image Pallete;
    [SerializeField] private Image ColorPreview;
    [SerializeField] private Text RText;
    [SerializeField] private Text GText;
    [SerializeField] private Text BText;
    [SerializeField] private Text HexText;

    [SerializeField] private float H;
    [SerializeField] private float S;
    [SerializeField] private float V;

    private Vector2 StartPosition = Vector2.zero;

    private ColorVoid Ask = null;

    public Color _Color
    {
        get
        {
            return Color.HSVToRGB(H, S, V);
        }
        set
        {
            Color.RGBToHSV(value, out H, out S, out V);

            UpdateInfo();
        }
    }
    public string _Hex
    {
        get
        {
            string info = "";
            string hex = "";

            Color rgb = Color.HSVToRGB(H, S, V);

            hex = StaticTools.IntToHex(Mathf.RoundToInt(rgb.r * 255));
            if(hex.Length < 2)
            {
                hex = "0" + hex;
            }

            info += hex;

            hex = StaticTools.IntToHex(Mathf.RoundToInt(rgb.g * 255));
            if (hex.Length < 2)
            {
                hex = "0" + hex;
            }

            info += hex;

            hex = StaticTools.IntToHex(Mathf.RoundToInt(rgb.b * 255));
            if (hex.Length < 2)
            {
                hex = "0" + hex;
            }

            info += hex;

            return info;
        }
        set
        {
            while(value.Length < 6)
            {
                value += "0";
            }

            Color rgb = new Color(StaticTools.HexToInt(value.Substring(0, 2)) / 255f, StaticTools.HexToInt(value.Substring(2, 2)) / 255f, StaticTools.HexToInt(value.Substring(4, 2)) / 255f);

            Color.RGBToHSV(rgb, out H, out S, out V);

            UpdateInfo();
        }
    }

    public void SetInfo(ColorVoid ask, Color startColor)
    {
        Ask = ask;
        _Color = startColor;

        Vector2 position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);

        if (position.x + RectTransform.sizeDelta.x / 2 > 1920)
        {
            position.x -= RectTransform.sizeDelta.x;
        }

        if (position.y - RectTransform.sizeDelta.y / 2 < 0)
        {
            position.y += RectTransform.sizeDelta.y;
        }

        RectTransform.anchoredPosition = position;
    }

    public void Return()
    {
        if(Ask != null)
        {
            Ask.Invoke(_Color);
        }

        Destroy(gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

    private void UpdateInfo()
    {
        Color color = _Color;
        ColorPreview.color = color;

        RText.text = Mathf.RoundToInt(color.r * 255).ToString();
        GText.text = Mathf.RoundToInt(color.g * 255).ToString();
        BText.text = Mathf.RoundToInt(color.b * 255).ToString();
        HexText.text = _Hex;

        Pallete.color = Color.HSVToRGB(H, 1, 1);
    }

    public void SetH(float value)
    {
        H = value;

        float h;
        float s;
        float v;

        Color.RGBToHSV(_Color, out h, out s, out v);

        UpdateInfo();
    }
    public void SetSV(float Ss, float Vv)
    {
        S = Ss;
        V = Vv;

        float h;
        float s;
        float v;

        Color.RGBToHSV(_Color, out h, out s, out v);

        UpdateInfo();
    }

    public void SetR(string value)
    {
        Color rgb = _Color;
        rgb.r = StaticTools.StringToInt(value) / 255f;

        _Color = rgb;

        UpdateInfo();
    }
    public void SetG(string value)
    {
        Color rgb = _Color;
        rgb.g = StaticTools.StringToInt(value) / 255f;

        _Color = rgb;

        UpdateInfo();
    }
    public void SetB(string value)
    {
        Color rgb = _Color;
        rgb.b = StaticTools.StringToInt(value) / 255f;

        _Color = rgb;

        UpdateInfo();
    }
    public void SetHex(string value) => _Hex = value;

    public void EditParameter(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskInput("", SetR);
                break;
            case 1:
                UserInteract.AskInput("", SetG);
                break;
            case 2:
                UserInteract.AskInput("", SetB);
                break;
            case 3:
                UserInteract.AskInput("", SetHex);
                break;
        }
    }


    protected Vector2 CalculatePosition()
    {
        Vector2 viewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        return new Vector2(1920 * viewPort.x, StaticTools.ScreenHeight * viewPort.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartPosition = RectTransform.anchoredPosition;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform.anchoredPosition = StartPosition + eventData.position - eventData.pressPosition;
    }
}
