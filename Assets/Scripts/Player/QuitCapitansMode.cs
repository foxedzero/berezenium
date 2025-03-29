using UnityEngine;

public class QuitCapitansMode : MonoBehaviour, IInteractable
{
    [SerializeField] private CameraChanger CameraChanger;
    [SerializeField] private GameObject Collider;
    [SerializeField] private Outline Outline;

    public string _Info => "";
    public bool _AbstractUse => true;

    private void OnEnable()
    {
        Collider.SetActive(true);
    }

    private void OnDisable()
    {
        Outline.enabled = false;
        Collider.SetActive(false);
    }

    public void Indicate(bool state) => Outline.enabled = state;

     public void Interact()
    {
        CameraChanger._MapCamera = false;
    }
}
