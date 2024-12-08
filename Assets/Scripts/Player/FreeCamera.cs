using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCamera : MonoBehaviour
{
    [SerializeField] private Settings Settings;
    [SerializeField] private Vector3 Rotation = Vector3.zero;
    [SerializeField] private Vector3 ClampSize = Vector3.one;
    [SerializeField] private float Speed;
    [SerializeField] private float Sensibility;
    [SerializeField] private CursorIconInfo CursorIcon;
    private Coroutine Coroutine = null;
    private bool Hold = false;

    private void Start()
    {
        Rotation = transform.localEulerAngles;

        Settings.OnChanges += UpdateValues;
        UpdateValues();
    }

    public void UpdateValues()
    {
        Sensibility = Settings._Data.Sensitivity;
    }

    private void Update()
    {
        if (CheckClick())
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

    private bool CheckClick()
    {
        if (InputManager.GetButtonDown(InputManager.ButtonEnum.CameraDirection))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>(0);
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count == 0)
            {
                return true;
            }
        }

        return false;
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
                direction *= Time.unscaledDeltaTime * Speed;
                transform.position += transform.forward * direction.z + transform.right * direction.x;
               transform.position = new Vector3(Mathf.Clamp(transform.position.x, -ClampSize.x, ClampSize.x), Mathf.Clamp(transform.position.y, 0, ClampSize.y), Mathf.Clamp(transform.position.z, -ClampSize.z, ClampSize.z));
            }

            yield return new WaitForEndOfFrame();
        }

        Coroutine = null;

        Cursor.lockState = CursorLockMode.None;
        CursorManager.EditIcon(CursorIcon, true);
    }
}
