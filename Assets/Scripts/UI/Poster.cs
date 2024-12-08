using UnityEngine;

public class Poster : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Settings Settings;

    private void Start()
    {
        Settings.OnChanges += UpdateSize;
    }

    private void OnDestroy()
    {
        Settings.OnChanges -= UpdateSize;
    }

    public void UpdateSize()
    {
        RectTransform.sizeDelta = new Vector2(StaticTools.ScreenHeight / 9 * 16, StaticTools.ScreenHeight);
    }
}
