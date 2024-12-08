using UnityEngine;
using UnityEngine.UI;

public class CityMessenger : MonoBehaviour
{
    [SerializeField] private AudioSource MessageSound;

    [SerializeField] private IlusionHolders IlusionHolders;
    [SerializeField] private RectTransform Content;
    [SerializeField] private Text MessageList;
    [SerializeField] private FestMessage FestMessage;
    [SerializeField] private Musician Musician;
    [SerializeField] private Musician.MusicOrder MusicOrder;
    private CityMessage[] Messages = new CityMessage[0];

    public string[] _SaveInfo
    {
        get
        {
            string[] info = new string[0];
            for(int i = 0; i < Messages.Length; i++)
            {
                if (!Messages[i].Red)
                {
                    info = StaticTools.ExpandMassive(info, Messages[i]._SaveInfo);
                }
            }

            return info;
        }
        set
        {
            if(value == null)
            {
                return;
            }

            Messages = new CityMessage[value.Length];
            string[] splitted = null;

            for(int i =0; i < value.Length; i++)
            {
                splitted = value[i].Split(";");
                Messages[i] = new CityMessage(splitted[0], splitted[1]);
                if (splitted[2] == "1")
                {
                    Messages[i].Fest = true;
                }
            }

            UpdateInfo();
        }
    }

    private void Start()
    {
        IlusionHolders.SetInfo(null, Click);
    }

    public void SetMessage(CityMessage message, bool remove)
    {
        int index = StaticTools.IndexOf(Messages, message);

        if (remove)
        {
            if(index > -1)
            {
                Messages = StaticTools.ReduceMassive(Messages, index);
            }
        }
        else
        {
            if(index < 0)
            {
                if (message.Red)
                {
                    Messages = StaticTools.ExpandMassive(Messages, message, 0);
                }
                else
                {
                    Messages = StaticTools.ExpandMassive(Messages, message);

                    MessageSound.Play();
                }
            }
        }

        bool red = false;
        foreach(CityMessage cityMessage in Messages)
        {
            if (cityMessage.Red)
            {
                red = true;
                break;
            }
        }

        if (red)
        {
            Musician.SetMusic(MusicOrder, false);
        }
        else
        {
            Musician.SetMusic(MusicOrder, true);
        }

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        string info = "";
        for(int i= 0; i < Messages.Length; i++)
        {
            if (Messages[i].Red)
            {
                info += $"<color=red>{Messages[i].Label}</color>\n";
            }
            else
            {
                info += $"{Messages[i].Label}\n";
            }
        }

        if (info.EndsWith("\n"))
        {
            info = info.Remove(info.Length - 1);
        }

        MessageList.text = info;

        Content.sizeDelta = new Vector2(0, Messages.Length * 35);

        IlusionHolders._MaxIndex = Messages.Length;
    }

    public void Click(int index)
    {
        if (Messages.Length < 1 || index >= Messages.Length)
        {
            return;
        }

        if (Messages[index].Fest)
        {
            FestMessage.SetInfo(Messages[index].Label, Messages[index].Info);
        }
        else
        {
            UserInteract.AskMessage(Messages[index].Label, Messages[index].Info);
        }

        if (!Messages[index].Red)
        {
            Messages = StaticTools.ReduceMassive(Messages, index);

            UpdateInfo();
        }
    }

    public class CityMessage
    {
        public string Label;
        public string Info;
        public bool Red;
        public bool Fest;

        public string _SaveInfo
        {
            get
            {
                return $"{Label};{Info};{Fest}";
            }
        }

        public CityMessage(string label, string info, bool red = false) 
        {
            Label = label ;
            Info = info ;
            Red = red ;
        }
    }
}
