using UnityEngine;

public class VrStep : MonoBehaviour
{
    [SerializeField] private LayerMask GroundMask;

    [SerializeField] private Texture StepTexture;

    [SerializeField] private AudioSource WalkSound;

    public void Step(int point)
    {
        SnowDowner.AddDown(new SnowDowner.DownAsk(StepTexture, transform.position.x + transform.forward.x * 0.2f, transform.position.z + transform.forward.z * 0.2f, transform.eulerAngles.y, 150));

        if (Vector3.Distance(CameraChanger.Instance._Camera.position, transform.position) > 5)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.25f, Vector3.down, out hit, 1, GroundMask))
        {
            Ground ground = hit.transform.GetComponent<Ground>();
            if (ground != null)
            {
                WalkSound.pitch = Random.Range(1f, 1.2f);
                WalkSound.Play();
            }
        }
    }
}
