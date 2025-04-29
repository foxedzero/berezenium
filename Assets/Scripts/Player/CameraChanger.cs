using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    public static CameraChanger Instance;

    [SerializeField] private TimeEditor TimeEditor;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private GameObject Windows;
    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private PlayerVisual PlayerVisual;
    [SerializeField] private GameObject PlayerInterface;
    [SerializeField] private GameObject SallyPanel;

    [SerializeField] private QuitCapitansMode QuitCapitansMode;

    [SerializeField] private GameObject MapCamera;
    [SerializeField] private GameObject MapInterface;

    [SerializeField] private bool UseMap = false;

    private NeedCursorOrder NeedCursor = new NeedCursorOrder();

    public Transform _Camera => UseMap ? MapCamera.transform : PlayerCamera.transform;
    public bool _MapCamera
    {
        get
        {
            return UseMap;
        }
        set
        {
            if (City._Factors._GameEnded)
            {
                value = true;
            }

            UseMap = value;

            QuitCapitansMode.enabled = value;

            Windows.SetActive(value);

            PlayerVisual.ShowModel(value);
            PlayerVisual.SetManaging(value);

            PlayerCamera.SetActive(!value);
            PlayerMovement.enabled = !value;
            PlayerInterface.SetActive(!value);

            MapCamera.SetActive(value);
            MapInterface.SetActive(value);

            if (!value)
            {
                TimeEditor._TimeIndex = 0;
                SallyPanel.SetActive(false);
            }

            CursorManager.SetNeedMouse(NeedCursor, !UseMap);
        }
    }

    private void Awake()
    {
        Instance = this;
        _MapCamera = UseMap;
    }

    private void Update()
    {
        if(!NewTutorialSystem.Instance._Closed && NewTutorialSystem.Instance._TutorialStage < 5)
        {
            return;
        }

        if (InputManager.GetButtonDown(InputManager.ButtonEnum.ManageMode) && !PlayerMovement._InCapsule)
        {
            _MapCamera = !UseMap;
        }
    }

    public void ChangeCamera()
    {
        _MapCamera = !UseMap;
    }
}
