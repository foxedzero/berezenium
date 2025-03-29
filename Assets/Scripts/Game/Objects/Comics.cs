using UnityEngine;

public class Comics : MonoBehaviour, IInteractable
{
    [SerializeField] private Outline Indicator;

    [SerializeField] private Transform Canvas;
    [SerializeField] private GameObject ComicsPrefab;
    private GameObject ComicsInstance = null;

    public bool _AbstractUse => true;

    public string _Info => $"Прочесть комикс";

    public void Indicate(bool state) => Indicator.enabled = state;

    public void Interact()
    {
        UserInteract.AskVariants("", new string[] {"Открыть комикс", "Первоисточник (откроется браузер)", "Отмена"}, new int[] { 0, 1, 2}, RightMouseAction);
    }

    public void RightMouseAction(int index)
    {
        switch (index)
        {
            case 0:
                if(ComicsInstance == null)
                {
                    ComicsInstance = Instantiate(ComicsPrefab, Canvas);
                }
                else
                {
                    Destroy(ComicsInstance);
                }
                break;
            case 1:
                Application.OpenURL("https://vk.com/@berloga_nkfp-komiksy-o-mire-berlogi");
                break;
        }
    }
}
