using UnityEngine;

public class TimeEditor : MonoBehaviour
{
    [SerializeField] private int TimeIndex;
    [SerializeField] private bool Menued;
    [SerializeField] private bool Paused;

    public event SimpleVoid OnTimeUpdate = null;

    public int _TimeIndex
    {
        get
        {
            return TimeIndex;
        }
        set
        {
            TimeIndex = value;

            UpdateTime();
        }
    }
    public bool _Paused
    {
        get
        {
            return Paused;
        }
        set
        {
            Paused = value;

            UpdateTime();
        }
    }
    public bool _Menued
    {
        get
        {
            return Menued;
        }
        set
        {
            Menued = value;

            UpdateTime();
        }
    }

    private void Start()
    {
        _TimeIndex = 0;
    }

    private void UpdateTime()
    {
        if (Menued || Paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            switch (TimeIndex)
            {
                case 0:
                    Time.timeScale = 1;
                    break;
                case 1:
                    Time.timeScale = 2;
                    break;
                case 2:
                    Time.timeScale = 4;
                    break;
                case 3:
                    Time.timeScale = 8;
                    break;
            }
        }

        if(OnTimeUpdate != null)
        {
            OnTimeUpdate.Invoke();
        }
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.Pause))
        {
            _Paused = !Paused;
        }
        else if (InputManager.GetButtonDown(InputManager.ButtonEnum.NextTimeScale))
        {
            _TimeIndex = (TimeIndex + 1) % 4;
        }
    }
}
