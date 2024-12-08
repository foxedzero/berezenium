using UnityEngine;
using WindowInterfaces;

public class WindowLister : MonoBehaviour
{ 
    [SerializeField] private Window Current;
    [SerializeField] private Window[] Windows;
    [SerializeField] private RectTransform Canvas;

    public event SimpleVoid OnCurrentChange;
    public event SimpleVoid OnWindowsCountChange;

    public Window _Current => Current;
    public Window[] _Windows => Windows;
    public RectTransform _Canvas => Canvas;

    public void CloseAll()
    {
        foreach(Window window in Windows)
        {
            if(window is INotAllClosable)
            {

            }
            else
            {
                window.Close();
            }
        }

        Debug.Log($"<color=yellow>закрыты все доступные окна</color>");
    }

    public void Select(Window window)
    {
        if (Current != null)
        {
            Current.Deselect();
        }

        Current = window;

        if(Current is IMetaVisible)
        {
            Current.transform.SetAsLastSibling();
        }
        else
        {
            Current.transform.SetSiblingIndex(transform.childCount - 1 - MetaVisibleCount());
        }

        if (OnCurrentChange != null)
        {
            OnCurrentChange.Invoke();
        }
    }

    public void RemoveWindow(Window window)
    {
        int windowIndex = StaticTools.IndexOf(Windows, window);
        if(windowIndex < 0)
        {
            return;
        }

        Windows = StaticTools.ReduceMassive(Windows, windowIndex);

        if (Current == window)
        {
            if (Windows.Length > 1)
            {
                Windows[Windows.Length - 1].Select();
            }
            else if (Windows.Length > 0)
            {
                Windows[0].Select();
            }
        }

        foreach (Window window1 in Windows)
        {
            if (window._Index <= window1._Index)
            {
                window1.DirectSetIndex(window1._Index - 1);
            }
        }

        if (OnWindowsCountChange != null)
        {
            OnWindowsCountChange.Invoke();
        }
    }

    public void AddWindow(Window window)
    {
        Windows = StaticTools.ExpandMassive(Windows, window);

        window.Select();
        window.DirectSetIndex(Windows.Length - 1);

        if(OnWindowsCountChange != null)
        {
            OnWindowsCountChange.Invoke();
        }
    }

    public void ChangeWindowIndex(Window window, int index)
    {
        if(index >= Windows.Length)
        {
            index = Windows.Length - 1;
        }
        else if(index < 0)
        {
            index = 0;
        }

        Window[] newMassive = new Window[Windows.Length];
        int lastIndex = window._Index;

        int originIndex = 0;
        int destinationIndex = 0;
        bool newIndexPassed = false;
        for(int i = 0; i < newMassive.Length; i++)
        {
            if (destinationIndex == index)
            {
                newMassive[destinationIndex] = Windows[lastIndex];
                Windows[lastIndex].DirectSetIndex(destinationIndex);
                destinationIndex++;

                newIndexPassed = true;
            }
            else if(destinationIndex == lastIndex)
            {
                if (!newIndexPassed)
                {
                    originIndex++;
                }

                newMassive[destinationIndex] = Windows[originIndex];

                Windows[originIndex].DirectSetIndex(destinationIndex);

                destinationIndex++;
                originIndex++;

                if (newIndexPassed)
                {
                    originIndex++;
                }
            }
            else
            {
                newMassive[destinationIndex] = Windows[originIndex];

                Windows[originIndex].DirectSetIndex(destinationIndex);

                destinationIndex++;
                originIndex++;
            }
        }

        Windows = newMassive;
    }

    private int MetaVisibleCount()
    {
        int itog = 0;

        foreach(Window window in Windows)
        {
            if(window is IMetaVisible)
            {
                itog++;
            }
        }

        return itog;
    }
}
