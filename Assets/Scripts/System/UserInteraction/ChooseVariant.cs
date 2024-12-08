using UnityEngine;
using System.Collections;

public class ChooseVariant : UserAction
{
    [SerializeField] private VariantChosed Delegate;
    [SerializeField] private RectTransform List;
    [SerializeField] private GameObject ContentPrefab;
    [SerializeField] private GameObject VariantPrefab;
    [SerializeField] private int Current = -1;
    private Variant[] Variants = new Variant[0];

    public delegate void VariantChosed(int index);

    public void Select(int index)
    {
        Delegate.Invoke(index);

        Destroy(gameObject);
    }

    public void SetInfo(string label, string[] variants, int[] variantIndexes, VariantChosed method)
    {
        if(variants == null || variants.Length == 0)
        {
            Debug.LogError($"вариантов нет");
            Destroy(gameObject);
            return;
        }
        if(variants.Length != variantIndexes.Length)
        {
            Debug.LogError($"Количество имен и индексов не совпадает <color=white>{variants.Length} != {variantIndexes.Length}</color>");
            Destroy(gameObject);
            return;
        }

        Delegate = method;

        Label.text = label;

        RectTransform content = Instantiate(ContentPrefab, List).GetComponent<RectTransform>();

        RectTransform[] contents = new RectTransform[1] {content};
        float[] widthes = new float[0];

        bool sectored = false;
        float width = 150;
        float yPosition = 0;

        if(label.Length > 0)
        {
            width = Label.preferredWidth + 30;
            yPosition += 40;
        }
        else
        {
            Label.gameObject.SetActive(false);
        }

        Variants = new Variant[variants.Length];
        if(variants.Length > 1)
        {
            for (int i = 0; i < variants.Length; i++)
            {
                Variant newVariant = Instantiate(VariantPrefab, content).GetComponent<Variant>();

                Variants[i] = newVariant;

                yPosition += 20;

                newVariant.SetInfo(this, variants[i], variantIndexes[i], i, -yPosition);

                if (newVariant._Width > width)
                {
                    width = newVariant._Width;
                }

                yPosition += 20;

                if (yPosition >= StaticTools.ScreenHeight - 50)
                {
                    content.sizeDelta = new Vector2(width, StaticTools.ScreenHeight);

                    widthes = StaticTools.ExpandMassive(widthes, width);

                    width = 150;
                    yPosition = 0;

                    content = Instantiate(ContentPrefab, List).GetComponent<RectTransform>();

                    contents = StaticTools.ExpandMassive(contents, content);

                    sectored = true;
                }
            }
        }
        else
        {
            Variant newVariant = Instantiate(VariantPrefab, content).GetComponent<Variant>();

            Variants[0] = newVariant;

            yPosition += 20;

            newVariant.SetInfo(this, variants[0], variantIndexes[0], 0, -yPosition);

            if (newVariant._Width > width)
            {
                width = newVariant._Width;
            }

            yPosition += 20;
        }

        widthes = StaticTools.ExpandMassive(widthes, width);

        content.sizeDelta = new Vector2(width, yPosition);

        Vector2 position = Vector2.zero;

        if (sectored)
        {
            RectTransform.sizeDelta = new Vector2(StaticTools.Summ(widthes), StaticTools.ScreenHeight - 50);

            position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);

            float xPosition = 0;
            for (int i = 0; i < contents.Length - 1; i++)
            {
                xPosition += widthes[i] / 2;

                contents[i].anchoredPosition = new Vector2(xPosition, -25);

                xPosition += widthes[i] / 2;
            }

            content.anchoredPosition = new Vector2(xPosition + width / 2, (StaticTools.ScreenHeight - yPosition) / 2 - 25);

            position.y = StaticTools.ScreenHeight / 2;
        }
        else
        {
            RectTransform.sizeDelta = new Vector2(width, yPosition);

            position = CalculatePosition() + new Vector2(RectTransform.sizeDelta.x / 2, -RectTransform.sizeDelta.y / 2);

            content.anchoredPosition = new Vector2(width / 2, 0);

            if (position.y - yPosition / 2 < 0)
            {
                if (position.y + RectTransform.sizeDelta.y * 1.5f > Screen.height)
                {
                    position.y -= position.y - RectTransform.sizeDelta.y / 2;
                }
                else
                {
                    position.y += RectTransform.sizeDelta.y;
                }
            }
        }

        if (position.x + RectTransform.sizeDelta.x / 2 > 1920)
        {
            if (position.x - RectTransform.sizeDelta.x * 1.5f < 0)
            {
                position.x -= (position.x + RectTransform.sizeDelta.x / 2) - 1920;
            }
            else
            {
                position.x -= RectTransform.sizeDelta.x;
            }
        }

        RectTransform.anchoredPosition = position;
    }

    public void SetCurrent(int index)
    {
        foreach(Variant variant in Variants)
        {
            variant.HighLight(false);
        }

        Variants[index].HighLight(true);

        Current = index;
    }

    private IEnumerator MoveCurrent(bool down)
    {
        int times = 0;

        if (down)
        {
            if (Current + 1 < Variants.Length)
            {
                SetCurrent(Current + 1);
            }

            while (Input.GetKey(KeyCode.DownArrow))
            {
                times++;

                if(times > 11)
                {
                    if (Current + 1 < Variants.Length)
                    {
                        SetCurrent(Current + 1);
                    }
                    else
                    {
                        yield break;
                    }
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            if (Current - 1 >= 0)
            {
                SetCurrent(Current - 1);
            }

            while (Input.GetKey(KeyCode.UpArrow))
            {
                times++;

                if (times > 11)
                {
                    if (Current - 1 >= 0)
                    {
                        SetCurrent(Current - 1);
                    }
                    else
                    {
                        yield break;
                    }
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
