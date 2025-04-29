using UnityEngine;
using System.Collections;

public class BearRotator : MonoBehaviour
{
    private Coroutine Coroutine = null;
    [SerializeField] private float Sensitivity;

    private void Update()
    {
        if (!MouseCheckUI.OnUI && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Coroutine != null)
            {
                StopCoroutine(Coroutine);
            }
            Coroutine = StartCoroutine(EditCamera());
        }
    }

    private IEnumerator EditCamera()
    {
        float y = transform.localEulerAngles.y;
        while (Input.GetKey(KeyCode.Mouse0))
        {
            y -= Input.GetAxis("Mouse X") * Sensitivity;

            transform.localEulerAngles = new Vector3(0, y ,0);

            yield return new WaitForEndOfFrame();
        }

        Coroutine = null;
    }
}
