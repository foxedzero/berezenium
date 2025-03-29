using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class ActBar : MonoBehaviour
{
    [SerializeField] private RectTransform Normal;
    [SerializeField] private RectTransform Nice;
    [SerializeField] private RectTransform Current;

    [SerializeField] private Animator CurrentAnimator;

    [SerializeField] private float Value;
    [SerializeField] private float Speed;
    [SerializeField] private bool Direction = true;
    [SerializeField] private bool Active = false;

    public void Activate(float hard, float kritSize)
    {
        gameObject.SetActive(true);

        CurrentAnimator.Play("Start");

        Speed = 2;

        if (hard <= 0)
        {
            Normal.sizeDelta = new Vector2(800, 0);
            Normal.anchoredPosition = new Vector2 (0, 0);
        }
        else
        {
            if(hard > 4)
            {
                Normal.sizeDelta = new Vector2(15, 0);
            }
            else if(hard > 2)
            {
                Normal.sizeDelta = new Vector2(Mathf.Lerp(15, 40, (4 - hard) / 2), 0);
            }
            else if (hard > 1)
            {
                Normal.sizeDelta = new Vector2(Mathf.Lerp(40, 100, 2 - hard), 0);
            }
            else if (hard > 0.5)
            {
                Normal.sizeDelta = new Vector2(Mathf.Lerp(100, 250, (1 - hard) * 2f), 0);
            }
            else
            {
                Normal.sizeDelta = new Vector2(Mathf.Lerp(250, 800, (0.5f - hard) * 2f), 0);
            }
        }

        Normal.anchoredPosition = new Vector2(Random.Range(Normal.sizeDelta.x / 2f, 800 - Normal.sizeDelta.x / 2f), 0);

        Nice.sizeDelta = new Vector2(kritSize, 0);
        Nice.anchoredPosition = new Vector2(Random.Range(Normal.anchoredPosition.x - Normal.sizeDelta.x/2 + kritSize/2f, Normal.anchoredPosition.x + Normal.sizeDelta.x / 2 - kritSize / 2f), 0);

        Active = true;
    }

    private void Update()
    {
        if (!Active)
        {
            return;
        }

        if (Direction)
        {
            Value += Time.unscaledDeltaTime * Speed;
            if(Value > 1)
            {
                Value = 1;
                Direction = !Direction;

                Speed = Mathf.Clamp(Speed - 0.05f, 1f, 2);
            }
        }
        else
        {
            Value -= Time.unscaledDeltaTime * Speed;
            if (Value < 0)
            {
                Value = 0;
                Direction = !Direction;

                Speed = Mathf.Clamp(Speed - 0.05f, 1f, 2);
            }
        }

        Current.anchoredPosition = new Vector2(800 * Value, 0);
    }

    public int Stop()
    {
        Active = false;

        StopAllCoroutines();
        StartCoroutine(Exit());

        if(Value * 800 > Nice.anchoredPosition.x- Nice.sizeDelta.x / 2f && Value * 800 < Nice.anchoredPosition.x + Nice.sizeDelta.x / 2f)
        {
            CurrentAnimator.Play("2");
            return 2;
        }
        else if(Value * 800 > Normal.anchoredPosition.x - Normal.sizeDelta.x/2 && Value * 800 < Normal.anchoredPosition.x + Normal.sizeDelta.x / 2)
        {
            CurrentAnimator.Play("1");
            return 1;
        }

        CurrentAnimator.Play("0");
        return 0;
    }

    private IEnumerator Exit()
    {
        yield return new WaitForSecondsRealtime(1);

        if (!Active)
        {
            gameObject.SetActive(false);
        }
    }
}
