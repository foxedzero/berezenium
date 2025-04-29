using UnityEngine;
using UnityEngine.UI;

public class CapitansHotkey : MonoBehaviour
{
    [SerializeField] private GameObject Button;
    [SerializeField] private Text Hotkey;
    private bool InCapsule = false;

    public bool _InCapsule
    {
        get
        {
            return InCapsule;
        }
        set
        {
            InCapsule = value;
            Button.SetActive(!value);
        }
    }

    private void OnEnable()
    {
        Hotkey.text = InputManager._Instance._ManageMode._Keys[0].ToString();
    }
}
