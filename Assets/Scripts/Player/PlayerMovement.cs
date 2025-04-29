
using System.Drawing;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController Controller;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private Transform CameraRotator;
    [SerializeField] private Settings Settings;
    [SerializeField] private Animator Animator;

    [SerializeField] private PlayerVisual PlayerVisual;

    [SerializeField] private float Height;

    [SerializeField] private Vector3 Rotation = Vector3.zero;
    [SerializeField] private float Sensitivity;

    [SerializeField] private bool InCapsule;

    private float Border = 0;

    private float Fall = 0;

    public Vector3 _Rotation
    {
        get
        {
            return Rotation;
        }
        set
        {
            Rotation = value;
        }
    }
    public bool _InCapsule
    {
        get
        {
            return InCapsule;
        }
        set
        {
            InCapsule = value;
        }
    }

    private void Start()
    {
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

    private void OnDisable()
    {
        Animator.SetBool("isWalking", false);
    }

    public void UpdateValues()
    {
        Sensitivity = Settings._Data.Sensitivity;
    }

    private void Update()
    {
        if (CursorManager._UILocked)
        {
            Animator.SetBool("isWalking", false);

            return;
        }

        Rotation += Sensitivity * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        Rotation.x = NormalizeRotation(Rotation.x);
        Rotation.y = NormalizeRotation(Rotation.y);

        if (InCapsule)
        {
            if (Rotation.x < 270 && Rotation.x > 0)
            {
                if (Rotation.x > 135)
                {
                    Rotation.x = 270;
                }
                else
                {
                    Rotation.x = 0;
                }
            }
            if (Rotation.y < 270 && Rotation.y > 90)
            {
                if (Rotation.y > 135)
                {
                    Rotation.y = 270;
                }
                else
                {
                    Rotation.y = 90;
                }
            }

            CameraRotator.eulerAngles = new Vector3(Rotation.x, Rotation.y, Rotation.z);
            return;
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

        Fall += Time.unscaledDeltaTime * 9.81f;
        if (Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector2.down, out RaycastHit raycast, Fall * Time.unscaledDeltaTime + 0.02f, LayerMask))
        {
            Fall = 0;
        }
        Controller.Move(Vector3.down * Time.unscaledDeltaTime * Fall);

        float y = InputManager.GetAxis(InputManager.AxisEnum.Vertical);
        float x = InputManager.GetAxis(InputManager.AxisEnum.Horizontal);
        if(x != 0 || y != 0)
        {
            Animator.SetBool("isWalking", true);
            transform.localEulerAngles = new Vector3(0, Rotation.y, 0);

            Vector3 direction = Vector3.zero;
            direction += transform.forward * y;
            direction += transform.right * x;
            direction = direction.normalized;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude, LayerMask))
            {

            }
            else if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.2f, LayerMask))
            {
                direction -= Vector3.Dot(direction, hit.normal) * hit.normal;
            }

            Controller.Move(direction * Time.unscaledDeltaTime * GlobalVariables._PlayerSpeed);
       
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -Border / 2, Border / 2), Mathf.Clamp(transform.position.y, -100, 100), Mathf.Clamp(transform.position.z, -Border / 2, Border / 2));
        }
        else
        {
            Animator.SetBool("isWalking", false);

            float yRot = transform.localEulerAngles.y;
            if(yRot - Rotation.y > 180)
            {
                yRot -= 360;
            }
            if (Rotation.y - yRot > 180)
            {
                yRot += 360;
            }
            if (Mathf.Abs(Rotation.y - yRot) > 30)
            {
                if (Rotation.y > yRot)
                {
                    yRot = Rotation.y - 30;
                }
                else
                {
                    yRot = Rotation.y + 30;
                }
                yRot = NormalizeRotation(yRot);
            }
            transform.localEulerAngles = new Vector3(0, yRot, 0);
        }

        CameraRotator.eulerAngles = new Vector3(Rotation.x, Rotation.y, Rotation.z);
    }

    private float NormalizeRotation(float value)
    {
        value %= 360;
        if (value < 0)
        {
            value += 360;
        }
        value %= 360;

        return value;
    }
}
