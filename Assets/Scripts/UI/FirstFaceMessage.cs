using UnityEngine;
using UnityEngine.UI;

public class FirstFaceMessage : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Text[] Texts;
    [SerializeField] private GameObject Square;
    [SerializeField] private float LifeTime;
    [SerializeField] FirstFaceMessenger FirstFaceMessenger = null;

    public RectTransform _RectTransform => RectTransform;

    private void OnDestroy()
    {
        FirstFaceMessenger.RemoveMessage(RectTransform);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    public void SetInfo(FirstFaceMessenger messenger, string[] info)
    {
        FirstFaceMessenger = messenger;
        if(info.Length == 1)
        {
            Texts[0].rectTransform.offsetMax = new Vector2(-5, -5);
            Square.SetActive(false);
            Texts[0].text = info[0];
        }
        else
        {
            Texts[0].text = info[0];
            Texts[1].text = info[1];
        }
    }

    private void Update()
    {
        LifeTime -= Time.unscaledDeltaTime;
        if (LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
