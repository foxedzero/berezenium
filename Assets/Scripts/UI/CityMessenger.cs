using UnityEngine;
using UnityEngine.UI;

public class CityMessenger : MonoBehaviour
{
    [SerializeField] private AudioSource MessageSound;

    [SerializeField] private FirstFaceMessenger FirstFaceMessenger;

    [SerializeField] private GameObject Panel;
    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private RectTransform Content;
    [SerializeField] private Text MessageList;
    [SerializeField] private Text CurrentMessage;
    [SerializeField] private Tipper Tipper;
    [SerializeField] private AudioClip[] MessageSounds;
    private CityMessage[] Messages = new CityMessage[0];

    private void Start()
    {
        IlusionHolders.SetInfo(null, Click);
    }

    public void ShowPanel()
    {
        if(Messages.Length == 0)
        {
            UserInteract.AskMessage("Нет сообщений", "Вы пока не получили никаких сообщений.");
            return;
        }
        Panel.SetActive(!Panel.activeSelf);
    }

    public void AddMessage(CityMessage message)
    {
        Messages = StaticTools.ExpandMassive(Messages, message, 0);

        FirstFaceMessenger.Show(new string[] { message.Label });

        MessageSound.clip = MessageSounds[Random.Range(0, MessageSounds.Length)];
        MessageSound.Play();
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if (Messages[0].Readed)
        {
            CurrentMessage.text = $"<{Messages[0].Label}>";
        }
        else
        {
            CurrentMessage.text = Messages[0].Label;
        }

        string info = "";
        int unread = 0;
        for(int i= 0; i < Messages.Length; i++)
        {
            if (Messages[i].Readed)
            {
                info += $"<{Messages[i].Label}>\n";
            }
            else
            {
                unread++;
                info += $"{Messages[i].Label}\n";
            }
        }

        if (info.EndsWith("\n"))
        {
            info = info.Remove(info.Length - 1);
        }

        MessageList.text = info;

        Content.sizeDelta = new Vector2(0, Messages.Length * 35);

        IlusionHolders._MaxIndex = Messages.Length - 1;

        Tipper._Info = $"{unread} непрочитанных сообщений\n\nНажмите, чтобы просмотреть все сообщения.";
    }

    public void Click(int index)
    {
        if (Messages.Length < 1 || index >= Messages.Length)
        {
            return;
        }

        UserInteract.AskMessage(Messages[index].Label, Messages[index].Info);

        Messages[index].Readed = true;
        UpdateInfo();
    }

    public class CityMessage
    {
        public string Label;
        public string Info;
        public bool Readed;

        public CityMessage(string label, string info) 
        {
            Label = label ;
            Info = info ;
        }
    }
}
