using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TipManager : MonoBehaviour
{
    [SerializeField] private RectTransform Panel;
    [SerializeField] private Animator Animator;
    [SerializeField] private Text Text;

    [SerializeField] private Tipper Tipper;

    private static TipManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    public static void ShowTip(Tipper tipper, bool remove)
    {
        if (remove)
        {
            if(Instance.Tipper == tipper)
            {
                Instance.Tipper = null;

                Instance.UpdateInfo();
            }

            tipper.OnInfoChange -= Instance.UpdateText;
        }
        else
        {
            if (Instance.Tipper != tipper)
            {
                Instance.Tipper = tipper;
                tipper.OnInfoChange += Instance.UpdateText;

                Instance.UpdateInfo();
            }
        }
    }

    private void UpdateInfo()
    {
        if (Tipper == null)
        {
            Text.text = "";
            Panel.gameObject.SetActive(false);
        }
        else
        {
            Text.text = Tipper._Info;
            Panel.gameObject.SetActive(true);

           Animator.Play("Fade");

            StartCoroutine(Resize());
        }
    }

    public void UpdateText()
    {
        if (Tipper == null)
        {
            Text.text = "";
        }
        else
        {
            Text.text = Tipper._Info;

            Panel.sizeDelta = new Vector2(Mathf.Clamp(Text.preferredWidth, 25, 450), 50);

            Panel.sizeDelta = new Vector2(Panel.sizeDelta.x, Text.preferredHeight);
        }
    }

    protected Vector2 CalculatePosition()
    {
        Vector2 viewPort = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        return new Vector2(1920 * viewPort.x, StaticTools.ScreenHeight * viewPort.y);
    }

    private IEnumerator Resize()
    {
        Panel.sizeDelta = new Vector2(Mathf.Clamp(Text.preferredWidth, 25, 600), 50);

        Panel.sizeDelta = new Vector2(Panel.sizeDelta.x, Text.preferredHeight);

        yield return new WaitForSecondsRealtime(0.5f);

        Vector2 position = CalculatePosition() + new Vector2(Panel.sizeDelta.x / 2 + 25, -Panel.sizeDelta.y / 2 - 25);

        if (position.x + Panel.sizeDelta.x / 2 + 12.5f > 1920)
        {
            position.x -= Panel.sizeDelta.x + 25;
        }

        if (position.y - Panel.sizeDelta.y / 2 - 12.5f < 0)
        {
            position.y += Panel.sizeDelta.y + 25;
        }

        Panel.anchoredPosition = position;
    }
}
