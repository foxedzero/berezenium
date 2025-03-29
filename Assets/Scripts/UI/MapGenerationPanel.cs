using UnityEngine;
using UnityEngine.UI;

public class MapGenerationPanel : MonoBehaviour
{
    [SerializeField] private MainMenu MainMenu;

    [SerializeField] private InputField Seed;
    [SerializeField] private Text Size;
    [SerializeField] private Toggle StartBonus;

    [SerializeField] private string MapSeed;
    [SerializeField] private int MapSize;

    public string _Seed => MapSeed;
    public int _Size => MapSize;
    public bool _StartBonus => StartBonus.isOn;

    private void OnEnable()
    {
        Seed.SetTextWithoutNotify($"{Random.Range(1000, 100000)}");
    }

    public void CheckSeed()
    {
        if(Seed.text.Length <= 0)
        {
            Seed.SetTextWithoutNotify($"{Random.Range(1000, 100000)}");
        }
        else if(Seed.text.Length > 30)
        {
            Seed.SetTextWithoutNotify(Seed.text.Substring(0, 30));
        }
    }

    public void SetSize()
    {
        UserInteract.AskVariants("Размер локации", new string[] {"Малый", "Средний", "Большой"}, new int[] {0, 1, 2}, SetSize);
    }
    public void SetSize(int index)
    {
        MapSize = index;
        switch (index)
        {
            case 0:
                Size.text = $"Малый";
                break;
            case 1:
                Size.text = $"Средний";
                break;
            case 2:
                Size.text = $"Большой";
                break;
        }
    }

    public void Answer(bool state)
    {
        if (state)
        {
            MapSeed = Seed.text;
        }
        else
        {
            MapSeed = "";
        }

        MainMenu.AnswerSeed(state);
    }
}
