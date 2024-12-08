using UnityEngine;
using WindowInterfaces;

public class WindowCreator : MonoBehaviour
{
    [SerializeField] private WindowLister Lister;
    [SerializeField] private GameObject[] Prefabs;

    public T CreateWindow<T> () where T : Window
    {
        GameObject prefab = null;
       
        foreach(GameObject prefab1 in Prefabs)
        {
            if(prefab1.GetComponent<Window>().GetType() == typeof(T))
            {
                prefab = prefab1;
                break;
            }
        }

        T newWindow = Instantiate(prefab, transform).GetComponent<T>();

        if (newWindow is ISingleOne)
        {
            foreach (Window window in Lister._Windows)
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

        newWindow.SetLister(Lister);

        return newWindow;
    }

    public Window CreateWindowWithType(string type)
    {
        Window newWindow = null;

        foreach(GameObject window in Prefabs)
        {
            if(window.GetComponent<Window>().GetType().ToString() == type)
            {
                newWindow = Instantiate(window, transform).GetComponent<Window>();
                break;
            }
        }

        if(newWindow == null)
        {
            return null;
        }

        if(newWindow is ISingleOne)
        {
            foreach(Window window in Lister._Windows)
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

        newWindow.SetLister(Lister);

        return null;
    }
}
