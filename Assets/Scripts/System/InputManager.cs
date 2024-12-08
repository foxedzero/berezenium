using UnityEngine;
using System.IO;
using System;

public class InputManager : MonoBehaviour
{
    private static InputManager Instance = null;
    public static InputManager _Instance
    {
        get
        {
            if(Instance == null)
            {
                Instance = new GameObject("InputManager").AddComponent<InputManager>();
            }

            return Instance;
        }
    }

    public enum ButtonState { Down, Hold, Up }
    public enum AxisEnum {Horizontal, Vertical, CameraSpeedUp}
    public enum ButtonEnum { Cancel, Interact, Return, CameraDirection, Rotate, Pause, NextTimeScale }

    [SerializeField] private KeyMapData KeyMap;
#if UNITY_EDITOR
    [SerializeField] private bool UpdateKeyMap;
#endif

    public KeyMapData _KeyMap
    {
        get { return KeyMap; }
        set
        {
            KeyMap = value;
            UpdateKeys();
            SaveKeyMap();
        }
    }

    private Axis Horizontal = null;
    private Axis Vertical = null;
    private Axis CameraSpeedUp = null;

    private Button Cancel = null;
    private Button Interact = null;
    private Button Return = null;
    private Button CameraDirection = null;
    private Button Rotate = null;
    private Button Pause = null;
    private Button NextTimeScale = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        string path = Path.Combine(Application.dataPath, "keyConfig.txt");
        if (File.Exists(path))
        {
            KeyMap = JsonUtility.FromJson<KeyMapData>(File.ReadAllText(path).Replace("\n", ""));
        }
        else
        {
            KeyMap = new KeyMapData();
        }

        UpdateKeys();
    }

    private void OnApplicationQuit()
    {
        SaveKeyMap();
        Instance = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UpdateKeyMap)
        {
            UpdateKeys();
            SaveKeyMap();

            UpdateKeyMap = false;
        }
    }
#endif

    public void SaveKeyMap() => File.WriteAllText(Path.Combine(Application.dataPath, "keyConfig.txt"), JsonUtility.ToJson(KeyMap).Replace(",",",\n"));

    private void UpdateKeys()
    {
        Horizontal = new Axis(KeyMap.Horizontal);
        Vertical = new Axis(KeyMap.Vertical);
        CameraSpeedUp = new Axis(KeyMap.CameraSpeedUp);

        Cancel = new Button(KeyMap.Cancel);
        Interact = new Button(KeyMap.Interact);
        Return = new Button(KeyMap.Return);
        CameraDirection = new Button(KeyMap.CameraDirection);
        Rotate = new Button(KeyMap.Rotate);
        Pause = new Button(KeyMap.Pause);
        NextTimeScale = new Button(KeyMap.NextTimeScale);
    }

    public static float GetAxis(AxisEnum axis)
    {
        switch (axis)
        {
            case  AxisEnum.Horizontal:
                return _Instance.Horizontal.GetValue();
            case AxisEnum.Vertical:
                return _Instance.Vertical.GetValue();
            case AxisEnum.CameraSpeedUp:
                return _Instance.CameraSpeedUp.GetValue();
        }

        Debug.LogError($"аксиса <color=white>{axis}</color> не существует");

        return 0;
    }

    private bool GetButtonState(ButtonEnum button, ButtonState state)
    {
        switch (button)
        {
            case ButtonEnum.Cancel:
                return Cancel.CheckState(state);
            case ButtonEnum.Interact:
                return Interact.CheckState(state);
                case ButtonEnum.Return:
                return Return.CheckState(state);
            case ButtonEnum.CameraDirection:
                return CameraDirection.CheckState(state);
            case ButtonEnum.Rotate:
                return Rotate.CheckState(state);
            case ButtonEnum.Pause:
                return Pause.CheckState(state);
            case ButtonEnum.NextTimeScale:
                return NextTimeScale.CheckState(state);
        }

        Debug.LogError($"кнопка <color=white>{button}</color> не найдена");

        return false;
    }

    public static bool GetButtonDown(ButtonEnum button) => _Instance.GetButtonState(button, ButtonState.Down);

    public static bool GetButton(ButtonEnum button) => _Instance.GetButtonState(button, ButtonState.Hold);

    public static bool GetButtonUp(ButtonEnum button) => _Instance.GetButtonState(button, ButtonState.Up);

    public class Axis
    {
        private KeyCode[] PositiveKeys = new KeyCode[0];
        private KeyCode[] NegativeKeys = new KeyCode[0];

        public Axis(string keys)
        {
            KeyCode[][] parsed = ParseKeys(keys);
            PositiveKeys = parsed[0];
            NegativeKeys = parsed[1];
        }

        public float GetValue()
        {
            float value = 0;

            foreach(KeyCode key in PositiveKeys)
            {
                if (Input.GetKey(key))
                {
                    value += 1;
                    break;
                }
            }

            foreach (KeyCode key in NegativeKeys)
            {
                if (Input.GetKey(key))
                {
                    value -= 1;
                    break;
                }
            }

            return value;
        }

        public static KeyCode[][] ParseKeys(string keys)
        {
            KeyCode[] positiveKeys = new KeyCode[0];
            KeyCode[] negativeKeys = new KeyCode[0];

            bool positive = true;
            string key = "";

            foreach (char letter in keys)
            {
                switch (letter)
                {
                    case '+':
                        positive = true;
                        break;
                    case '-':
                        positive = false;
                        break;
                    case ' ':
                        if (positive)
                        {
                            positiveKeys = StaticTools.ExpandMassive(positiveKeys, GetKeyCode(key));

                            key = "";
                        }
                        else
                        {
                            negativeKeys = StaticTools.ExpandMassive(negativeKeys, GetKeyCode(key));

                            key = "";
                            positive = false;
                        }
                        break;
                    default:
                        key += letter;
                        break;
                }
            }

            if (key.Length > 0)
            {
                if (positive)
                {
                    positiveKeys = StaticTools.ExpandMassive(positiveKeys, GetKeyCode(key));
                }
                else
                {
                    negativeKeys = StaticTools.ExpandMassive(negativeKeys, GetKeyCode(key));
                }
            }

            return new KeyCode[][] { positiveKeys, negativeKeys };
        }
    }

    public class Button
    {
        private KeyCode[] Keys = new KeyCode[0];

        public Button(string keys) => Keys = ParseKeys(keys);

        public bool CheckState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Down:
                    foreach (KeyCode key in Keys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            return true;
                        }
                    }
                    break;
                case ButtonState.Hold:
                    foreach (KeyCode key in Keys)
                    {
                        if (Input.GetKey(key))
                        {
                            return true;
                        }
                    }
                    break;
                case ButtonState.Up:
                    foreach (KeyCode key in Keys)
                    {
                        if (Input.GetKeyUp(key))
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public static KeyCode[] ParseKeys(string keysInfo)
        {
            KeyCode[] keys = new KeyCode[0];
            string key = "";

            foreach (char letter in keysInfo)
            {
                if (letter == ' ')
                {
                    keys = StaticTools.ExpandMassive(keys, GetKeyCode(key));
                    key = "";
                }
                else
                {
                    key += letter;
                }
            }

            if (key.Length > 0)
            {
                keys = StaticTools.ExpandMassive(keys, GetKeyCode(key));
            }

            return keys;
        }
    }

    private static KeyCode GetKeyCode(string key)
    {
        foreach(KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
        {
            if(code.ToString().ToLower() == key.ToLower())
            {
                return code;
            }
        }

        Debug.LogError($"клавиша <color=white>{key}</color> недействительна");

        return KeyCode.None;
    }
}

[Serializable]
public class KeyMapData
{
    public string Horizontal = "+D +RightArrow -A -LeftArrow";
    public string Vertical = "+W +UpArrow -S -DownArrow";
    public string CameraSpeedUp = $"+{KeyCode.Equals} -{KeyCode.Minus}";

    public string Cancel = "Escape";

    public string Return = $"Return";

    public string Interact = $"E F Mouse0";

    public string CameraDirection = $"LeftAlt Mouse2";

    public string Rotate = "R";
   
    public string Pause = "Space";
    public string NextTimeScale = "Tab";
}