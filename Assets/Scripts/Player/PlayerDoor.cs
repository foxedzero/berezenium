using UnityEngine;

public class PlayerDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource Audio;
    [SerializeField] private Vector3 Point;

    public void Interact()
    {
        FindObjectOfType<CameraChanger>()._MapCamera = false;
        FindObjectOfType<PlayerMovement>().transform.position = Point;
        Audio.pitch = Random.Range(0.95f, 1.05f);
        Audio.Play();
    }
}
