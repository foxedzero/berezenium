using UnityEngine;
using UnityEngine.UI;

public class UserInput : UserAction
{
    [SerializeField] private InputField Field;
    private bool ReturnOnFail = true;
    private float StandartWidth = 0;

    public delegate void Delegate(string value);
    private Delegate ToReturn;

    public SimpleVoid OnTextEdit = null;

    public string _Value 
    {
        get
        {
            return Field.text;
        }
        set
        {
            Field.SetTextWithoutNotify(value);

            TextChanged();
        }
    }
    public bool _ReturnOnFail
    {
        get
        {
            return ReturnOnFail;
        }
        set
        {
            ReturnOnFail = value;
        }
    }

    public void SetInfo(Delegate toReturn, string label)
    {
        ToReturn = toReturn;

        float width = 250;

        Label.text = label;

        if (label.Length > 0)
        {
            if (Label.preferredWidth + 50 > width)
            {
                width = Label.preferredWidth + 50;
            }
        }

        if (toReturn != null)
        {
            if (label.Length > 0)
            {
                RectTransform.sizeDelta = new Vector2(width, 80);
            }
            else
            {
                RectTransform.sizeDelta = new Vector2(width, 50);
            }

            Field.ActivateInputField();
        }
        else
        {
            RectTransform.sizeDelta = new Vector2(width, 50);

            Field.gameObject.SetActive(false);
        }

        StandartWidth = width;

        Vector2 position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);

        if (position.x + width / 2 > 1920)
        {
            position.x -= width;
        }

        if (position.y - RectTransform.sizeDelta.y / 2 < 0)
        {
            position.y += RectTransform.sizeDelta.y;
        }

        RectTransform.anchoredPosition = position;
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
           ToReturn.Invoke(Field.text);

            Destroy(gameObject);
        }

        base.Update();
    }

    public void TextChanged()
    {
        float width = Field.preferredWidth + 50;

        if (width > StandartWidth)
        {
            if (RectTransform.anchoredPosition.x - width / 2 < 0)
            {
                RectTransform.anchoredPosition = new Vector2(width / 2, RectTransform.anchoredPosition.y);
            }
            else if (RectTransform.anchoredPosition.x + width / 2 > 1920)
            {
                RectTransform.anchoredPosition = new Vector2(1920 - width / 2, RectTransform.anchoredPosition.y);
            }

            RectTransform.sizeDelta = new Vector2(width, RectTransform.sizeDelta.y);
        }
        else
        {
            RectTransform.sizeDelta = new Vector2(StandartWidth, RectTransform.sizeDelta.y);
        }

        if (OnTextEdit != null)
        {
            OnTextEdit.Invoke();
        }
    }
}
