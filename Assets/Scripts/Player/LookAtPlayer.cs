using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private CameraChanger CameraChanger;

    private void Start()
    {
       CameraChanger =  CameraChanger.Instance;
    }

    void Update()
    {
        transform.eulerAngles = Quaternion.LookRotation(transform.position - CameraChanger._Camera.position ).eulerAngles;

        transform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(CameraChanger._Camera.position, transform.position) * 0.035f, 1, 8);
    }
}
