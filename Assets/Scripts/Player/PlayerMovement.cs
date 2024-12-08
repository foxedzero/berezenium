
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController Controller;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private Transform Camera;
    [SerializeField] private Settings Settings;
    [SerializeField] private Animator Animator;

    [SerializeField] private Vector3 Rotation = Vector3.zero;
    [SerializeField] private float Speed;
    [SerializeField] private float Sensitivity;

    [SerializeField] private AudioSource WalkSound;
    [SerializeField] private AudioClip[] WalkSounds;

    private float CameraShake = 0;
    private float WalkNoise = 0;
    private float Fall = 0;

    public float _Speed
    {
        get
        {
            return Speed;
        }
        set
        {
            Speed = value;
        }
    }

    private void Start()
    {
        Rotation = transform.localEulerAngles;

        Settings.OnChanges += UpdateValues;
        UpdateValues();
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

        transform.localEulerAngles = new Vector3(0, Rotation.y, 0);
        Camera.localEulerAngles = new Vector3(Rotation.x, 0, Rotation.z);

        RaycastHit raycast;
        if (Fall > 0)
        {
            if (Physics.Raycast(transform.position, Vector2.down, out raycast, Mathf.Max(0.1f, Time.deltaTime * Mathf.Max(Fall, 0.1f)), LayerMask))
            {
                Fall = 0;
                transform.position = raycast.point;
            }
            else
            {
                if (Fall < 15)
                {
                    Fall += Time.deltaTime * 10;
                }

                transform.position -= Vector3.up * Fall * Time.deltaTime;
            }
        }
        else
        {
            if (!Physics.Raycast(transform.position, Vector2.down, out raycast, 0.1f, LayerMask))
            {
                Fall += Time.deltaTime;
            }
        }
        Vector3 direction = Vector3.zero;
        direction += transform.forward * InputManager.GetAxis(InputManager.AxisEnum.Vertical);
        direction += transform.right * InputManager.GetAxis(InputManager.AxisEnum.Horizontal);
   
        if (direction.x != 0 || direction.z != 0)
        {
            Animator.SetBool("isWalking", true);

            CameraShake = (CameraShake + Time.unscaledDeltaTime * Speed * 90) % 360;
            WalkNoise += Time.unscaledDeltaTime * Speed;
            if (WalkNoise > 4)
            {
                WalkNoise -= 4;
                
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.25f, Vector3.down, out hit, 1))
                {
                    Ground ground = hit.transform.GetComponent<Ground>();
                    if (ground != null)
                    {
                        int sound = 0;

                        switch (ground._Material)
                        {
                            case Ground.GroundMaterial.Wood:
                                sound = Random.Range(0, 2);
                                break;
                            case Ground.GroundMaterial.Metal:
                                sound = Random.Range(2, 4);
                                break;
                            case Ground.GroundMaterial.Beton:
                                sound = Random.Range(4, 6);
                                break;
                            case Ground.GroundMaterial.Snow:
                                sound = Random.Range(6, 8);
                                break;
                        }

                        WalkSound.clip = WalkSounds[sound];
                        WalkSound.pitch = Random.Range(0.9f, 1.1f);
                        WalkSound.Play();
                    }
                }
            }

            Camera.transform.localPosition = new Vector3(0, 1.75f + Mathf.Sin(CameraShake * Mathf.Deg2Rad) * 0.05f, 0);
          //  Rotation.z = Mathf.Sin(CameraShake * Mathf.Deg2Rad) * 1f;

            Controller.Move(direction.normalized * Time.unscaledDeltaTime * Speed);
        }
        else
        {

            Animator.SetBool("isWalking", false);
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -250, 250), Mathf.Clamp(transform.position.y, 0, 100), Mathf.Clamp(transform.position.z, -250, 250));
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
}
