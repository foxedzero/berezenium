using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FestMessage : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private RectTransform Content;
    [SerializeField] private Text Label;
    [SerializeField] private Text Info;
    [SerializeField] private Musician.MusicOrder MusicOrder;

    public void SetInfo(string label, string info)
    {
        gameObject.SetActive(true);

        Info.text = info;

        Label.text = label;

        StartCoroutine(Resize());

        FindObjectOfType<Musician>().SetMusic(MusicOrder, false);
    }

    public void Return()
    {
        gameObject.SetActive(false);
        FindObjectOfType<Musician>().SetMusic(MusicOrder, true);
    }

    private IEnumerator Resize()
    {
        float width = Mathf.Clamp(Info.preferredWidth + 50, 250, 700);
        width = Mathf.Max(width, Label.preferredWidth);
        RectTransform.sizeDelta = new Vector2(width, 80);

        yield return new WaitForEndOfFrame();

        float height = Mathf.Clamp(Info.preferredHeight + 100, 250, 700);
        RectTransform.sizeDelta = new Vector2(width, height + 50);
        Content.sizeDelta = new Vector2(0, Mathf.Max(Info.preferredHeight, height - 115));

        gameObject.SetActive(true);
    }
}
