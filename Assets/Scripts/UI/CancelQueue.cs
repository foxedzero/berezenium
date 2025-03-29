using UnityEngine;

public interface ICancelable
{
    public void Cancel();
}

public class CancelQueue : MonoBehaviour
{
    private static CancelQueue Instance;

    private ICancelable[] Queue = new ICancelable[0];

    private void Awake()
    {
        Instance = this;
    }

    public static void Register(ICancelable cancelable, bool remove)
    {
        if (remove)
        {
            Instance.Queue = StaticTools.RemoveFromMassive(Instance.Queue, cancelable);
        }
        else
        {
            Instance.Queue = StaticTools.ExcludingExpandMassive(Instance.Queue, cancelable);
        }
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Cancel))
        {
            if(Queue.Length > 0)
            {
                Queue[Queue.Length - 1].Cancel();
            }
        }
    }
}
