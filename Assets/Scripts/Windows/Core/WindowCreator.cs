using UnityEngine;
using WindowInterfaces;

public class WindowCreator : MonoBehaviour
{
    private static WindowCreator Instance = null;

    [SerializeField] private WindowLister Lister;
    [SerializeField] private GameObject[] Prefabs;
    [SerializeField] private RectTransform Content;

    private void Awake()
    {
        Instance = this;
    }

    public static T CreateWindow<T> () where T : Window
    {
        GameObject prefab = null;
       
        foreach(GameObject prefab1 in Instance.Prefabs)
        {
            if(prefab1.GetComponent<Window>().GetType() == typeof(T))
            {
                prefab = prefab1;
                break;
            }
        }

        T newWindow = Instantiate(prefab, Instance.Content.transform).GetComponent<T>();

        if (newWindow is ISingleOne)
        {
            foreach (Window window in Instance.Lister._Windows)
            {
                if (newWindow.GetType() == window.GetType())
                {
                    Destroy(newWindow.gameObject);

                    window.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    window.Select();

                    return window as T;
                }
            }
        }

        newWindow.SetLister(Instance.Lister);

        return newWindow;
    }

    public static Window CreateWindowWithType(string type)
    {
        Window newWindow = null;

        foreach(GameObject window in Instance.Prefabs)
        {
            if(window.GetComponent<Window>().GetType().ToString() == type)
            {
                newWindow = Instantiate(window, Instance.Content.transform).GetComponent<Window>();
                break;
            }
        }

        if(newWindow == null)
        {
            return null;
        }

        if(newWindow is ISingleOne)
        {
            foreach(Window window in Instance.Lister._Windows)
            {
                if(newWindow.GetType() == window.GetType())
                {
                    Destroy(newWindow.gameObject);

                    window.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    window.Select();

                    return window;
                }
            }
        }

        newWindow.SetLister(Instance.Lister);

        return null;
    }
}
