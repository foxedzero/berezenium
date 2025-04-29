using UnityEngine;

public class TimeEditor : MonoBehaviour
{
    [SerializeField] private Weather Weather;

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
            if (City._Factors._GameEnded)
            {
                value = true;
            }

            Paused = value;

            if (Paused)
            {
                NewTutorialSystem.Instance.GamePaused();
            }

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
        if (!CameraChanger.Instance._MapCamera)
        {
            Paused = false;
            TimeIndex = 0;

            if (Menued)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            if (OnTimeUpdate != null)
            {
                OnTimeUpdate.Invoke();
            }

            Weather.FreezeSnow(gameObject, true);

            return;
        }

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

        Weather.FreezeSnow(gameObject, !Paused);

        if (OnTimeUpdate != null)
        {
            OnTimeUpdate.Invoke();
        }
    }

    private void Update()
    {
        if (!CameraChanger.Instance._MapCamera)
        {
            return;
        }

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
