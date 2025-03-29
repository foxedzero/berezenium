using UnityEngine;

public class AutorsLink : MonoBehaviour
{
    [SerializeField] private string Link;

    public void OpenLink()
    {
        Application.OpenURL(Link);
    }
}
