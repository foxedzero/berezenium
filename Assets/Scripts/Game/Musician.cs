using UnityEngine;
using System.Collections;

public class Musician : MonoBehaviour
{
    [SerializeField] private AudioSource Source;

    [SerializeField] private MusicOrder FirstMusic;

    private MusicOrder[] Orders = new MusicOrder[0];
    private AudioClip CurrentMusic = null;

    private void Start()
    {
        SetMusic(FirstMusic, false);
    }

    public void SetMusic(MusicOrder order, bool remove)
    {
        int index = StaticTools.IndexOf(Orders, order);
        if (remove)
        {
            if(index > -1)
            {
                Orders = StaticTools.ReduceMassive(Orders, index);
            }
        }
        else
        {
            if(index < 0)
            {
                Orders = StaticTools.ExpandMassive(Orders, order);
            }
        }

        if(Orders.Length < 1)
        {
            return;
        }

        index = 0;
        for(int i = 0; i < Orders.Length; i++)
        {
            if (Orders[i].Priority > Orders[index].Priority)
            {
                index = i;
            }
        }

        if(Orders[index].Music != CurrentMusic)
        {
            CurrentMusic = Orders[index].Music;
            StopAllCoroutines();
            StartCoroutine(ChangeMusic());
        }
    }

    private IEnumerator ChangeMusic()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while(Source.volume > 0)
        {
            Source.volume -= Time.unscaledDeltaTime;
            yield return waitForEndOfFrame;
        }

        Source.clip = CurrentMusic;

        Source.Play();

        while (Source.volume < 1)
        {
            Source.volume += Time.unscaledDeltaTime;
            yield return waitForEndOfFrame;
        }

        Source.volume = 1;
    }

    [System.Serializable]
    public class MusicOrder
    {
        public AudioClip Music;
        public int Priority;
    }
}
