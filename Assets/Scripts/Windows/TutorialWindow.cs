using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;

public class TutorialWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private Text Info;
    [SerializeField] private Image Illustration;

    [SerializeField] private Page[] Pages;
    [SerializeField] private int CurrentPage;

    public override string _Label => "Окно обучения";

    public override void SetLister(WindowLister lister)
    {
        base.SetLister(lister);

        transform.parent = Lister._Canvas.transform;

        RectTransform.anchoredPosition = new Vector2(-735, StaticTools.ScreenHeight / 2 - 350);

        SetPage(PlayerPrefs.GetInt("Tutorial", 0));
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Return))
        {
            NextPage();
        }
    }

    public void NextPage()
    {
        SetPage(Mathf.Min(CurrentPage + 1, Pages.Length - 1));
    }
    public void BackPage()
    {
        SetPage(Mathf.Max(CurrentPage - 1, 0));
    }

    public void SetPage(int page)
    {
        CurrentPage = page;
        PlayerPrefs.SetInt("Tutorial", CurrentPage);
        PlayerPrefs.Save();

        Label.text = Pages[page].Title;
        Info.text = Pages[page].Info;
        Illustration.sprite = Pages[page].Illustration;
    }

    public override void RightMouse()
    {
        string[] variants = new string[2 + Pages.Length];
        variants[0] = $"{(Pinned ? "открепить" : "закрепить")} окно";
        variants[1] = $"закрыть окно";
        int[] indexes = new int[variants.Length];
        indexes[0] = -2;
        indexes[1] = -1;

        for(int i = 0; i < Pages.Length; i++)
        {
            variants[i + 2] = $"{i + 1}. {Pages[i].Title}";
            indexes[i + 2] = i;
        }
        UserInteract.AskVariants(_Label, variants, indexes, RightMouseActions);
    }

    public override void RightMouseActions(int index)
    {
        switch (index)
        {
            case -2:
                Pinned = !Pinned;
                break;
            case -1:
                Close();
                break;
            default:
                SetPage(index);
                break;
        }
    }

    [System.Serializable]
    public class Page
    {
       public string Title;
         [Multiline] public string Info;
        public Sprite Illustration;
    }
}
