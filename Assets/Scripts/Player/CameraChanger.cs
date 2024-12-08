using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private EnvironmentPass EnvironmentPass;
    [SerializeField] private GameObject PlayerInterface;
    [SerializeField] private GameObject PlayerModel;

    [SerializeField] private GameObject MapCamera;
    [SerializeField] private GameObject MapInterface;

    [SerializeField] private bool UseMap = false;

    private NeedCursorOrder NeedCursor = new NeedCursorOrder();

    public bool _MapCamera
    {
        get
        {
            return UseMap;
        }
        set
        {
            UseMap = value;

            EnvironmentPass.SetPass(!value);

            foreach (SkinnedMeshRenderer renderer in PlayerModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (value)
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }

            PlayerCamera.SetActive(!value);
            PlayerMovement.enabled = !value;
            PlayerInterface.SetActive(!value);

            MapCamera.SetActive(value);
            MapInterface.SetActive(value);

            CursorManager.SetNeedMouse(NeedCursor, !UseMap);
        }
    }

    private void Start()
    {
        _MapCamera = UseMap;
    }

    public void ChangeCamera()
    {
        _MapCamera = !UseMap;
    }
}
