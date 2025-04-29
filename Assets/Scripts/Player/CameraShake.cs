using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform Camera;
    [SerializeField] private float ShakeTime;
    [SerializeField] private float Amplitude;
    [SerializeField] private float Speed;
    private bool Stop = true;

    private float Target;
    private float Value;

    public void Shake(float time, float amplitude)
    {
        ShakeTime = time;
        Amplitude = amplitude;
        Stop = false;

        Target = Random.Range(-amplitude, amplitude);

        Camera.localEulerAngles = new Vector3(Camera.localEulerAngles.x, Camera.localEulerAngles.y, Camera.localEulerAngles.z);
    }

    private void Update()
    {
        ShakeTime -= Time.unscaledDeltaTime;
        if(ShakeTime <= 0)
        {
            Target = 0;
        }

        Vector3 rotation = Camera.localEulerAngles;

        if (Mathf.Abs(Target - Value) < 0.1f)
        {
            Value = Target;
            Target = Random.Range(-Amplitude, Amplitude);

            if(ShakeTime <= 0)
            {
                rotation.z = Value;

                Camera.localEulerAngles = rotation;

                Stop = true;
                return;
            }
        }

        Value += Mathf.Sign(Target - Value) * Speed * Time.unscaledDeltaTime;

        rotation.z = Value;

        Camera.localEulerAngles = rotation;
    }
}
