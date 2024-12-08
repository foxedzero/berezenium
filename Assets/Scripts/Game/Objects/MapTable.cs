using UnityEngine;

public class MapTable : MonoBehaviour,IInteractable
{
    [SerializeField] private CameraChanger CameraChanger;

   public void Interact()
    {
        CameraChanger.ChangeCamera();
    }
}
