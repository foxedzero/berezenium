using System.Collections;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] private Vector2 MousePosition = Vector2.zero;
    [SerializeField] private Vector2 AngleAmplitude;
    [SerializeField] private float Speed;

    private void Update()
    {
        Vector2 mouseViewPosition = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - Vector3.one / 2f) * 2;

        float magnitude = (MousePosition - mouseViewPosition).magnitude;
        if (magnitude > 0.01f)
        {
            MousePosition += Speed * Time.deltaTime * (mouseViewPosition - MousePosition);
        }

        Vector2 angles = new Vector2(Mathf.Clamp(AngleAmplitude.x * MousePosition.x, -AngleAmplitude.x, AngleAmplitude.x), Mathf.Clamp(AngleAmplitude.y * MousePosition.y, -AngleAmplitude.y, AngleAmplitude.y));

        transform.localEulerAngles = new Vector3(-angles.y, angles.x, 0);
    }

}
