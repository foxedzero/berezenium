using UnityEngine;

public class FirstFaceMessenger : MonoBehaviour
{
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private RectTransform Content;
    private RectTransform[] Messages = new RectTransform[0];

    public void RemoveMessage(RectTransform message)
    {
        Messages = StaticTools.RemoveFromMassive(Messages, message);
    }

    public void Show(string[] info)
    {
        FirstFaceMessage message = Instantiate(MessagePrefab, Content).GetComponent<FirstFaceMessage>();
        message.SetInfo(this, info);

        Messages = StaticTools.ExpandMassive(Messages, message._RectTransform, 0);

        PlaceMessages();
    }

    private void PlaceMessages()
    {
        for(int i = 0; i < Messages.Length; i++)
        {
            Messages[i].anchoredPosition = new Vector2(0, 32.5f + 75 * i);
        }
    }
}
