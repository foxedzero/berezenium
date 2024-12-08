using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserConfirm : UserAction
{
    [SerializeField] private RectTransform Content;
    [SerializeField] private Text Info;

    private BoolAnswer ToReturn = null;

    public delegate void BoolAnswer(bool state);

    public void SetInfo(string label, BoolAnswer ask, string info)
    {
        ToReturn = ask;

        Label.text = label;
        Info.text = info;
        
        StartCoroutine(Resize());
    }

    public void Answer(bool state)
    {
        ToReturn.Invoke(state);

        Destroy(gameObject);
    }

    private IEnumerator Resize()
    {
        float width = Mathf.Clamp(Info.preferredWidth + 50, 400, 700);
        width = Mathf.Max(width, Label.preferredWidth);
        RectTransform.sizeDelta = new Vector2(width, 80);

        Vector2 position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);
        RectTransform.anchoredPosition = position;

        yield return new WaitForEndOfFrame();

        float height = Mathf.Clamp(Info.preferredHeight + 100, 250, 700);
        RectTransform.sizeDelta = new Vector2(width, height + 50);
        Content.sizeDelta = new Vector2(0, Mathf.Max(Info.preferredHeight, height - 50));

        position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);

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
}
