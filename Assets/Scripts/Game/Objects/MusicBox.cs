using UnityEngine;

public class MusicBox : MonoBehaviour, IInteractable
{
    private Musician.MusicOrder MusicOrder = new Musician.MusicOrder();

    private void OnDisable()
    {
        Musician musician = FindObjectOfType<Musician>();

        musician.SetMusic(MusicOrder, true);
    }

    public void Interact()
    {
        ShowMusic();
    }

    public void ShowMusic()
    {
        string[] variants = new string[]
        {
            "Выключить",
            "WetGes (альбом - Поток бирюзы)",
            "Triumph of Humanity (альбом - Pianofilia)",
            "Nightmare Existing (альбом - Pianofilia)",
            "Pir (альбом - Pianofilia)",
            "Pianofilia (альбом - Pianofilia)",
            "Diesaw (альбом - Pianofilia)",
            "Wampirity end (альбом - Pianofilia)",
            "Hunger (альбом - Aimneko)",
            "Off Security (альбом - Aimneko)",
            "Lust (альбом - Aimneko)",
            "Greed (альбом - Aimneko)",
            "Mad Scientist (альбом - Aimneko)",
            "Melodyart (альбом - Aimneko)",
            "TheFalseFaith (альбом - Aimneko)",
            "ArtExaminator (альбом - Aimneko)",
            "Hotel Constantin (альбом - Hotel Constantin)",
            "Deltvora1 (альбом - Deltvora)",
            "Deltvora2 end (альбом - Deltvora)",
            "Deltvora3 end (альбом - Deltvora)",
            "Fairy Invasioner (альбом - Fairy Invasioner)",
            "rock1 (прочее)",
            "lastsolo (прочее)"
        };

        UserInteract.AskVariants("Музыкальная колонка", variants, StaticTools.Range(variants.Length), SetMusic);
    }
    public void SetMusic(int index)
    {
        Musician musician = FindObjectOfType<Musician>();

        MusicOrder.Priority = 10;
        musician.SetMusic(MusicOrder, true);

        switch (index)
        {
            case 0:
                break;
            case 1:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/wetGes");
                break;
            case 2:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/triumph of humanity");
                break;
            case 3:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/nightmare existing");
                break;
            case 4:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/pir");
                break;
            case 5:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/pianofilia");
                break;
            case 6:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/diesaw");
                break;
            case 7:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/wampirity end");
                break;
            case 8:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Hunger");
                break;
            case 9:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Off Security");
                break;
            case 10:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Lust");
                break;
            case 11:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Greed");
                break;
            case 12:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Mad Scientist");
                break;
            case 13:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Melodyart");
                break;
            case 14:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/TheFalseFaith");
                break;
            case 15:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/ArtExaminator");
                break;
            case 16:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Hotel Konstantin");
                break;
            case 17:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Deltvor");
                break;
            case 18:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Deltvor2");
                break;
            case 19:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/Deltvora_3");
                break;
            case 20:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/fairy invasion");
                break;
            case 21:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/rock1");
                break;
            case 22:
                MusicOrder.Music = Resources.Load<AudioClip>("ZtiMaratOsts/lastsolo");
                break;
        }

        if (index != 0)
        {
            musician.SetMusic(MusicOrder, false);
        }
    }
}
