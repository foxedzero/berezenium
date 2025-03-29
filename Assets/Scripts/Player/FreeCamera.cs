
using System.Collections;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [SerializeField] private Settings Settings;
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Vector3 Rotation = Vector3.zero;
    [SerializeField] private float Speed;
    [SerializeField] private float Sensibility;
    [SerializeField] private CursorIconInfo CursorIcon;
    private Coroutine Coroutine = null;
    private bool Initialized = false;

    private float Border = 0;

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        CursorManager.EditIcon(CursorIcon, true);
    }

    private void OnEnable()
    {
        if (!Initialized)
        {
            transform.position = PlayerCamera.position + Vector3.up * 4;
            transform.eulerAngles = new Vector3(0, PlayerCamera.eulerAngles.y, 0);
            Initialized = true;
        }
    }

    private void Start()
    {
        Rotation = transform.localEulerAngles;

        Settings.OnChanges += UpdateValues;
        UpdateValues();


        switch (SaveManager._Instance._SaveData.MapSize)
        {
            case 0:
                Border = 150;
                break;
            case 1:
                Border = 175;
                break;
            case 2:
                Border = 200;
                break;
        }
    }

    public void UpdateValues()
    {
        Sensibility = Settings._Data.Sensitivity;
    }

    private void Update()
    {
        if (!MouseCheckUI.OnUI && InputManager.GetButtonDown(InputManager.ButtonEnum.CameraDirection))
        {
            if (Coroutine != null)
            {
                StopCoroutine(Coroutine);
            }
            Coroutine = StartCoroutine(EditCamera());
        }

            Speed = Mathf.Clamp(Speed + InputManager.GetAxis(InputManager.AxisEnum.CameraSpeedUp) * 15 * Time.unscaledDeltaTime, 1, 50);

        //Rotation += Sensibility * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        //NormalizeRotation();

        //if (Rotation.x < 270 && Rotation.x > 90)
        //{
        //    if (Rotation.x > 135)
        //    {
        //        Rotation.x = 270;
        //    }
        //    else
        //    {
        //        Rotation.x = 90;
        //    }
        //}

        //transform.localEulerAngles = Rotation;


        //Vector3 direction = Vector3.zero;
        //direction += Vector3.forward * InputManager.GetAxis(InputManager.AxisEnum.Vertical);
        //direction += Vector3.right * InputManager.GetAxis(InputManager.AxisEnum.Horizontal);

        //if (direction.x != 0 || direction.z != 0)
        //{
        //    direction *= Time.deltaTime * Speed;
        //    transform.position += transform.forward * direction.z + transform.right * direction.x;
        //  //  transform.position = new Vector3(Mathf.Clamp(transform.position.x, -ClampSize.x, ClampSize.x), Mathf.Clamp(transform.position.y, -ClampSize.y, ClampSize.y), Mathf.Clamp(transform.position.z, -ClampSize.z, ClampSize.z));
        //}

    }

    private void NormalizeRotation()
    {
        Rotation.x %= 360;
        if (Rotation.x < 0)
        {
            Rotation.x += 360;
        }
        Rotation.x %= 360;
    }

    private IEnumerator EditCamera()
    {
        Cursor.lockState = CursorLockMode.Confined;
        CursorManager.EditIcon(CursorIcon, false);

        while (InputManager.GetButton(InputManager.ButtonEnum.CameraDirection))
        {
            Rotation += Sensibility * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            NormalizeRotation();

            if(Input.GetAxis("Mouse Y") != 0 && Input.GetAxis("Mouse X") != 0)
            {
                NewTutorialSystem.Instance.ManageModeCameraRotated();
            }

            if (Rotation.x < 270 && Rotation.x > 90)
            {
                if (Rotation.x > 135)
                {
                    Rotation.x = 270;
                }
                else
                {
                    Rotation.x = 90;
                }
            }

            transform.localEulerAngles = Rotation;


            Vector3 direction = Vector3.zero;
            direction += Vector3.forward * InputManager.GetAxis(InputManager.AxisEnum.Vertical);
            direction += Vector3.right * InputManager.GetAxis(InputManager.AxisEnum.Horizontal);

            if (direction.x != 0 || direction.z != 0)
            {
                    NewTutorialSystem.Instance.ManageModeCameraMoved();



                direction *= Time.unscaledDeltaTime * Speed;
                transform.position += transform.forward * direction.z + transform.right * direction.x;
               transform.position = new Vector3(Mathf.Clamp(transform.position.x, -Border/2, Border/2), Mathf.Clamp(transform.position.y, 0, 70), Mathf.Clamp(transform.position.z, -Border/2, Border/2));
            }

            yield return new WaitForEndOfFrame();
        }

        Coroutine = null;

        Cursor.lockState = CursorLockMode.None;
        CursorManager.EditIcon(CursorIcon, true);
    }
}
